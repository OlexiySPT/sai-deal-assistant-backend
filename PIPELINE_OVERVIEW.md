# CI/CD Pipeline Overview

This repository now includes a complete CI/CD pipeline that automatically builds, tests, and deploys your ASP.NET WebAPI backend to your Ubuntu server.

## What Was Created

### 1. GitHub Actions Workflow (`.github/workflows/cicd.yml`)

A comprehensive CI/CD workflow with three jobs:

- **Build Job**: Compiles the ASP.NET application and creates deployment artifacts
- **Test Job**: Runs all unit and integration tests
- **Deploy Job**: Deploys to Ubuntu server at 192.168.1.245 (only on main/master branches)

### 2. Deployment Scripts (`deployment/`)

- **setup-server.sh**: Prepares your Ubuntu server for deployments
  - Installs .NET 8.0 Runtime
  - Creates deployment directories
  - Configures systemd service
  - Sets up proper permissions

- **deploy-manual.sh**: Manual deployment script for testing
  - Useful for testing before pushing to GitHub
  - Performs the same steps as the CI/CD pipeline

- **systemd-service-template.service**: Template for the systemd service
  - Configures the application as a system service
  - Enables automatic restart on failure

### 3. Documentation

- **README.md**: Updated with CI/CD overview and setup instructions
- **DEPLOYMENT.md**: Comprehensive deployment guide
- **QUICKSTART.md**: Quick reference for setup

### 4. Configuration Files

- **.gitattributes**: Ensures proper line endings across platforms
- **.gitignore**: Updated to exclude build artifacts and sensitive files

## How It Works

### Automatic Deployment Flow

```
Push to GitHub (main/master)
    ↓
Build Job
    ├── Checkout code
    ├── Setup .NET
    ├── Restore dependencies
    ├── Build application
    └── Publish & upload artifacts
    ↓
Test Job
    ├── Checkout code
    ├── Setup .NET
    ├── Restore dependencies
    └── Run all tests
    ↓
Deploy Job (if tests pass)
    ├── Download build artifacts
    ├── Setup SSH connection
    ├── Backup current deployment
    ├── Copy files to server
    └── Restart service
```

### Security Features

- SSH key-based authentication (no passwords)
- Secrets stored in GitHub (never in code)
- Automatic backup before each deployment
- Service runs as non-root user
- Sensitive files excluded from repository

### Rollback Capability

Previous deployments are automatically backed up to:
```
/opt/sai-deal-assistant-backend/backup/app-YYYYMMDD-HHMMSS/
```

You can easily rollback if needed.

## Quick Start

### For First-Time Setup:

1. Set up your Ubuntu server:
   ```bash
   scp deployment/setup-server.sh user@192.168.1.245:~/
   ssh user@192.168.1.245 'chmod +x setup-server.sh && sudo ./setup-server.sh'
   ```

2. Generate and configure SSH keys:
   ```bash
   ssh-keygen -t ed25519 -f ~/.ssh/github_deploy_key
   ssh-copy-id -i ~/.ssh/github_deploy_key.pub user@192.168.1.245
   ```

3. Add secrets to GitHub:
   - `DEPLOY_SSH_KEY`: Private key contents
   - `DEPLOY_USER`: Deployment user (e.g., www-data)
   - `DEPLOY_SERVER_IP`: 192.168.1.245
   - `DEPLOY_PATH`: /opt/sai-deal-assistant-backend

4. Push to GitHub and watch it deploy!

### For Daily Development:

Just push your changes:
```bash
git add .
git commit -m "Your changes"
git push origin main
```

The pipeline handles everything else automatically.

## Customization

### Change .NET Version

Edit `.github/workflows/cicd.yml`:
```yaml
env:
  DOTNET_VERSION: '8.0.x'  # Change to desired version
```

### Change Deployment Branches

Edit `.github/workflows/cicd.yml` in the deploy job:
```yaml
if: github.event_name == 'push' && (github.ref == 'refs/heads/main' || github.ref == 'refs/heads/your-branch')
```

### Add Environment Variables

Edit the systemd service on your server:
```bash
sudo nano /etc/systemd/system/sai-deal-assistant-backend.service
```

Add under `[Service]`:
```ini
Environment=YOUR_VAR=value
```

Then reload:
```bash
sudo systemctl daemon-reload
sudo systemctl restart sai-deal-assistant-backend
```

## Monitoring

### View Deployment Status

Check GitHub Actions tab in your repository to see:
- Build status
- Test results
- Deployment logs

### Monitor Service on Server

```bash
# Check if service is running
sudo systemctl status sai-deal-assistant-backend

# View live logs
sudo journalctl -u sai-deal-assistant-backend -f

# View recent errors
sudo journalctl -u sai-deal-assistant-backend -p err -n 50
```

## Troubleshooting

### Pipeline Fails at Build

- Verify your .NET project files are in the repository
- Check .NET version matches your project requirements

### Pipeline Fails at Deploy

- Verify all GitHub secrets are set correctly
- Test SSH connection manually
- Check server logs

### Service Won't Start

```bash
# Check detailed logs
sudo journalctl -u sai-deal-assistant-backend -n 100 --no-pager

# Verify .NET runtime
dotnet --version

# Check file permissions
ls -la /opt/sai-deal-assistant-backend/app/
```

## Support

For detailed documentation, see:
- [DEPLOYMENT.md](deployment/DEPLOYMENT.md) - Complete deployment guide
- [QUICKSTART.md](deployment/QUICKSTART.md) - Quick setup reference

## Next Steps

When you add your ASP.NET code to this repository:

1. Update the service file with your actual DLL name
2. Configure any environment variables needed
3. Add tests to verify functionality
4. Push to GitHub and let the pipeline deploy!

The pipeline is ready to use as soon as you add your application code.
