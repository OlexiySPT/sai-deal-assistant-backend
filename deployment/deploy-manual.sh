#!/bin/bash

# Local deployment test script
# Use this to test deployment to your server manually before pushing to GitHub

set -e

# Configuration
DEPLOY_USER="${DEPLOY_USER:-www-data}"
DEPLOY_SERVER_IP="${DEPLOY_SERVER_IP:-192.168.1.245}"
DEPLOY_PATH="${DEPLOY_PATH:-/opt/sai-deal-assistant-backend}"
SSH_KEY="${SSH_KEY:-$HOME/.ssh/github_deploy_key}"

echo "=== Manual Deployment Test ==="
echo "Server: ${DEPLOY_USER}@${DEPLOY_SERVER_IP}"
echo "Deploy Path: ${DEPLOY_PATH}"
echo "SSH Key: ${SSH_KEY}"
echo ""

# Check if SSH key exists
if [ ! -f "${SSH_KEY}" ]; then
  echo "Error: SSH key not found at ${SSH_KEY}"
  echo "Please set SSH_KEY environment variable or create the key at the default location"
  exit 1
fi

echo "Building application..."
dotnet publish --configuration Release --output ./publish

echo "Testing SSH connection..."
ssh -i "${SSH_KEY}" -o StrictHostKeyChecking=no "${DEPLOY_USER}@${DEPLOY_SERVER_IP}" "echo 'SSH connection successful'"

echo "Creating backup of current deployment..."
ssh -i "${SSH_KEY}" "${DEPLOY_USER}@${DEPLOY_SERVER_IP}" "if [ -d ${DEPLOY_PATH}/app ]; then sudo cp -r ${DEPLOY_PATH}/app ${DEPLOY_PATH}/backup/app-\$(date +%Y%m%d-%H%M%S); fi"

echo "Deploying files to server..."
rsync -avz -e "ssh -i ${SSH_KEY}" ./publish/ "${DEPLOY_USER}@${DEPLOY_SERVER_IP}:${DEPLOY_PATH}/app/"

echo "Restarting service..."
ssh -i "${SSH_KEY}" "${DEPLOY_USER}@${DEPLOY_SERVER_IP}" "sudo systemctl restart sai-deal-assistant-backend"

echo "Checking service status..."
ssh -i "${SSH_KEY}" "${DEPLOY_USER}@${DEPLOY_SERVER_IP}" "sudo systemctl status sai-deal-assistant-backend --no-pager"

echo ""
echo "=== Deployment Complete ==="
echo "View logs with: ssh ${DEPLOY_USER}@${DEPLOY_SERVER_IP} 'sudo journalctl -u sai-deal-assistant-backend -f'"
