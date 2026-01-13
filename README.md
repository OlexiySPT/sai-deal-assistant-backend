# Sai.DealAssistant-backend
ASP.NET WebAPI backend app for Sai.DealAssistant

## CI/CD Pipeline

This repository includes a complete CI/CD pipeline that automatically builds, tests, and deploys the backend application to your Ubuntu server.

### Pipeline Overview

The CI/CD pipeline is triggered on every push to `main`, `master`, or `develop` branches and runs the following jobs:

1. **Build** - Compiles the ASP.NET application and creates deployment artifacts
2. **Test** - Runs all unit tests and integration tests
3. **Deploy** - Deploys the application to the Ubuntu server (only on `main`/`master` branches)

### Setup Instructions

#### 1. Server Setup (Ubuntu 192.168.1.245)

Run the following commands on your Ubuntu server to prepare it for deployments:

```bash
# Copy the setup script to your server
scp deployment/setup-server.sh user@192.168.1.245:~/

# SSH into your server
ssh user@192.168.1.245

# Make the script executable and run it
chmod +x setup-server.sh
sudo ./setup-server.sh
```

The setup script will:
- Install .NET 8.0 Runtime
- Create deployment directories
- Set up a systemd service for the application
- Configure proper permissions

#### 2. GitHub Secrets Configuration

Configure the following secrets in your GitHub repository settings (Settings → Secrets and variables → Actions):

| Secret Name | Description | Example Value |
|-------------|-------------|---------------|
| `DEPLOY_SSH_KEY` | Private SSH key for deployment authentication | Contents of your private key file |
| `DEPLOY_USER` | Username for SSH connection | `www-data` or your deployment user |
| `DEPLOY_SERVER_IP` | IP address of your Ubuntu server | `192.168.1.245` |
| `DEPLOY_PATH` | Path where the app will be deployed | `/opt/sai-deal-assistant-backend` |

##### Generating SSH Key for Deployment

On your local machine or CI server:

```bash
# Generate SSH key pair
ssh-keygen -t ed25519 -C "github-actions-deploy" -f ~/.ssh/github_deploy_key

# Copy the public key to your Ubuntu server
ssh-copy-id -i ~/.ssh/github_deploy_key.pub user@192.168.1.245

# Copy the private key content and add it to GitHub Secrets as DEPLOY_SSH_KEY
cat ~/.ssh/github_deploy_key
```

#### 3. Managing the Service on Ubuntu Server

After deployment, manage the service using systemd:

```bash
# Start the service
sudo systemctl start sai-deal-assistant-backend

# Stop the service
sudo systemctl stop sai-deal-assistant-backend

# Restart the service
sudo systemctl restart sai-deal-assistant-backend

# Check service status
sudo systemctl status sai-deal-assistant-backend

# View logs
sudo journalctl -u sai-deal-assistant-backend -f
```

### Pipeline Workflow

1. **On Push**: When you push changes to the repository
2. **Build**: The application is built and compiled
3. **Test**: All tests are executed
4. **Deploy**: If tests pass and the push is to `main`/`master`, the application is automatically deployed to the Ubuntu server
5. **Rollback**: Previous deployments are backed up in `/opt/sai-deal-assistant-backend/backup/`

### Customization

- **Change .NET version**: Update `DOTNET_VERSION` in `.github/workflows/cicd.yml`
- **Modify deployment path**: Update the `DEPLOY_PATH` secret in GitHub
- **Adjust service name**: Update `SERVICE_NAME` in `deployment/setup-server.sh`
- **Change deployment branches**: Edit the `deploy` job conditions in `.github/workflows/cicd.yml`

### Troubleshooting

**Deployment fails with SSH connection error:**
- Verify the SSH key is correctly added to GitHub Secrets
- Ensure the public key is in `~/.ssh/authorized_keys` on the server
- Check firewall rules on the Ubuntu server allow SSH connections

**Service fails to start:**
- Check logs: `sudo journalctl -u sai-deal-assistant-backend -n 50`
- Verify .NET runtime is installed: `dotnet --version`
- Ensure the DLL name in the service file matches your actual application DLL

**Tests fail in CI:**
- Run tests locally: `dotnet test`
- Check test output in GitHub Actions for detailed error messages
