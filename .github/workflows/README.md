# GitHub Actions Workflows

## CI/CD Pipeline (`cicd.yml`)

This workflow automatically builds, tests, and deploys the ASP.NET backend application.

### Triggers

- **Push** to `main`, `master`, or `develop` branches
- **Pull Request** to `main`, `master`, or `develop` branches

### Jobs

1. **Build**
   - Runs on: All pushes and PRs
   - Actions: Restore, build, publish
   - Artifacts: Published app in `./publish`

2. **Test**
   - Runs on: All pushes and PRs
   - Depends on: Build job
   - Actions: Run all tests
   - Artifacts: Test results (.trx files)

3. **Deploy**
   - Runs on: Push to `main` or `master` only
   - Depends on: Build and Test jobs
   - Actions: Deploy to Ubuntu server via SSH
   - Prerequisites: GitHub Secrets must be configured

### Required GitHub Secrets

| Secret | Description |
|--------|-------------|
| `DEPLOY_SSH_KEY` | Private SSH key for server access |
| `DEPLOY_USER` | SSH username (e.g., www-data) |
| `DEPLOY_SERVER_IP` | Server IP address (e.g., 192.168.1.245) |
| `DEPLOY_PATH` | Deployment directory path (e.g., /opt/sai-deal-assistant-backend) |

### Configuration

To modify the workflow:
- Change .NET version: Edit `DOTNET_VERSION` environment variable
- Change build configuration: Edit `BUILD_CONFIGURATION` environment variable
- Change deployment branches: Edit the `if` condition in the deploy job

### Testing Locally

Before pushing, you can test the build locally:

```bash
dotnet restore
dotnet build --configuration Release
dotnet test --configuration Release
dotnet publish --configuration Release --output ./publish
```

### Monitoring

- View workflow runs in the "Actions" tab of your GitHub repository
- Check logs for each job to debug any issues
- Test results are uploaded as artifacts for review

### Notes

- The workflow will fail gracefully if there are no .NET projects yet
- Once you add ASP.NET projects, the workflow will automatically build and deploy them
- Deployment backups are created automatically before each deploy
