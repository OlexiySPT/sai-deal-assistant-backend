# ✅ CI/CD Pipeline Setup Complete

## What Was Done

A complete CI/CD pipeline has been created for the Sai Deal Assistant Backend repository. The pipeline automatically builds, tests, and deploys your ASP.NET WebAPI application to your Ubuntu server (192.168.1.245) whenever you push changes to the repository.

## Files Created

### GitHub Actions Workflow
```
.github/workflows/
├── cicd.yml          - Main CI/CD pipeline configuration
└── README.md         - Workflow documentation
```

### Deployment Infrastructure
```
deployment/
├── setup-server.sh                      - Ubuntu server preparation script
├── deploy-manual.sh                     - Manual deployment testing script
├── systemd-service-template.service     - systemd service configuration
├── DEPLOYMENT.md                        - Complete deployment guide
└── QUICKSTART.md                        - Quick setup reference
```

### Documentation
```
├── README.md                - Updated with CI/CD overview
├── PIPELINE_OVERVIEW.md     - Complete pipeline documentation
├── .gitignore               - Updated for .NET artifacts
└── .gitattributes           - Line ending configuration
```

## What The Pipeline Does

When you push code to `main` or `master`:

1. **Build Job**
   - Checks out your code
   - Sets up .NET 8.0
   - Restores dependencies
   - Builds the application
   - Publishes deployment artifacts

2. **Test Job**
   - Runs all unit and integration tests
   - Generates test reports
   - Fails if any tests fail

3. **Deploy Job** (only if tests pass)
   - Downloads build artifacts
   - Connects to Ubuntu server via SSH
   - Backs up current deployment
   - Copies new files to server
   - Restarts the application service
   - Verifies service is running

## Your Next Steps

### 1. Prepare Your Ubuntu Server

```bash
# Copy the setup script to your server
scp deployment/setup-server.sh user@192.168.1.245:~/

# SSH to your server
ssh user@192.168.1.245

# Run the setup script
chmod +x setup-server.sh
sudo ./setup-server.sh
```

This will:
- Install .NET 8.0 Runtime
- Create deployment directories at `/opt/sai-deal-assistant-backend/`
- Set up the systemd service
- Configure proper permissions

### 2. Generate SSH Keys for Deployment

```bash
# On your local machine
ssh-keygen -t ed25519 -C "github-actions-deploy" -f ~/.ssh/github_deploy_key

# Copy the public key to your server
ssh-copy-id -i ~/.ssh/github_deploy_key.pub user@192.168.1.245
```

### 3. Configure GitHub Secrets

Go to: Repository Settings → Secrets and variables → Actions

Add these 4 secrets:

| Secret Name | Value |
|-------------|-------|
| `DEPLOY_SSH_KEY` | Contents of `~/.ssh/github_deploy_key` (private key) |
| `DEPLOY_USER` | `www-data` (or your deployment user) |
| `DEPLOY_SERVER_IP` | `192.168.1.245` |
| `DEPLOY_PATH` | `/opt/sai-deal-assistant-backend` |

To get the private key:
```bash
cat ~/.ssh/github_deploy_key
```

### 4. Update Service Configuration (When You Add Your Code)

Once you add your ASP.NET application code to this repository:

1. On your Ubuntu server, edit the service file:
   ```bash
   sudo nano /etc/systemd/system/sai-deal-assistant-backend.service
   ```

2. Update the `ExecStart` line with your actual DLL filename:
   ```ini
   ExecStart=/usr/bin/dotnet /opt/sai-deal-assistant-backend/app/YourActualApp.dll
   ```

3. Reload and restart:
   ```bash
   sudo systemctl daemon-reload
   sudo systemctl restart sai-deal-assistant-backend
   ```

### 5. Test Deployment (Optional)

Test the deployment manually before pushing to GitHub:

```bash
export DEPLOY_USER="www-data"
export DEPLOY_SERVER_IP="192.168.1.245"
export SSH_KEY="$HOME/.ssh/github_deploy_key"

./deployment/deploy-manual.sh
```

### 6. Push to GitHub and Deploy!

Once everything is configured:

```bash
git add .
git commit -m "Add my ASP.NET application"
git push origin main
```

The CI/CD pipeline will automatically:
- ✅ Build your application
- ✅ Run your tests
- ✅ Deploy to your server
- ✅ Restart the service

## Monitoring Your Deployments

### View Pipeline Status
- Go to the "Actions" tab in your GitHub repository
- Click on the latest workflow run to see detailed logs

### Monitor Your Application on the Server

```bash
# Check if service is running
sudo systemctl status sai-deal-assistant-backend

# View live logs
sudo journalctl -u sai-deal-assistant-backend -f

# View recent logs
sudo journalctl -u sai-deal-assistant-backend -n 100

# View only errors
sudo journalctl -u sai-deal-assistant-backend -p err
```

## Managing the Service

```bash
# Start the service
sudo systemctl start sai-deal-assistant-backend

# Stop the service
sudo systemctl stop sai-deal-assistant-backend

# Restart the service
sudo systemctl restart sai-deal-assistant-backend

# Enable auto-start on boot
sudo systemctl enable sai-deal-assistant-backend

# Disable auto-start
sudo systemctl disable sai-deal-assistant-backend
```

## Rollback to Previous Version

If a deployment breaks something, you can quickly rollback:

```bash
# List available backups
ls -la /opt/sai-deal-assistant-backend/backup/

# Stop the service
sudo systemctl stop sai-deal-assistant-backend

# Restore a backup (replace YYYYMMDD-HHMMSS with actual timestamp)
sudo rm -rf /opt/sai-deal-assistant-backend/app
sudo cp -r /opt/sai-deal-assistant-backend/backup/app-YYYYMMDD-HHMMSS /opt/sai-deal-assistant-backend/app

# Start the service
sudo systemctl start sai-deal-assistant-backend
```

## Customization Options

### Change .NET Version

Edit `.github/workflows/cicd.yml`:
```yaml
env:
  DOTNET_VERSION: '8.0.x'  # Change to your desired version
```

### Add Environment Variables

Edit the systemd service on your server:
```bash
sudo nano /etc/systemd/system/sai-deal-assistant-backend.service
```

Add under `[Service]`:
```ini
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=ConnectionStrings__DefaultConnection=your-connection-string
Environment=JWT_SECRET=your-secret
```

Then reload:
```bash
sudo systemctl daemon-reload
sudo systemctl restart sai-deal-assistant-backend
```

### Deploy to Different Branches

Edit `.github/workflows/cicd.yml` in the deploy job:
```yaml
if: github.event_name == 'push' && (github.ref == 'refs/heads/main' || github.ref == 'refs/heads/staging')
```

## Security Best Practices

✅ **Implemented:**
- SSH key-based authentication (no passwords)
- Secrets stored securely in GitHub
- Automatic backups before deployment
- Service runs as non-root user
- Sensitive files excluded from repository

🔐 **Recommended:**
- Set up a firewall on your Ubuntu server
- Configure HTTPS with a reverse proxy (nginx/Apache)
- Regularly rotate SSH keys
- Enable two-factor authentication on GitHub
- Keep .NET runtime updated

## Documentation

For more detailed information, see:

- **[QUICKSTART.md](deployment/QUICKSTART.md)** - Quick setup reference
- **[DEPLOYMENT.md](deployment/DEPLOYMENT.md)** - Comprehensive guide
- **[PIPELINE_OVERVIEW.md](PIPELINE_OVERVIEW.md)** - Pipeline details
- **[.github/workflows/README.md](.github/workflows/README.md)** - Workflow docs
- **[README.md](README.md)** - Main repository README

## Troubleshooting

### Pipeline fails at build
- Ensure you have .NET project files in the repository
- Check .NET version matches your project requirements

### Pipeline fails at deploy
- Verify all GitHub secrets are set correctly
- Test SSH connection manually
- Check server logs

### Service won't start
```bash
sudo journalctl -u sai-deal-assistant-backend -n 50
dotnet --version
ls -la /opt/sai-deal-assistant-backend/app/
```

## Support

If you encounter issues:
1. Check the detailed documentation in the files listed above
2. Review GitHub Actions logs in the Actions tab
3. Check service logs on the Ubuntu server
4. Verify all secrets and configurations are correct

---

## Summary

✅ CI/CD pipeline is fully configured and ready to use
✅ Complete documentation provided
✅ Security best practices implemented
✅ Automated testing and deployment
✅ Rollback capability included

**The pipeline will activate as soon as you add your ASP.NET application code and push to GitHub!**
