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

## Connect to GitHub

### 1. Sign in to GitHub CLI (once)

```bash
gh auth login
```

Choose **GitHub.com**, **HTTPS**, and sign in in the browser.

### 2. Create the remote repository and push

From the repository root:

```bash
git add .
git commit -m "Initial commit: StudentConferenceApp"
gh repo create StudentConferenceApp --source=. --public --push
```

Use `--private` instead of `--public` if you want a private repo.

### 3. Or link an existing empty repo on GitHub

```bash
git remote add origin https://github.com/YOUR_USERNAME/StudentConferenceApp.git
git branch -M main
git push -u origin main
```

Replace `YOUR_USERNAME` with your GitHub username.

## Configuration

Copy connection strings locally via `Presentation/appsettings.Development.json` (not required in repo). Default uses SQL Server LocalDB.
