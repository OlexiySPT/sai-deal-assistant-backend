# Quick Setup Guide

This is a quick reference for setting up the CI/CD pipeline. For detailed instructions, see [DEPLOYMENT.md](DEPLOYMENT.md).

## Setup Steps

### 1. Configure GitHub Secrets

Go to your repository Settings → Secrets and variables → Actions, and add these secrets:

```
DEPLOY_SSH_KEY       = <contents of your private SSH key>
DEPLOY_USER          = www-data (or your deployment user)
DEPLOY_SERVER_IP     = 192.168.1.245
DEPLOY_PATH          = /opt/sai-deal-assistant-backend
```

### 2. Set Up Server

On your Ubuntu server (192.168.1.245):

```bash
# Copy setup script to server
scp deployment/setup-server.sh your-user@192.168.1.245:~/

# SSH to server and run setup
ssh your-user@192.168.1.245
chmod +x setup-server.sh
sudo ./setup-server.sh
```

### 3. Configure SSH Keys

On your local machine:

```bash
# Generate SSH key
ssh-keygen -t ed25519 -C "github-actions-deploy" -f ~/.ssh/github_deploy_key

# Copy public key to server
ssh-copy-id -i ~/.ssh/github_deploy_key.pub your-user@192.168.1.245

# Add private key to GitHub Secrets as DEPLOY_SSH_KEY
cat ~/.ssh/github_deploy_key
```

### 4. Test Deployment (Optional)

```bash
export DEPLOY_USER="www-data"
export DEPLOY_SERVER_IP="192.168.1.245"
export SSH_KEY="$HOME/.ssh/github_deploy_key"
./deployment/deploy-manual.sh
```

### 5. Push to GitHub

```bash
git push origin main
```

The pipeline will automatically build, test, and deploy your application!

## Common Commands

### Manage Service on Server

```bash
sudo systemctl start sai-deal-assistant-backend
sudo systemctl stop sai-deal-assistant-backend
sudo systemctl restart sai-deal-assistant-backend
sudo systemctl status sai-deal-assistant-backend
sudo journalctl -u sai-deal-assistant-backend -f
```

## What Gets Deployed

When you push to `main` or `master`:
1. ✅ Code is built
2. ✅ Tests are run
3. ✅ Application is deployed to 192.168.1.245
4. ✅ Previous version is backed up
5. ✅ Service is restarted

## Important Notes

- **Backups**: Previous deployments are saved to `/opt/sai-deal-assistant-backend/backup/`
- **Logs**: View logs with `sudo journalctl -u sai-deal-assistant-backend -f`
- **Service Name**: The service is named `sai-deal-assistant-backend`
- **Deployment Path**: Files are deployed to `/opt/sai-deal-assistant-backend/app/`

## Troubleshooting

**Pipeline fails?**
- Check GitHub Actions logs in your repository
- Verify all secrets are correctly configured

**Service won't start?**
- Check logs: `sudo journalctl -u sai-deal-assistant-backend -n 50`
- Verify .NET is installed: `dotnet --version`

**Need help?**
- See detailed documentation in [DEPLOYMENT.md](DEPLOYMENT.md)
