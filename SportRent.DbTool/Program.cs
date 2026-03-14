using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;

internal static class Program
{
    private static readonly Regex ScriptFilePattern = new(
        @"^\d{3}_.+\.sql$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnoreCase);

    private static int Main(string[] args)
    {
        try
        {
            BuildOptions options = ParseArguments(args);
            string root = ResolveRoot(options.Root);
            string scriptsDirectory = ResolvePath(root, options.ScriptsDirectory ?? "database");
            string outputPath = ResolvePath(
                root,
                options.OutputPath ?? Path.Combine("SportRent.Mobile", "Resources", "Raw", "Database", "sportRent.db"));
            string[] scripts = GetScriptFiles(scriptsDirectory);

            Console.WriteLine($"[INFO] Root: {root}");
            Console.WriteLine($"[INFO] Scripts dir: {scriptsDirectory}");
            Console.WriteLine($"[INFO] DB file: {outputPath}");
            Console.WriteLine($"[INFO] Scripts found: {scripts.Length}");

            BuildDatabase(outputPath, scripts);

            Console.WriteLine("[SUCCESS] Database created.");
            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine("[ERROR] Failed to build database.");
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static BuildOptions ParseArguments(string[] args)
    {
        string? root = null;
        string? scriptsDirectory = null;
        string? outputPath = null;

        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i];

            if (arg is "--help" or "-h")
            {
                PrintUsage();
                Environment.Exit(0);
            }

            string value = ReadValue(args, ref i);

            switch (arg)
            {
                case "--root":
                    root = value;
                    break;
                case "--scripts-dir":
                    scriptsDirectory = value;
                    break;
                case "--output":
                    outputPath = value;
                    break;
                default:
                    throw new ArgumentException($"Unknown argument: {arg}");
            }
        }

        return new BuildOptions(root, scriptsDirectory, outputPath);
    }

    private static string ReadValue(string[] args, ref int index)
    {
        if (index + 1 >= args.Length)
        {
            throw new ArgumentException($"Expected a value after {args[index]}");
        }

        index++;
        return args[index];
    }

    private static string ResolveRoot(string? rootArgument)
    {
        if (!string.IsNullOrWhiteSpace(rootArgument))
        {
            string root = Path.GetFullPath(rootArgument);

            if (!Directory.Exists(root))
            {
                throw new DirectoryNotFoundException($"Root directory not found: {root}");
            }

            return root;
        }

        return FindProjectRoot();
    }

    private static string ResolvePath(string root, string path)
    {
        return Path.GetFullPath(Path.IsPathRooted(path) ? path : Path.Combine(root, path));
    }

    private static string[] GetScriptFiles(string scriptsDirectory)
    {
        if (!Directory.Exists(scriptsDirectory))
        {
            throw new DirectoryNotFoundException($"Scripts directory not found: {scriptsDirectory}");
        }

        string[] scripts = Directory
            .EnumerateFiles(scriptsDirectory, "*.sql", SearchOption.TopDirectoryOnly)
            .Where(path => ScriptFilePattern.IsMatch(Path.GetFileName(path)))
            .OrderBy(path => Path.GetFileName(path), StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (scripts.Length == 0)
        {
            throw new InvalidOperationException(
                $"No numbered SQL scripts matching 000_name.sql were found in {scriptsDirectory}");
        }

        return scripts;
    }

    private static void BuildDatabase(string outputPath, IReadOnlyList<string> scripts)
    {
        string artifactsDirectory = Path.GetDirectoryName(outputPath)
            ?? throw new InvalidOperationException($"Cannot determine output directory for {outputPath}");

        Directory.CreateDirectory(artifactsDirectory);

        string tempPath = Path.Combine(
            artifactsDirectory,
            $"{Path.GetFileNameWithoutExtension(outputPath)}.{Guid.NewGuid():N}{Path.GetExtension(outputPath)}");

        try
        {
            using (SqliteConnection connection = OpenConnection(tempPath))
            {
                using SqliteTransaction transaction = connection.BeginTransaction();

                foreach (string scriptPath in scripts)
                {
                    ExecuteScript(connection, transaction, scriptPath);
                }

                transaction.Commit();
            }

            ReplaceDatabase(tempPath, outputPath);
        }
        catch
        {
            TryDeleteFile(tempPath);
            throw;
        }
    }

    private static SqliteConnection OpenConnection(string dbPath)
    {
        string connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            ForeignKeys = true,
            Pooling = false
        }.ToString();

        var connection = new SqliteConnection(connectionString);
        connection.Open();

        return connection;
    }

    private static void ExecuteScript(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string scriptPath)
    {
        if (!File.Exists(scriptPath))
        {
            throw new FileNotFoundException("SQL script not found.", scriptPath);
        }

        string sql = File.ReadAllText(scriptPath);

        if (string.IsNullOrWhiteSpace(sql))
        {
            throw new InvalidOperationException($"SQL script is empty: {scriptPath}");
        }

        Console.WriteLine($"[INFO] Running {Path.GetFileName(scriptPath)}");

        using SqliteCommand command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        _ = command.ExecuteNonQuery();
    }

    private static void ReplaceDatabase(string tempPath, string outputPath)
    {
        if (File.Exists(outputPath))
        {
            Console.WriteLine("[INFO] Replacing existing database...");
            RetryOnIOException(() => File.Delete(outputPath), $"Unable to delete existing database: {outputPath}");
        }

        RetryOnIOException(() => File.Move(tempPath, outputPath), $"Unable to place database at: {outputPath}");
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
        }
    }

    private static void RetryOnIOException(Action action, string failureMessage)
    {
        const int maxAttempts = 10;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            try
            {
                action();
                return;
            }
            catch (IOException) when (attempt < maxAttempts)
            {
                Thread.Sleep(200);
            }
        }

        throw new IOException(failureMessage);
    }

    private static string FindProjectRoot()
    {
        foreach (string startPath in new[] { Directory.GetCurrentDirectory(), AppContext.BaseDirectory })
        {
            var directory = new DirectoryInfo(startPath);

            while (directory != null)
            {
                if (Directory.Exists(Path.Combine(directory.FullName, "database")))
                {
                    return directory.FullName;
                }

                directory = directory.Parent;
            }
        }

        throw new DirectoryNotFoundException("Project root with database folder not found.");
    }

    private static void PrintUsage()
    {
        Console.WriteLine("SportRent.DbTool usage:");
        Console.WriteLine("  --root <path>         Optional project root.");
        Console.WriteLine("  --scripts-dir <path>  Optional SQL scripts directory. Default: database");
        Console.WriteLine("  --output <path>       Optional output DB path. Default: SportRent.Mobile/Resources/Raw/Database/sportRent.db");
    }

    private sealed record BuildOptions(string? Root, string? ScriptsDirectory, string? OutputPath);
}
