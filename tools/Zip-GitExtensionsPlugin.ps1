param([string] $Version, [string] $Configuration = 'Release')

If (-not($Version)) 
{ 
    Throw "Parameter -Version is required";
}

Push-Location $PSScriptRoot;

$sourceBasePath = "..\src\GitExtensions.BundleBackuper\bin\" + $Configuration;

$name = "GitExtensions.BundleBackuper-" + $Version;
$target = $sourceBasePath + "\GitExtensions.BundleBackuper." + $Version + ".zip";

$tempPath = Join-Path $env:TEMP -ChildPath $name;
New-Item -Force -ItemType Directory $tempPath | Out-Null;

Copy-Item -Force ($sourceBasePath + "\net461\GitExtensions.BundleBackuper.dll") $tempPath | Out-Null;

Compress-Archive -Path ($tempPath + "\*") -DestinationPath $target -Force;
Write-Host ("Created release zip at '" + $target + "'");

Remove-Item -Force -Recurse $tempPath;

Pop-Location;