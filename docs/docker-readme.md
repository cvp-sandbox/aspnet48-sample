# Docker Setup for Event Registration System

This document provides instructions for containerizing the Event Registration System ASP.NET Framework 4.8 application using Docker on Windows Server 2022.

## Prerequisites

- Windows Server 2022 with Docker installed
- Docker configured to use Windows containers
- Access to the application source code repository

## Docker Files

- `Dockerfile`: Multi-stage build file for building and running the application
- `.dockerignore`: Specifies which files to exclude from the Docker build context

## Building the Docker Image

The Dockerfile is designed to install Git and clone the repository directly during the build process, which is ideal for CI/CD pipelines.

### Basic Build

```powershell
# Build with default repository settings
docker build -t event-registration-system:latest .
```

### Specifying Repository URL and Branch

```powershell
# Build with custom repository URL and branch
docker build -t event-registration-system:latest \
  --build-arg REPO_URL=https://github.com/yourorg/your-repo.git \
  --build-arg REPO_BRANCH=develop .
```

### Using Private Repository

For private repositories, you can use:

```powershell
# Using credentials in the URL
docker build -t event-registration-system:latest \
  --build-arg REPO_URL=https://username:personal-access-token@github.com/yourorg/private-repo.git .

# Or using Git credential helper before building
git config --global credential.helper store
# Enter credentials when prompted
git ls-remote https://github.com/yourorg/private-repo.git
# Then build (Git will use stored credentials)
docker build -t event-registration-system:latest \
  --build-arg REPO_URL=https://github.com/yourorg/private-repo.git .
```

## Running the Docker Container

To run the Docker container:

```powershell
# Run the container, mapping port 80 to host port 8080
docker run -d -p 8080:80 --name event-registration-app event-registration-system:latest
```

Access the application at http://localhost:8080

## CI/CD Integration with Jenkins

### Sample Jenkinsfile

```groovy
pipeline {
    agent {
        label 'windows-server-2022'
    }
    
    parameters {
        string(name: 'REPO_URL', defaultValue: 'https://github.com/yourorg/aspnet-event-management.git', description: 'Git repository URL')
        string(name: 'REPO_BRANCH', defaultValue: 'main', description: 'Git branch or tag to build')
    }
    
    stages {
        stage('Checkout Dockerfile') {
            steps {
                // Only need to checkout the Dockerfile and related files, not the entire repo
                checkout([
                    $class: 'GitSCM',
                    branches: [[name: "${params.REPO_BRANCH}"]],
                    userRemoteConfigs: [[url: "${params.REPO_URL}"]],
                    extensions: [[$class: 'SparseCheckoutPaths', sparseCheckoutPaths: [
                        [path: 'Dockerfile'], 
                        [path: '.dockerignore']
                    ]]]
                ])
            }
        }
        
        stage('Build Docker Image') {
            steps {
                // Build using the repository URL and branch as build arguments
                bat """
                    docker build -t event-registration-system:%BUILD_NUMBER% ^
                    --build-arg REPO_URL=${params.REPO_URL} ^
                    --build-arg REPO_BRANCH=${params.REPO_BRANCH} .
                """
                bat 'docker tag event-registration-system:%BUILD_NUMBER% event-registration-system:latest'
            }
        }
        
        stage('Test') {
            steps {
                bat 'docker run --rm event-registration-system:%BUILD_NUMBER% powershell -Command "Test-NetConnection -ComputerName localhost -Port 80"'
            }
        }
        
        stage('Push to Registry') {
            steps {
                withCredentials([usernamePassword(credentialsId: 'docker-registry-credentials', usernameVariable: 'DOCKER_USERNAME', passwordVariable: 'DOCKER_PASSWORD')]) {
                    bat 'docker login your-registry.example.com -u %DOCKER_USERNAME% -p %DOCKER_PASSWORD%'
                    bat 'docker tag event-registration-system:%BUILD_NUMBER% your-registry.example.com/event-registration-system:%BUILD_NUMBER%'
                    bat 'docker push your-registry.example.com/event-registration-system:%BUILD_NUMBER%'
                    bat 'docker tag event-registration-system:%BUILD_NUMBER% your-registry.example.com/event-registration-system:latest'
                    bat 'docker push your-registry.example.com/event-registration-system:latest'
                }
            }
        }
        
        stage('Deploy') {
            steps {
                // Deploy commands specific to your environment
                withCredentials([file(credentialsId: 'docker-compose-file', variable: 'DOCKER_COMPOSE_FILE')]) {
                    bat 'copy %DOCKER_COMPOSE_FILE% docker-compose.prod.yml'
                    bat 'docker-compose -f docker-compose.prod.yml up -d'
                }
            }
        }
    }
    
    post {
        always {
            bat 'docker rmi event-registration-system:%BUILD_NUMBER% -f || true'
            cleanWs()
        }
    }
}
```

## Configuration

### Environment Variables

You can pass environment variables to configure the application:

```powershell
docker run -d -p 8080:80 \
  -e "ConnectionStrings:DefaultConnection=Server=db-server;Database=EventDB;User Id=sa;Password=YourPassword;" \
  --name event-registration-app \
  event-registration-system:latest
```

### Volume Mounts

For persistent data storage:

```powershell
docker run -d -p 8080:80 \
  -v C:\DockerData\EventRegistration\App_Data:C:\inetpub\wwwroot\App_Data \
  --name event-registration-app \
  event-registration-system:latest
```

## Notes for Production Deployment

1. **Database Configuration**: Update connection strings to point to your production database server.

2. **SSL/TLS**: For production, configure SSL/TLS either through the container or via a reverse proxy.

3. **Windows Authentication**: If Windows Authentication is required, additional configuration may be needed.

4. **Container Orchestration**: For high availability, consider using Docker Swarm or Kubernetes for Windows containers.

5. **Resource Limits**: Set appropriate memory and CPU limits for the container based on your application's needs.

## Troubleshooting

- **Container fails to start**: Check the Docker logs using `docker logs event-registration-app`
- **Application errors**: Access the event logs inside the container using `docker exec event-registration-app powershell Get-EventLog -LogName Application -Newest 20`
- **IIS issues**: Verify IIS configuration with `docker exec event-registration-app powershell Get-WebSite`
