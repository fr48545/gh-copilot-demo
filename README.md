# Github Copilot demo 

## Demo Scenarios

### To start discovering Github Copilot jump to [`The Ultimate GitHub Copilot Tutorial on MOAW`](https://aka.ms/github-copilot-hol)
<br/>


## Solution Overview


This repository has been inspired by the [Azure Container Apps: Dapr Albums Sample](https://github.com/Azure-Samples/containerapps-dapralbums)

It's used as a code base to demonstrate Github Copilot capabilities.

The solution is composed of two services: the .net album API and the NodeJS album viewer.


### Album API (`albums-api`)

The [`albums-api`](./albums-api) is a .NET 8 minimal Web API that manages a list of albums in memory.

### Album Viewer (`album-viewer`)

The [`album-viewer`](./album-viewer) is a modern Vue.js 3 application built with TypeScript through which the albums retrieved by the API are surfaced. The application uses the Vue 3 Composition API with full TypeScript support for enhanced developer experience and type safety. In order to display the repository of albums, the album viewer contacts the backend album API.

## Getting Started

There are multiple ways to run this solution locally. Choose the method that best fits your development workflow.

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Node.js](https://nodejs.org/) (version 16 or higher)
- [TypeScript](https://www.typescriptlang.org/) (automatically installed with project dependencies)
- [Visual Studio Code](https://code.visualstudio.com/) (recommended)

### Option 1: Using VS Code Debug Panel (Recommended)

This is the easiest way to run the solution with full debugging capabilities.

1. Open the solution in Visual Studio Code
2. Open the Debug panel (Ctrl+Shift+D / Cmd+Shift+D)
3. Select **"All services"** from the dropdown
4. Click the green play button or press F5

This will automatically:
- Build the .NET API and start it on `http://localhost:3000`
- Start the Vue.js TypeScript app on `http://localhost:3001`
- Open both services in your default browser

You can also run individual services:
- **"C#: Album API Debug"** - Runs only the .NET API
- **"Node.js: Album Viewer Debug"** - Runs only the Vue.js TypeScript frontend

### Option 2: Command Line

#### Starting the Album API (.NET)

```powershell
# Navigate to the API directory
cd albums-api

# Restore dependencies (first time only)
dotnet restore

# Run the API
dotnet run
```

The API will start on `http://localhost:3000` and you can access the Swagger documentation at `http://localhost:3000/swagger`.

#### Starting the Album Viewer (Vue.js + TypeScript)

```powershell
# Navigate to the viewer directory
cd album-viewer

# Install dependencies (first time only)
npm install

# Start the development server
npm run dev

# Optional: Run TypeScript type checking
npm run type-check
```

The Vue.js TypeScript app will start on `http://localhost:3001` and automatically open in your browser.

#### Running Both Services

You can run both services simultaneously using separate terminal windows:

```powershell
# Terminal 1 - Start the API
cd albums-api
dotnet run

# Terminal 2 - Start the Vue TypeScript app
cd album-viewer
npm run dev
```

### Environment Configuration

The solution uses the following default configuration:

- **Album API**: Runs on `http://localhost:3000`
- **Album Viewer**: Runs on `http://localhost:3001` (TypeScript + Vue 3)
- **API Endpoint**: The Vue app is configured to call the API at `localhost:3000`

If you need to change these settings, you can modify:
- API port: `albums-api/Properties/launchSettings.json`
- Vue app configuration: Environment variables in `.vscode/launch.json` or set `VITE_ALBUM_API_HOST` environment variable

### Alternative: GitHub Codespaces

The easiest way is to open this solution in a GitHub Codespace, or run it locally in a devcontainer. The development environment will be automatically configured for you.

## Deploy to Azure Container Apps

The following steps build both container images in Azure Container Registry and deploy them to Azure Container Apps. Docker is not required locally because the image builds run in Azure.

### Azure prerequisites

- An active [Azure subscription](https://azure.microsoft.com/free/)
- [Azure CLI](https://learn.microsoft.com/cli/azure/install-azure-cli) version 2.62 or later
- Permission to create resource groups, role assignments, container registries, and Container Apps

Run all commands from the repository root in a Bash-compatible terminal.

### 1. Sign in and configure deployment variables

```bash
az login
az account set --subscription "<subscription-name-or-id>"

export RESOURCE_GROUP="rg-albums-demo"
export LOCATION="eastus"
export CONTAINERAPPS_ENVIRONMENT="albums-environment"
# Azure Container Registry names must be globally unique and contain only
# lowercase letters and numbers.
export ACR_NAME="albums$(date +%s)"
```

### 2. Create the Azure resources

```bash
az extension add --name containerapp --upgrade
az provider register --namespace Microsoft.App --wait
az provider register --namespace Microsoft.OperationalInsights --wait

az group create \
	--name "$RESOURCE_GROUP" \
	--location "$LOCATION"

az acr create \
	--resource-group "$RESOURCE_GROUP" \
	--name "$ACR_NAME" \
	--sku Basic \
	--admin-enabled true

az containerapp env create \
	--resource-group "$RESOURCE_GROUP" \
	--name "$CONTAINERAPPS_ENVIRONMENT" \
	--location "$LOCATION"
```

### 3. Build the container images in Azure

```bash
az acr build \
	--registry "$ACR_NAME" \
	--image albums-api:latest \
	./albums-api

az acr build \
	--registry "$ACR_NAME" \
	--image album-viewer:latest \
	./album-viewer

export ACR_SERVER="$(az acr show --name "$ACR_NAME" --query loginServer --output tsv)"
export ACR_USERNAME="$(az acr credential show --name "$ACR_NAME" --query username --output tsv)"
export ACR_PASSWORD="$(az acr credential show --name "$ACR_NAME" --query 'passwords[0].value' --output tsv)"
```

### 4. Deploy the API

```bash
az containerapp create \
	--resource-group "$RESOURCE_GROUP" \
	--environment "$CONTAINERAPPS_ENVIRONMENT" \
	--name albums-api \
	--image "$ACR_SERVER/albums-api:latest" \
	--registry-server "$ACR_SERVER" \
	--registry-username "$ACR_USERNAME" \
	--registry-password "$ACR_PASSWORD" \
	--ingress external \
	--target-port 8080 \
	--env-vars \
		ASPNETCORE_ENVIRONMENT=Production \
		ASPNETCORE_FORWARDEDHEADERS_ENABLED=true

export API_FQDN="$(az containerapp show \
	--resource-group "$RESOURCE_GROUP" \
	--name albums-api \
	--query properties.configuration.ingress.fqdn \
	--output tsv)"

curl --fail "https://$API_FQDN/albums"
```

### 5. Deploy the album viewer

The viewer's Nginx server forwards `/albums` requests to the API URL supplied through `ALBUM_API_HOST`.

```bash
az containerapp create \
	--resource-group "$RESOURCE_GROUP" \
	--environment "$CONTAINERAPPS_ENVIRONMENT" \
	--name album-viewer \
	--image "$ACR_SERVER/album-viewer:latest" \
	--registry-server "$ACR_SERVER" \
	--registry-username "$ACR_USERNAME" \
	--registry-password "$ACR_PASSWORD" \
	--ingress external \
	--target-port 8080 \
	--env-vars "ALBUM_API_HOST=https://$API_FQDN"

export VIEWER_FQDN="$(az containerapp show \
	--resource-group "$RESOURCE_GROUP" \
	--name album-viewer \
	--query properties.configuration.ingress.fqdn \
	--output tsv)"

echo "Album viewer: https://$VIEWER_FQDN"
curl --fail --head "https://$VIEWER_FQDN"
```

Open the printed viewer URL in a browser to verify that the albums are loaded from the API.

> For simplicity, this demo uses the ACR administrator credentials. For production workloads, disable the ACR administrator account and configure image pulls with a managed identity and the `AcrPull` role.

### Deploy updated versions

After changing either service, rebuild its image with a unique tag and update the corresponding Container App. For example:

```bash
export IMAGE_TAG="$(git rev-parse --short HEAD)"

az acr build \
	--registry "$ACR_NAME" \
	--image "albums-api:$IMAGE_TAG" \
	./albums-api

az containerapp update \
	--resource-group "$RESOURCE_GROUP" \
	--name albums-api \
	--image "$ACR_SERVER/albums-api:$IMAGE_TAG"
```

### Clean up

Delete the resource group when the deployment is no longer needed to avoid additional charges:

```bash
az group delete --name "$RESOURCE_GROUP" --yes --no-wait
unset ACR_PASSWORD
```