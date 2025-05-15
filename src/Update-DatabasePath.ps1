# Update-DatabasePath.ps1
# Script to update DatabasePath in .env files to use absolute paths

# Get the current directory (src folder)
$srcDir = Get-Location

# Navigate up one level to the repository root
$repoRoot = Join-Path -Path $srcDir -ChildPath ".." | Resolve-Path

# Calculate the absolute path to the database file
$dbPath = Join-Path -Path $repoRoot -ChildPath "_db\EventRegistration.db"
$dbPath = [System.IO.Path]::GetFullPath($dbPath)

Write-Host "Using database path: $dbPath" -ForegroundColor Cyan

# Find all .env files recursively
$envFiles = Get-ChildItem -Path $srcDir -Filter ".env" -Recurse -File

Write-Host "Found $($envFiles.Count) .env files to process" -ForegroundColor Cyan

foreach ($file in $envFiles) {
    Write-Host "Processing $($file.FullName)" -ForegroundColor Yellow
    
    try {
        # Read the current content
        $content = Get-Content -Path $file.FullName -Raw
        
        # Check if the file contains DatabasePath
        if ($content -match 'DatabasePath=') {
            # Create the new content with updated DatabasePath
            $updatedContent = $content -replace 'DatabasePath=.*', "DatabasePath=$dbPath"
            
            # Write the updated content back to the file
            $updatedContent | Set-Content -Path $file.FullName -NoNewline
            
            Write-Host "  Updated DatabasePath in $($file.Name) to: $dbPath" -ForegroundColor Green
        } else {
            Write-Host "  No DatabasePath found in $($file.Name)" -ForegroundColor Gray
        }
    } catch {
        Write-Host "  Error processing $($file.Name): $_" -ForegroundColor Red
    }
}

Write-Host "Database path update complete!" -ForegroundColor Cyan
