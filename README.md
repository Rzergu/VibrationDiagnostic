curl -v -F name=HimeraSend -F recordId=0  -F file=@HimeraSend.sh  http://192.168.0.101:4300/files
docker buildx build -f Dockerfile --platform linux/amd64 --tag lieznovskiy/himeraradio:latest --load .



To start debugging client:

GoTo:

./Himera/firmware_server_project/HimeraRadio/ClientApp/angular-docker

to run use: ng serve

and launch with Visual Studio Code:

lauch.json

"

    {
        // Use IntelliSense to learn about possible attributes.
        // Hover to view descriptions of existing attributes.
        // For more information, visit: https://go.microsoft.com/fwlink/?linkid=830387
        "version": "0.2.0",
        "configurations": [
            {
                "type": "chrome",
                "request": "launch",
                "name": "Launch Chrome against localhost",
                "url": "http://localhost:4200",
                "webRoot": "${workspaceFolder}"
            }
        ]
    }

"
to build docker image on Mac :

docker buildx build -f Dockerfile --platform linux/amd64 --tag lieznovskiy/himeraradio-client:latest --load .

On Windows/Linux just use image that mac build during debug

to push image to docker hub: 

docker push lieznovskiy/himeraradio-client:latest  //WILL/MUST be changed

To start debugging Server:

open .sln with Visual Studio and debug it with docker

to build docker image on Mac :

docker buildx build -f Dockerfile --platform linux/amd64 --tag lieznovskiy/himeraradio:latest --load .

On Windows/Linux just use image that mac build during debug

to push image to docker hub: 

docker push lieznovskiy/himeraradio:latest  //WILL/MUST be changed


SERVER SSH: 

radiotest.qmfrc.com:221

USER: adminv
PASS: fGhbb5566773

To restart: 

sudo su (same cred)

to run:

docker-compose up

to shut down:

docker-compose down

to stop all:

docker stop $(docker ps -a -q)

to clear containers:

docker rm $(docker ps -a -q)

to clear images:

docker rmi $(docker ps -a -q)

Backend usees ASP.NET CORE
For database PostgreSQL to store scheme and MongoDB for files
For client part Angular 18
For hosting Docker/Docker Compose
And nginx as a server

docker login 
HimAdmReg
fGhbb5566773
docker login 192.168.11.15:58090