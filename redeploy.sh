#!/bin/bash

echo "Stopping client and api container"
docker stop api adminv-clientapp-1

echo "Removing containers of api and client apps"
docker rm adminv-clientapp-1 api

echo "Removing images of api and client apps"
docker rmi 192.168.11.15:58090/himeraradio:latest 192.168.11.15:58090/himeraradio-client:latest

docker-compose up -d
