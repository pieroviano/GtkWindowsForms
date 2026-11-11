<#
.SYNOPSIS
  Force-refresh this repo's NuGet packages into every consumer.

.DESCRIPTION
  The packages carry a date-based version that stays identical across a day's builds, so NuGet never
  re-extracts them and consumers keep running stale bits. Worse, the .NET build server (VBCSCompiler /
  persistent MSBuild nodes) keeps the cached gtk DLLs LOADED and LOCKED between builds, so the cache
  cannot be overwritten or evicted at all (`dotnet restore` then fails with "Access ... denied").

  This script breaks that deadlock:
    1. Shuts down the build servers       -> releases the locks on the cached DLLs.
    2. Deletes the gtk.* cache folders     -> forces a clean re-extract.
    3. (optional) rebuilds the package sln -> regenerates the packages with the current code.

  Run it whenever a consumer still shows old behaviour after you rebuilt a package.

.PARAMETER Rebuild
  Also rebuild Gtk.Windows.Forms.sln after clearing the cache.

.PARAMETER Consumer
  Path to a consumer .csproj/.sln to build last (e.g. the VbNetToCSharp UI), so it re-extracts the
  fresh package in the same run.

.EXAMPLE
  ./refresh-gtk-package.ps1 -Rebuild -Consumer C:\Temp\VbToCSharp\VbNetToCSharp.UI\VbNetToCSharp.UI.csproj
#>
[CmdletBinding()]
param(
    [switch]$Rebuild,
    [string]$Consumer
)

$ErrorActionPreference = 'Stop'
$repo = $PSScriptRoot

function Step($msg) { Write-Host "==> $msg" -ForegroundColor Cyan }

# 1) Release the locks the persistent build servers hold on the cached assemblies.
Step 'Shutting down .NET build servers (releases cached-DLL locks)'
dotnet build-server shutdown | Out-Null
Get-Process VBCSCompiler -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

# 2) Evict this repo's packages from the global NuGet cache so consumers re-extract.
Step 'Clearing gtk.* from the NuGet cache'
$cacheRoot = Join-Path $env:USERPROFILE '.nuget\packages'
$evicted = @()
Get-ChildItem $cacheRoot -Directory -ErrorAction SilentlyContinue |
    Where-Object { $_.Name -match '^gtk\.(windows|drawing|resources)' } |
    ForEach-Object {
        Remove-Item -Recurse -Force $_.FullName -ErrorAction SilentlyContinue
        if (-not (Test-Path $_.FullName)) { $evicted += $_.Name }
    }
if ($evicted) { $evicted | ForEach-Object { Write-Host "    evicted $_" } }
else { Write-Host '    (nothing cached)' }

# 3) Optionally regenerate the packages.
if ($Rebuild) {
    Step 'Rebuilding Gtk.Windows.Forms.sln'
    dotnet build (Join-Path $repo 'Gtk.Windows.Forms.sln') -c Debug --no-incremental
    if ($LASTEXITCODE -ne 0) { throw "Package build failed ($LASTEXITCODE)." }
}

# 4) Optionally build a consumer so it pulls the fresh package now.
if ($Consumer) {
    if (-not (Test-Path $Consumer)) { throw "Consumer not found: $Consumer" }
    Step "Building consumer $Consumer"
    dotnet build $Consumer -c Debug
    if ($LASTEXITCODE -ne 0) { throw "Consumer build failed ($LASTEXITCODE)." }
}

Step 'Done. If a consumer app is running under the debugger, STOP and restart it so the new DLL loads.'
