#!/bin/bash

set -e

IMAGE_NAME=podsofkon
#IMAGE_VERSION=latest
IMAGE_VERSION=0.2
#DOCKER_REGISTRY=us-ashburn-1.ocir.io/oradbclouducm/gd74087885
DOCKER_REGISTRY=us-ashburn-1.ocir.io/oradbclouducm/podsofkon

export IMAGE=${DOCKER_REGISTRY}/${IMAGE_NAME}:${IMAGE_VERSION}
export IMAGE_VERSION=$IMAGE_VERSION

mvn clean

#docker buildx build --platform linux/amd64,linux/arm64 -t $IMAGE .
podman build -t=$IMAGE .
#docker buildx build --platform=linux/amd64 -t=$IMAGE .

#docker push --platform linux/amd64  "$IMAGE"
podman push "$IMAGE"
#if [  $? -eq 0 ]; then
#    podman rmi "$IMAGE"
#fi

