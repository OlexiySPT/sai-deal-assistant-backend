#!/bin/bash

# Deployment setup script for Ubuntu Server
# This script should be run on the Ubuntu server (192.168.1.245) to prepare it for deployments

set -e

echo "=== Sai Deal Assistant Backend - Deployment Setup ==="

# Variables - Update these as needed
DEPLOY_USER=${DEPLOY_USER:-"www-data"}
DEPLOY_PATH=${DEPLOY_PATH:-"/opt/sai-deal-assistant-backend"}
SERVICE_NAME="sai-deal-assistant-backend"
APP_DLL_NAME=${APP_DLL_NAME:-"YourApp.dll"}  # Update this to match your actual DLL name

# Check if running with sudo
if [ "$EUID" -ne 0 ]; then
  echo "Please run with sudo"
  exit 1
fi

echo "Installing .NET 8.0 Runtime..."
# Add Microsoft package repository
wget https://packages.microsoft.com/config/ubuntu/$(lsb_release -rs)/packages-microsoft-prod.deb -O packages-microsoft-prod.deb
dpkg -i packages-microsoft-prod.deb
rm packages-microsoft-prod.deb

# Update package list and install .NET runtime
apt-get update
apt-get install -y aspnetcore-runtime-8.0

echo "Creating deployment directory structure..."
mkdir -p ${DEPLOY_PATH}/app
mkdir -p ${DEPLOY_PATH}/backup

echo "Setting up permissions..."
chown -R ${DEPLOY_USER}:${DEPLOY_USER} ${DEPLOY_PATH}
chmod -R 755 ${DEPLOY_PATH}

echo "Creating systemd service..."
cat > /etc/systemd/system/${SERVICE_NAME}.service << EOF
[Unit]
Description=Sai Deal Assistant Backend API
After=network.target

[Service]
Type=notify
WorkingDirectory=${DEPLOY_PATH}/app
ExecStart=/usr/bin/dotnet ${DEPLOY_PATH}/app/${APP_DLL_NAME}
Restart=always
RestartSec=10
KillSignal=SIGINT
SyslogIdentifier=${SERVICE_NAME}
User=${DEPLOY_USER}
Environment=ASPNETCORE_ENVIRONMENT=Production
Environment=DOTNET_PRINT_TELEMETRY_MESSAGE=false

[Install]
WantedBy=multi-user.target
EOF

echo "Reloading systemd..."
systemctl daemon-reload

echo "Enabling service to start on boot..."
systemctl enable ${SERVICE_NAME}

echo ""
echo "=== Setup Complete ==="
echo ""
echo "Next steps:"
echo "1. Ensure the deploy user (${DEPLOY_USER}) has the application files in ${DEPLOY_PATH}/app"
echo "2. Update the APP_DLL_NAME in this script to match your actual .dll file name"
echo "3. Configure GitHub Secrets with the following:"
echo "   - DEPLOY_SSH_KEY: Private SSH key for deployment"
echo "   - DEPLOY_USER: ${DEPLOY_USER}"
echo "   - DEPLOY_SERVER_IP: 192.168.1.245"
echo "   - DEPLOY_PATH: ${DEPLOY_PATH}"
echo "4. Start the service with: sudo systemctl start ${SERVICE_NAME}"
echo "5. Check service status with: sudo systemctl status ${SERVICE_NAME}"
echo ""
