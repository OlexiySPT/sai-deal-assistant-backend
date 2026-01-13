# Deployment Guide

This guide explains how to set up and use the CI/CD pipeline for deploying the Sai Deal Assistant Backend to your Ubuntu server.

## Prerequisites

- Ubuntu Server 20.04 or later (192.168.1.245)
- SSH access to the server
- GitHub repository with Actions enabled
- .NET 8.0 SDK (for development)

## Quick Start

### 1. Prepare Your Ubuntu Server

Copy the setup script to your server and run it:

```bash
# From your local machine
scp deployment/setup-server.sh your-user@192.168.1.245:~/
ssh your-user@192.168.1.245

# On the server
chmod +x setup-server.sh
sudo ./setup-server.sh
```

This will:
- Install .NET 8.0 Runtime
- Create deployment directories at `/opt/sai-deal-assistant-backend/`
- Configure systemd service
- Set up proper permissions

### 2. Set Up SSH Authentication

Generate an SSH key pair for GitHub Actions:

```bash
ssh-keygen -t ed25519 -C "github-actions-deploy" -f ~/.ssh/github_deploy_key
```

Copy the public key to your server:

```bash
ssh-copy-id -i ~/.ssh/github_deploy_key.pub your-user@192.168.1.245
```

### 3. Configure GitHub Secrets

In your GitHub repository, go to Settings → Secrets and variables → Actions, and add:

1. **DEPLOY_SSH_KEY**: Contents of `~/.ssh/github_deploy_key` (private key)
   ```bash
   cat ~/.ssh/github_deploy_key
   ```

2. **DEPLOY_USER**: The user for deployment (e.g., `www-data` or your user)

3. **DEPLOY_SERVER_IP**: `192.168.1.245`

4. **DEPLOY_PATH**: `/opt/sai-deal-assistant-backend`

### 4. Test the Setup

Before pushing to GitHub, test the deployment manually:

```bash
# Set environment variables
export DEPLOY_USER="www-data"
export DEPLOY_SERVER_IP="192.168.1.245"
export SSH_KEY="$HOME/.ssh/github_deploy_key"

# Run manual deployment
./deployment/deploy-manual.sh
```

### 5. Push to GitHub

Once everything is configured, simply push to your repository:

```bash
git add .
git commit -m "Your changes"
git push origin main
```

The CI/CD pipeline will automatically:
1. Build your application
2. Run all tests
3. Deploy to your Ubuntu server (if on main/master branch)

## Pipeline Details

### Build Job

- Restores NuGet packages
- Builds the application in Release configuration
- Publishes the application to `./publish` directory
- Uploads artifacts for the next jobs

### Test Job

- Runs all unit and integration tests
- Generates test result reports
- Uploads test results as artifacts

### Deploy Job

- Downloads build artifacts
- Connects to Ubuntu server via SSH
- Backs up current deployment
- Copies new files to server
- Restarts the systemd service

## Server Management

### Service Commands

```bash
# Start the service
sudo systemctl start sai-deal-assistant-backend

# Stop the service
sudo systemctl stop sai-deal-assistant-backend

# Restart the service
sudo systemctl restart sai-deal-assistant-backend

# Check status
sudo systemctl status sai-deal-assistant-backend

# View logs (follow mode)
sudo journalctl -u sai-deal-assistant-backend -f

# View last 100 lines of logs
sudo journalctl -u sai-deal-assistant-backend -n 100
```

### Rollback to Previous Version

If a deployment fails, you can rollback to a previous version:

```bash
# List available backups
ls -la /opt/sai-deal-assistant-backend/backup/

# Restore a specific backup (replace TIMESTAMP with actual value)
sudo systemctl stop sai-deal-assistant-backend
sudo rm -rf /opt/sai-deal-assistant-backend/app
sudo cp -r /opt/sai-deal-assistant-backend/backup/app-TIMESTAMP /opt/sai-deal-assistant-backend/app
sudo systemctl start sai-deal-assistant-backend
```

## Customization

### Change Deployment Path

1. Update `DEPLOY_PATH` secret in GitHub
2. Update `DEPLOY_PATH` variable in `deployment/setup-server.sh`
3. Re-run the setup script on the server

### Change Service Name

1. Edit `SERVICE_NAME` in `deployment/setup-server.sh`
2. Update service name in `.github/workflows/cicd.yml` (deploy job)
3. Re-run the setup script on the server

### Deploy to Multiple Environments

You can create separate workflows for different environments:

1. Copy `.github/workflows/cicd.yml` to `.github/workflows/cicd-staging.yml`
2. Update the branch triggers and secret names
3. Create separate secrets for each environment (e.g., `STAGING_DEPLOY_USER`, `PROD_DEPLOY_USER`)

### Configure Environment Variables

Add environment variables to the systemd service file:

```bash
sudo nano /etc/systemd/system/sai-deal-assistant-backend.service
```

Add additional `Environment=` lines under the `[Service]` section:

```ini
[Service]
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ConnectionStrings__DefaultConnection=your-connection-string
Environment=JWT_SECRET=your-jwt-secret
```

Reload and restart:

```bash
sudo systemctl daemon-reload
sudo systemctl restart sai-deal-assistant-backend
```

## Troubleshooting

### Deployment Fails

**SSH Connection Error:**
- Verify SSH key is correctly added to GitHub Secrets
- Check public key is in `~/.ssh/authorized_keys` on the server
- Test SSH connection manually: `ssh -i ~/.ssh/github_deploy_key user@192.168.1.245`

**Permission Denied:**
- Ensure deployment user has write access to `DEPLOY_PATH`
- Check ownership: `ls -la /opt/sai-deal-assistant-backend/`
- Fix permissions: `sudo chown -R www-data:www-data /opt/sai-deal-assistant-backend/`

### Service Won't Start

**Check Logs:**
```bash
sudo journalctl -u sai-deal-assistant-backend -n 50 --no-pager
```

**Common Issues:**
- Missing .NET runtime: `dotnet --version`
- Wrong DLL path in service file
- Port already in use
- Missing environment variables

### Tests Fail

**Run Tests Locally:**
```bash
dotnet test --verbosity detailed
```

**Check Dependencies:**
```bash
dotnet restore
dotnet build
```

## Security Best Practices

1. **SSH Keys**: Never commit SSH private keys to the repository
2. **Secrets**: Store all sensitive data in GitHub Secrets
3. **Firewall**: Configure firewall on Ubuntu server to allow only necessary ports
4. **User Permissions**: Use a dedicated user for deployment (not root)
5. **HTTPS**: Configure reverse proxy (nginx/Apache) with SSL certificate
6. **Backup**: Regularly backup your deployment and database

## Monitoring

### Set Up Log Rotation

Create log rotation config:

```bash
sudo nano /etc/systemd/journald.conf
```

Set limits:
```ini
[Journal]
SystemMaxUse=500M
SystemMaxFileSize=50M
```

Restart journald:
```bash
sudo systemctl restart systemd-journald
```

### Monitor Service Health

Create a simple health check script:

```bash
#!/bin/bash
if ! systemctl is-active --quiet sai-deal-assistant-backend; then
    echo "Service is down! Restarting..."
    sudo systemctl restart sai-deal-assistant-backend
fi
```

Add to crontab to run every 5 minutes:
```bash
*/5 * * * * /path/to/health-check.sh
```

## Additional Resources

- [ASP.NET Core Deployment Documentation](https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/)
- [GitHub Actions Documentation](https://docs.github.com/en/actions)
- [systemd Service Management](https://www.freedesktop.org/software/systemd/man/systemctl.html)
