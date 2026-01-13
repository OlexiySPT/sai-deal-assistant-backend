#!/bin/bash
set -e

echo "Starting deployment to dev server..."

# Configuration
CONTAINER_NAME="sai-deal-assistant-dev"
IMAGE_NAME="sai-deal-assistant:dev"
HOST_PORT=5000
CONTAINER_PORT=8080

# Stop and remove existing container if it exists
echo "Stopping existing container..."
docker stop $CONTAINER_NAME 2>/dev/null || true
docker rm $CONTAINER_NAME 2>/dev/null || true

# Remove old image if exists
echo "Removing old image..."
docker rmi $IMAGE_NAME 2>/dev/null || true

# Load the new image
echo "Loading new Docker image..."
docker load -i /tmp/sai-deal-assistant-dev.tar

# Run the new container
echo "Starting new container..."
docker run -d \
  --name $CONTAINER_NAME \
  --restart unless-stopped \
  -p $HOST_PORT:$CONTAINER_PORT \
  -e ASPNETCORE_ENVIRONMENT=Production \
  $IMAGE_NAME

# Clean up
echo "Cleaning up..."
rm -f /tmp/sai-deal-assistant-dev.tar

# Show container status
echo "Deployment completed successfully!"
docker ps | grep $CONTAINER_NAME

echo "Application should be available at http://192.168.1.245:$HOST_PORT"
