Write-Host ""
Write-Host "============================="
Write-Host " SportRent Database Rebuild"
Write-Host "============================="
Write-Host ""

$EfVersion = "9.0.4"
$DbRelativePath = "SportRent.Mobile\Resources\Raw\Database\sportRent.db"

$solutionRoot = Resolve-Path (Join-Path $PSScriptRoot "..")
$DbAbsolutePath = Join-Path $solutionRoot $DbRelativePath
Set-Location $solutionRoot

Write-Host "[INFO] Solution root: $solutionRoot"
Write-Host "[INFO] EF Core version: $EfVersion"
Write-Host "[INFO] SQLite DB path: $DbRelativePath"
Write-Host "[INFO] SQLite DB absolute path: $DbAbsolutePath"

function Fail($message)
{
    Write-Host "[ERROR] $message"
    exit 1
}

function Ensure-DotnetEfTool($version)
{
    Write-Host "[CHECK] dotnet-ef tool..."

    $toolLine = dotnet tool list -g | Select-String "dotnet-ef"

    if (-not $toolLine)
    {
        Write-Host "[INFO] Installing dotnet-ef $version ..."
        dotnet tool install --global dotnet-ef --version $version

        if ($LASTEXITCODE -ne 0) {
            Fail "Failed to install dotnet-ef $version"
        }
    }
    else
    {
        Write-Host "[INFO] dotnet-ef already installed."
    }
}

function Ensure-Package($project, $package, $version)
{
    $installed = dotnet list $project package | Select-String $package

    if (-not $installed)
    {
        Write-Host "[INFO] Installing $package $version into $project ..."
        dotnet add $project package $package --version $version

        if ($LASTEXITCODE -ne 0) {
            Fail "Failed to install package $package $version into $project"
        }
    }
    else
    {
        Write-Host "[INFO] $package already referenced in $project"
    }
}

Ensure-DotnetEfTool $EfVersion

Ensure-Package "SportRent.Data" "Microsoft.EntityFrameworkCore" $EfVersion
Ensure-Package "SportRent.Data" "Microsoft.EntityFrameworkCore.Sqlite" $EfVersion
Ensure-Package "SportRent.Data" "Microsoft.EntityFrameworkCore.Design" $EfVersion

Write-Host ""
Write-Host "[STEP] Rebuilding SQLite database..."
dotnet run --project SportRent.DbTool -- --root $solutionRoot --scripts-dir database --output $DbRelativePath

if ($LASTEXITCODE -ne 0) {
    Fail "Database build failed."
}

Write-Host ""
Write-Host "[STEP] Cleaning old scaffolded files..."
Remove-Item ".\SportRent.Data\Entities\*" -Recurse -Force -ErrorAction SilentlyContinue
Remove-Item ".\SportRent.Data\Context\*" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host ""
Write-Host "[STEP] Running EF Core scaffolding..."

dotnet ef dbcontext scaffold `
"Data Source=$DbAbsolutePath" `
Microsoft.EntityFrameworkCore.Sqlite `
--project SportRent.Data `
--startup-project SportRent.Data `
--context SportRentDbContext `
--output-dir Entities `
--context-dir Context `
--force `
--no-onconfiguring

if ($LASTEXITCODE -ne 0) {
    Fail "Scaffolding failed."
}

Write-Host ""
Write-Host "[SUCCESS] Database and entities rebuilt successfully."
