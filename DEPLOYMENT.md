# Dev Branch CI/CD Pipeline

## Overview

This CI/CD pipeline automatically builds, tests, and deploys the SAI Deal Assistant Backend to the development server when a Pull Request is merged into the `dev` branch.

## Workflow Trigger

The workflow is triggered when:
- A Pull Request targeting the `dev` branch is **merged** (not just closed)

## Pipeline Steps

1. **Checkout Code**: Pulls the latest code from the `dev` branch
2. **Setup .NET**: Configures .NET 8.0 SDK
3. **Restore Dependencies**: Restores NuGet packages
4. **Build**: Builds the solution in Release configuration
5. **Run Tests**: Executes all unit tests
6. **Build Docker Image**: Creates a Docker image tagged as `sai-deal-assistant:dev`
7. **Deploy to Dev Server**: 
   - Copies the Docker image to the dev server
   - Executes deployment script on the server
   - Starts the container with Production configuration

## Configuration

### GitHub Secrets

The following secret must be configured in the GitHub repository:

- `DEV_SERVER_PASSWORD`: SSH password for the dev server

To add this secret:
1. Go to repository Settings → Secrets and variables → Actions
2. Click "New repository secret"
3. Name: `DEV_SERVER_PASSWORD`
4. Value: (the SSH password provided by the server administrator)

### Dev Server Details

- **Host**: 192.168.1.245
- **SSH Port**: 2222
- **SSH User**: sai
- **Application Port**: 5000
- **Container Name**: sai-deal-assistant-dev

### Database Configuration

The application uses the following connection strings (configured in `appsettings.Production.json`):

```json
{
  "ConnectionStrings": {
    "AppConnection": "Host=192.168.1.245;Database=dev_dealassistantdatabase;Username=dealassiatantuser;Password=Kabanah5%",
    "MigrationConnection": "Host=192.168.1.245;Database=dev_dealassistantdatabase;Username=dealassiatant_migrator;Password=Kabanah5%"
  }
}
```

## Deployment Process

When the deployment script runs on the dev server, it:

1. Stops and removes the existing container (if any)
2. Removes the old Docker image
3. Loads the new Docker image from the transferred file
4. Starts a new container with:
   - Name: `sai-deal-assistant-dev`
   - Port mapping: 5000:8080
   - Environment: Production
   - Restart policy: unless-stopped

## Accessing the Application

After successful deployment, the application is available at:
- **URL**: http://192.168.1.245:5000

## Monitoring

To check the deployment status on the dev server:

```bash
# SSH into the dev server
ssh -p2222 sai@192.168.1.245

# Check container status
docker ps | grep sai-deal-assistant-dev

# View container logs
docker logs sai-deal-assistant-dev

# Follow logs in real-time
docker logs -f sai-deal-assistant-dev
```

## Troubleshooting

### Workflow Fails to Connect to Dev Server

1. Verify the `DEV_SERVER_PASSWORD` secret is set correctly
2. Ensure the dev server is accessible from GitHub Actions runners
3. Check that SSH port 2222 is open on the dev server

### Deployment Script Fails

1. SSH into the dev server
2. Check if Docker is running: `sudo systemctl status docker`
3. Verify there's enough disk space: `df -h`
4. Check Docker logs: `docker logs sai-deal-assistant-dev`

### Application Not Accessible

1. Verify the container is running: `docker ps | grep sai-deal-assistant-dev`
2. Check if port 5000 is accessible: `curl http://localhost:5000`
3. Review application logs: `docker logs sai-deal-assistant-dev`
4. Ensure the database server is accessible from the dev server

## Files

- `.github/workflows/deploy-dev.yml`: GitHub Actions workflow definition
- `deploy-dev.sh`: Deployment script executed on the dev server
- `src/Sai.DealAssistant.WebApi/appsettings.Production.json`: Production configuration with dev database settings
