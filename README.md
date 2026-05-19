# StudentConferenceApp

ASP.NET Core application for student conference registration (participants, supervisors, admin, document generation).

## Main project

Run the web app from **`Presentation`**:

```bash
cd Presentation
dotnet ef database update --project ../DataAccess --startup-project .
dotnet run
```

Default URLs: `https://localhost:7098` and `http://localhost:5015` (see `Properties/launchSettings.json`).

## GitHub

This project is configured for:

**https://github.com/Kizitoakachukwu/StudentConferenceApp**

The local repo already has an initial commit on branch `main` and `origin` set to that URL.

### Publish to your account (one-time)

1. Sign in to GitHub CLI:

```powershell
gh auth login
```

2. Create the repo and push:

```powershell
cd c:\Users\kizit\source\repos\StudentConferenceApp
.\scripts\push-to-github.ps1
```

Or manually:

```powershell
gh repo create Kizitoakachukwu/StudentConferenceApp --public --source=. --remote=origin --push
```

Use `--private` instead of `--public` for a private repository.

### Alternative (browser)

1. Open [Create repository](https://github.com/new?name=StudentConferenceApp) (signed in as **Kizitoakachukwu**).
2. Name: `StudentConferenceApp`, leave it empty (no README).
3. Run:

```powershell
git push -u origin main
```

## Configuration

Copy connection strings locally via `Presentation/appsettings.Development.json` (not required in repo). Default uses SQL Server LocalDB.
