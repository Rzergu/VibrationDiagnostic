#!/bin/bash

cd HimeraRadio
docker buildx build -f Dockerfile --platform linux/amd64 --tag lieznovskiy/vibration-diagnostic:latest --load .
docker push lieznovskiy/vibration-diagnostic:latest

cd ClientApp/angular-docker
docker buildx build -f Dockerfile --platform linux/amd64 --tag lieznovskiy/vibration-diagnostic-client:latest --load .
docker push lieznovskiy/vibration-diagnostic-client:latest
