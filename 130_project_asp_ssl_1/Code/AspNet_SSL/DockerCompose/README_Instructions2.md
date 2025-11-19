# Docker - ASP.NET Core with Nginx SSL Reverse Proxy - Part2    

This project demonstrates a Docker Compose setup with:   
- ASP.NET Core 8.0 MVC application
- Nginx reverse proxy with SSL termination


This project demonstrates a Docker Compose setup with: 1) ASP.NET Core 8.0 MVC application; 2) Nginx reverse proxy with SSL termination   

## 1 Introduction

This project is based on Docker/Container technology and demonstrates how to set up an ASP.NET Core application with Nginx as a reverse proxy, handling SSL termination. 
The goal is to provide a secure HTTPS endpoint for the application while allowing for easy development and testing using Docker.   

Article will for technical reasons be divided into 2 parts:
1. Docker - ASP.NET Core with Nginx SSL Reverse Proxy - Part1   
1. Docker - ASP.NET Core with Nginx SSL Reverse Proxy - Part2    


## 2 Pushing to Docker Hub    

To push the ASP.NET Core application image to Docker Hub, follow these steps:	

1- **Build the ASP.NET Core application image:**  
   ```
   # build image
   docker build -t docker.io/markpelf/asp_net_app:1.0 . 
   # see images
   docker image ls
   ```

   131-10pic.png


2- **Log in to Docker Hub:**   
 ```
   docker login
   ```
3- push the image to Docker Hub:   
   ```
   docker push docker.io/markpelf/asp_net_app:1.0
   ```

   131-11pic.png


## 3 DockerCompose - Run from Docker Hub    

To run the ASP.NET Core application from Docker Hub with Nginx reverse proxy and SSL, follow these steps:
1- **Create a `project_asp_ssl_02.yaml` file** with the following content:
   ```
# project_asp_ssl_02.yaml
# build & run project
# docker compose -p project_asp_ssl -f project_asp_ssl_02.yaml up -d
# see images
# docker image ls
# see containers
# docker ps -a
# stop project
# docker compose -p project_asp_ssl down

services:
  reverseproxy:
      build:
        context: .
        dockerfile: nginx.Dockerfile
      depends_on: 
        - asp_net_app
      ports:
        - "50443:443"
        - "50080:80"
      networks:
        - net01


  # main app
  asp_net_app:
    image: docker.io/markpelf/asp_net_app:1.0
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
    ports:
      - "8080:8080"
    networks:
      - net01

networks:
  net01:
   ```   
   
2- Copy folder `ssl/` containing `nginx.crt`, `nginx.key` and `nginx.pfx` to the same directory as `project_asp_ssl_02.yaml`.   
3- Import the pfx file as a trusted certificate. Follow the steps to import the pfx file as a trusted certificate on your machine.    
4- **Run the Docker Compose setup:**   
   ```
    # build & run project
	docker compose -p project_asp_ssl -f project_asp_ssl_02.yaml up -d
	# see images
	docker image ls
	# see containers
	docker ps -a
	# stop project
	docker compose -p project_asp_ssl down 
   ```
5- **Access the application:**
   - **HTTPS (recommended):** https://localhost:50443

### 3.1 Running on Windows Box  

Similar as in Part1.  


## 4 Moving Docker images without Docker Hub   

### 4.1 Using docker save and docker load   

This is the most common and robust method for transferring Docker images between hosts without a registry. On the source machine.
```
    docker save -o my_image.tar my_image:tag
``` 
This command saves the Docker image named my_image with the specified tag into a single .tar archive file named my_image.tar. Transfer the .tar file.
Copy the my_image.tar file to the destination machine using tools like scp, rsync, or even a USB drive. On the destination machine.
```
    docker load -i my_image.tar 
```
This command loads the Docker image from the my_image.tar file into the local Docker image repository on the destination machine.
Docker images compress well, so you can zip the .tar file before transferring it to save bandwidth and storage space.

### 4.2 Renaming and saving images   
On the source machine, rename the images to match those specified in the new Docker Compose file:
```
	docker tag project_asp_ssl-asp_net_app:latest asp_net_app:1.3
	docker tag project_asp_ssl-reverseproxy:latest reverseproxy:1.3

	#Then save the renamed images to tar files:
	docker save -o asp_net_app-1.3.tar asp_net_app:1.3
	docker save -o reverseproxy-1.3.tar reverseproxy:1.3
```   

Now we will delete the renamed images from the source machine to simulate a fresh environment on the destination machine:   
```
	docker rmi asp_net_app:1.3
	docker rmi reverseproxy:1.3
	docker rmi project_asp_ssl-asp_net_app:latest
	docker rmi project_asp_ssl-reverseproxy:latest
```   

### 4.3 New Docker Compose file
Here is new Docker Compose file, using local images instead of pulling from Docker Hub:
```
# project_asp_ssl_03.yaml
# build & run project
# docker compose -p project_asp_ssl -f project_asp_ssl_03.yaml up -d
# see images
# docker image ls
# see containers
# docker ps -a
# stop project
# docker compose -p project_asp_ssl down

services:
  reverseproxy:
      image: reverseproxy:1.3
      depends_on: 
        - asp_net_app
      ports:
        - "50443:443"
        - "50080:80"
      networks:
        - net01


  # main app
  asp_net_app:
    image: asp_net_app:1.3
    environment:
      - ASPNETCORE_ENVIRONMENT=Production
      - ASPNETCORE_URLS=http://+:8080
    ports:
      - "8080:8080"
    networks:
      - net01

networks:
  net01:
```    

### 4.4 Loading images on destination machine and running Docker Compose
On the destination machine, load the saved images from the tar files:
```
	docker load -i asp_net_app-1.3.tar
	docker load -i reverseproxy-1.3.tar
```
Now, run the Docker Compose setup using the new Docker Compose file:
```
	# build & run project
	docker compose -p project_asp_ssl -f project_asp_ssl_03.yaml up -d
	# see images
	docker image ls
	# see containers
	docker ps -a
	# stop project
	docker compose -p project_asp_ssl down 
```    

All works as before, with the ASP.NET Core application accessible via HTTPS through the Nginx reverse proxy.

## 5 References     

1. Containerize a .NET app reference, https://learn.microsoft.com/en-us/dotnet/core/containers/publish-configuration
1. Docker with SSL and an nginx reverse proxy, https://gist.github.com/dahlsailrunner/679e6dec5fd769f30bce90447ae80081

