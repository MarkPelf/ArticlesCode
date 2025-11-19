# Docker - ASP.NET Core with Nginx SSL Reverse Proxy - Part1  

This project demonstrates a Docker Compose setup with: 1) ASP.NET Core 8.0 MVC application; 2) Nginx reverse proxy with SSL termination  

## 1 Introduction  

This project is based on Docker/Container technology and demonstrates how to set up an ASP.NET Core application with Nginx as a reverse proxy, handling SSL termination. 
The goal is to provide a secure HTTPS endpoint for the application while allowing for easy development and testing using Docker.  

Article will for technical reasons be divided into 2 parts:  
1. Docker - ASP.NET Core with Nginx SSL Reverse Proxy - Part1  
1. Docker - ASP.NET Core with Nginx SSL Reverse Proxy - Part2   


## 1.1 Architecture  
Project has 2 Docker Containers (services):  
1. **reverseproxy - Nginx Reverse Proxy** - Handles SSL termination and forwards requests to the ASP.NET app over HTTP.
1. **asp_net_app - ASP.NET Core Hello World Application** - The web application running on Kestrel server.

Exposed Ports:  
- **50443** - HTTPS access to the application via Nginx reverse proxy.
- **50080** - HTTP access to the application via Nginx reverse proxy (redirects to HTTPS).
- **8080** - Direct HTTP access to the ASP.NET Core application (no SSL).

Communication Flow:  
```
Https on port 50443
Client(Browser) --HTTPS:50443--> reverseproxy(SSLTermination) --HTTP:8080--> asp_net_app

Http on port 50080 (redirects to HTTPS)
Client(Browser) --HTTP:50080--> reverseproxy(SSLTermination) 
Client(Browser) <--Redirect.HTTPS:50443-- reverseproxy(SSLTermination) 
Client(Browser) --HTTPS:50443--> reverseproxy(SSLTermination) --HTTP:8080--> asp_net_app

Direct access to AspNetApp (no SSL)
Client(Browser) --HTTP:8080--> asp_net_app
```

### 1.2 Prerequisites (Windows box)  

- Docker and Docker Compose installed  
- OpenSSL (for generating SSL certificates)  

130-15pic.png  

## 2 DockerCompose - Build&Run - Setup Instructions (Windows box)  

### 2.1 Generate SSL Certificates (Development Only)  

Before running the containers, you need to generate self-signed SSL certificates:  

#### 2.1.1 Generate SSL Certificates   

**In Windows Terminal - WSL:**  
```
# Create ssl directory if it doesn't exist
mkdir -p ssl

# Generate self-signed certificate for development
sudo openssl req -x509 -nodes -days 365 -newkey rsa:2048 \
  -keyout ssl/nginx.key \
  -out ssl/nginx.crt \
  -subj "/C=RS/ST=Serbia/L=Belgrade/O=MyBussines/OU=DevOps/CN=localhost" \
  -addext "subjectAltName=DNS:localhost,IP:127.0.0.1" -passin pass:Password1!
```  
   
This will create a `ssl/` directory with:   
- `nginx.crt` - SSL certificate
- `nginx.key` - SSL private key

#### 2.1.2 Export a pfx that you can import / trust

**In Windows Terminal - WSL:**
```# Create 
sudo openssl pkcs12 -export -out ssl/nginx.pfx -inkey ssl/nginx.key -in ssl/nginx.crt
```  

#### 2.1.3 Import the pfx file as a trusted certificate

Follow the steps to import the pfx file as a trusted certificate on your machine.
- **Windows:** Use the Certificate Manager (`certmgr.msc`) to import the `nginx.pfx` into "Trusted Root Certification Authorities".   

130-10pic.png

### 2.2 Files   
Ensure you have the following files in your project directory:   

- **project_asp_ssl_01.yaml** - Defines the services, networks, and volumes for the Docker Compose setup.
- **Dockerfile** - Multi-stage build for the ASP.NET Core application.
- **nginx.Dockerfile** - Builds the Nginx image with SSL configuration.
- **nginx.conf** - Nginx configuration file with SSL settings.   

#### 2.2.1 project_asp_ssl_01.yaml - Docker Compose configuration   
```
# project_asp_ssl_01.yaml
# build & run project
# docker compose -p project_asp_ssl -f project_asp_ssl_01.yaml up -d
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
# Use other ports from 50443 and 50080 if those are not available on your machine:
        - "50443:443" 
        #- "50447:443" 
        #- "50080:80" 
        - "51080:80" 
      networks:
        - net01


  # main app
  asp_net_app:
    build:
      context: ../AspNetHelloWorld2/
      dockerfile: Dockerfile
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

#### 2.2.2 Dockerfile - Multi-stage build for ASP.NET app   
```
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY AspNetHelloWorld2.csproj .
RUN dotnet restore

# Copy everything else and build
COPY . .
RUN dotnet publish -c Release -o /app/publish --no-restore

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0-alpine AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port 8080
EXPOSE 8080

ENTRYPOINT ["dotnet", "AspNetHelloWorld2.dll"]
```   
#### 2.2.3 nginx.Dockerfile - Nginx reverse proxy image
```
FROM nginx

# Copy nginx configuration
COPY nginx.conf /etc/nginx/nginx.conf

# Create directory for SSL certificates
RUN mkdir -p /etc/nginx/ssl

# Copy SSL certificates
COPY ssl/nginx.crt /etc/nginx/ssl/nginx.crt
COPY ssl/nginx.key /etc/nginx/ssl/nginx.key

EXPOSE 80 443

CMD ["nginx", "-g", "daemon off;"]
```     

#### 2.2.4 nginx.conf - Nginx configuration with SSL settings   
```
events {
    worker_connections 1024;
}

http {
    # Strip any incoming port from Host header (e.g., "example.com:80" -> "example.com")
    map $http_host $host_without_port {
        ~^(?<hn>[^:]+)(?::\d+)?$ $hn;
    }

    # Upstream ASP.NET Core app (listening only on HTTP 8080 inside its container)
    upstream aspnet_backend {
        server asp_net_app:8080;
    }
  

    # ---------------------------
    # HTTP listener (container 80)
    # ---------------------------
    server {
        listen 80 default_server;
        server_name _;

        # Force HTTPS with 308 (permanent, preserves method/body)
        location / {
            # Redirect explicitly to the published HTTPS port of NGINX (e.g., 50443 on host)
            # Use other ports from 50443 and 50080 if those are not available on your machine:
            return 308 https://$host_without_port:50443$request_uri;
            # return 308 https://$host_without_port:50447$request_uri;
        }
    }

    # ---------------------------------------
    # HTTPS reverse proxy (container 443 SSL)
    # ---------------------------------------
    server {
        listen 443 ssl default_server;
        server_name _;

        # TLS cert/key (self-signed or provided)
        ssl_certificate     /etc/nginx/ssl/nginx.crt;
        ssl_certificate_key /etc/nginx/ssl/nginx.key;
        ssl_protocols TLSv1.2 TLSv1.3;
        ssl_ciphers HIGH:!aNULL:!MD5;
        ssl_prefer_server_ciphers on;

        # Optional security headers (uncomment after validation)
        # add_header Strict-Transport-Security "max-age=31536000; includeSubDomains" always;
        # add_header X-Content-Type-Options nosniff;
        # add_header X-Frame-Options DENY;

        location / {
            proxy_pass http://aspnet_backend;
            proxy_http_version 1.1;

            # Connection / Upgrade
            proxy_set_header Upgrade $http_upgrade;
            proxy_set_header Connection keep-alive;

            # Forward original host WITHOUT port for app logic
            proxy_set_header Host $host_without_port;
            proxy_set_header X-Forwarded-Host $host_without_port;

            # Forward scheme & external HTTPS port so ASP.NET Core can build correct absolute URLs
            proxy_set_header X-Forwarded-Proto https;
            proxy_set_header X-Forwarded-Port 50443;
            proxy_set_header X-Forwarded-Ssl on;

            # Client IP chain
            proxy_set_header X-Real-IP $remote_addr;
            proxy_set_header X-Forwarded-For $proxy_add_x_forwarded_for;

            # Disable caching for upgraded connections
            proxy_cache_bypass $http_upgrade;
        }
    }
}
```    

### 2.3 Services (Docker Containers)   

#### 2.3.1 reverseproxy   
- Built from `nginx.Dockerfile`
- Handles SSL termination
- Forwards requests to ASP.NET app via HTTP
- Ports:
  - 50443:443 (HTTPS)
  - 50080:80 (HTTP, redirects to HTTPS)

#### 2.3.2 asp_net_app   
- Built from `Dockerfile` using multi-stage build
- ASP.NET Core 8.0 MVC application
- Listens on port 8080 (HTTP only, SSL handled by proxy)
- Port 8080 exposed for direct access (optional)

### 2.4 Build and Run   

Start the containers:  
**In Windows Terminal:**
```
# build & run project
docker compose -p project_asp_ssl -f project_asp_ssl_01.yaml up -d
# see images
docker image ls
# see containers
docker ps -a
```   

Stop the containers:  
**In Windows Terminal:**
```
# stop project
docker compose -p project_asp_ssl down
```

130-11pic.png

130-12pic.png

### 2.5 Access the Application    

- **HTTPS (recommended):** https://localhost:50443
- **HTTP (redirects to HTTPS):** http://localhost:50080
- **Direct ASP.NET app (without SSL):** http://localhost:8080

**Note:** Since we're using self-signed certificates, your browser will show a security warning. This is expected for development. Click "Advanced" and proceed to accept the certificate.

### 2.6 Screenshots from Windows Box    

130-22pic.png

130-23pic.png

### 2.7 Production Considerations   

For production deployment:

1. **Use real SSL certificates** from a Certificate Authority (Let's Encrypt, etc.)
2. **Remove direct port exposure** (8080) for the ASP.NET app
3. **Add security headers** in nginx configuration
4. **Configure proper logging**
5. **Use Docker secrets** for sensitive data


### 2.8 Troubleshooting   

#### 2.8.1 Check container logs:   
```
docker compose -p project_asp_ssl logs asp_net_app
docker compose -p project_asp_ssl logs reverseproxy
```

#### 2.8.2 Rebuild containers:   
```
docker compose -p project_asp_ssl -f project_asp_ssl_01.yaml up -d --build
```

#### 2.8.3 Check if containers are running:  
```
docker ps  
```

## 3 References     
 
1. Containerize a .NET app reference, https://learn.microsoft.com/en-us/dotnet/core/containers/publish-configuration
1. Docker with SSL and an nginx reverse proxy, https://gist.github.com/dahlsailrunner/679e6dec5fd769f30bce90447ae80081

