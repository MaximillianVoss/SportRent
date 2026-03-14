using Microsoft.Data.Sqlite;

internal class Program
{
    private static readonly string[] ScriptFiles =
    {
        "001_recreate.sql",
        "002_schema.sql",
        "003_seed.sql"
    };

    private static void Main(string[] args)
    {
        try
        {
            string root = FindProjectRoot();

            string databaseDir = Path.Combine(root, "database");
            string artifactsDir = Path.Combine(root, "artifacts");
            string dbPath = Path.Combine(artifactsDir, "sportRent.db");

            Console.WriteLine($"[INFO] Root: {root}");
            Console.WriteLine($"[INFO] Database dir: {databaseDir}");
            Console.WriteLine($"[INFO] Artifacts dir: {artifactsDir}");
            Console.WriteLine($"[INFO] DB file: {dbPath}");

            _ = Directory.CreateDirectory(artifactsDir);

            if (File.Exists(dbPath))
            {
                Console.WriteLine("[INFO] Deleting old database...");
                File.Delete(dbPath);
            }

            CreateDatabase(dbPath);
            ExecuteScripts(dbPath, databaseDir);

            Console.WriteLine("[SUCCESS] Database created.");
        }
        catch (Exception ex)
        {
            Console.WriteLine("[ERROR] Failed to build database.");
            Console.WriteLine(ex);
        }
    }

    private static void CreateDatabase(string dbPath)
    {
        string cs = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            ForeignKeys = true
        }.ToString();

        using var connection = new SqliteConnection(cs);
        connection.Open();

        Console.WriteLine("[INFO] SQLite file created.");
    }

    private static void ExecuteScripts(string dbPath, string databaseDir)
    {
        string cs = new SqliteConnectionStringBuilder
        {
            DataSource = dbPath,
            ForeignKeys = true
        }.ToString();

        using var connection = new SqliteConnection(cs);
        connection.Open();

        foreach (string script in ScriptFiles)
        {
            string path = Path.Combine(databaseDir, script);

            Console.WriteLine($"[INFO] Running {script}");

            string sql = File.ReadAllText(path);

            using SqliteCommand cmd = connection.CreateCommand();
            cmd.CommandText = sql;
            _ = cmd.ExecuteNonQuery();
        }
    }

    private static string FindProjectRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);

        Console.WriteLine($"[DEBUG] Start search from: {dir.FullName}");

        while (dir != null)
        {
            string databasePath = Path.Combine(dir.FullName, "database");

            Console.WriteLine($"[DEBUG] Checking: {databasePath}");

            if (Directory.Exists(databasePath))
            {
                Console.WriteLine("[DEBUG] Found database folder!");
                return dir.FullName;
            }

            dir = dir.Parent;
        }

        throw new DirectoryNotFoundException("database folder not found");
    }
}