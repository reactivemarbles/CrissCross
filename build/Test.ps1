#requires -Version 7.0

param(
    [string]$Configuration = 'Release',
    [string]$ResultsDirectory = 'TestResults'
)

$ErrorActionPreference = 'Stop'
$sourceDirectory = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '../src'))
$resultsPath = [System.IO.Path]::GetFullPath($ResultsDirectory, $sourceDirectory)
$testProjects = @(Get-ChildItem -LiteralPath (Join-Path $sourceDirectory 'Tests') -Recurse -File -Filter '*.Tests.csproj' | Sort-Object FullName)
if ($testProjects.Count -eq 0) {
    throw 'No test projects were found under src/Tests.'
}

Push-Location $sourceDirectory
try {
    foreach ($testProject in $testProjects) {
        Write-Host "Running $($testProject.BaseName)"
        dotnet test --project $testProject.FullName `
            --configuration $Configuration `
            --max-parallel-test-modules 1 `
            --minimum-expected-tests 1 `
            --coverage `
            --coverage-output-format cobertura `
            --results-directory (Join-Path $resultsPath $testProject.BaseName)
        if ($LASTEXITCODE -ne 0) {
            throw "Tests failed for $($testProject.BaseName) with exit code $LASTEXITCODE."
        }
    }
}
finally {
    Pop-Location
}
