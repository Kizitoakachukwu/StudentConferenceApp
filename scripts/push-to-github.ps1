# Push StudentConferenceApp to https://github.com/Kizitoakachukwu/StudentConferenceApp
# Run once after: gh auth login

$ErrorActionPreference = "Stop"
Set-Location (Join-Path $PSScriptRoot "..")

Write-Host "Checking GitHub CLI login..." -ForegroundColor Cyan
gh auth status
if ($LASTEXITCODE -ne 0) {
    Write-Host "Run: gh auth login" -ForegroundColor Yellow
    exit 1
}

Write-Host "Creating repository (if needed) and pushing..." -ForegroundColor Cyan
gh repo create Kizitoakachukwu/StudentConferenceApp --public --source=. --remote=origin --push

Write-Host "Done. Open: https://github.com/Kizitoakachukwu/StudentConferenceApp" -ForegroundColor Green
