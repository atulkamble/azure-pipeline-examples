Here are different levels of Azure Pipelines YAML code samples, from **basic** to **advanced**.

---

### **1. Basic Azure Pipeline (CI Only)**
This simple pipeline builds a .NET application on a Windows agent.

```yaml
trigger:
- main

pool:
  vmImage: 'windows-latest'

steps:
- task: UseDotNet@2
  inputs:
    packageType: 'sdk'
    version: '6.x'

- script: dotnet build
  displayName: 'Build Project'
```

---

### **2. Medium Complexity (CI/CD - Build & Deploy to Azure App Service)**
This pipeline builds a .NET app and deploys it to **Azure App Service**.

```yaml
trigger:
- main

pool:
  vmImage: 'ubuntu-latest'

variables:
  buildConfiguration: 'Release'

steps:
- task: UseDotNet@2
  inputs:
    packageType: 'sdk'
    version: '6.x'

- script: dotnet build --configuration $(buildConfiguration)
  displayName: 'Build Project'

- task: DotNetCoreCLI@2
  inputs:
    command: 'publish'
    publishWebProjects: true
    arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)'
    zipAfterPublish: true

- task: PublishBuildArtifacts@1
  inputs:
    pathToPublish: '$(Build.ArtifactStagingDirectory)'
    artifactName: 'drop'

- task: AzureWebApp@1
  inputs:
    azureSubscription: '<Your Azure Service Connection>'
    appName: '<Your-App-Service-Name>'
    package: '$(Build.ArtifactStagingDirectory)/**/*.zip'
```

---

### **3. Advanced Pipeline (CI/CD with Multi-Stage & Environments)**
This pipeline includes separate **Build, Test, and Deploy** stages, along with a manual approval step.

```yaml
trigger:
- main

stages:
- stage: Build
  jobs:
  - job: BuildJob
    pool:
      vmImage: 'ubuntu-latest'
    steps:
    - task: UseDotNet@2
      inputs:
        packageType: 'sdk'
        version: '6.x'

    - script: dotnet build --configuration Release
      displayName: 'Build Project'

    - task: DotNetCoreCLI@2
      inputs:
        command: 'publish'
        publishWebProjects: true
        arguments: '--configuration Release --output $(Build.ArtifactStagingDirectory)'
        zipAfterPublish: true

    - task: PublishBuildArtifacts@1
      inputs:
        pathToPublish: '$(Build.ArtifactStagingDirectory)'
        artifactName: 'drop'

- stage: Test
  dependsOn: Build
  jobs:
  - job: TestJob
    pool:
      vmImage: 'ubuntu-latest'
    steps:
    - script: dotnet test
      displayName: 'Run Unit Tests'

- stage: Deploy
  dependsOn: Test
  condition: succeeded()
  jobs:
  - deployment: DeployToAzure
    displayName: 'Deploy to Azure'
    environment: Production
    strategy:
      runOnce:
        deploy:
          steps:
          - task: AzureWebApp@1
            inputs:
              azureSubscription: '<Your Azure Service Connection>'
              appName: '<Your-App-Service-Name>'
              package: '$(Pipeline.Workspace)/drop/**/*.zip'
```

---

### **4. Hard Pipeline (Multi-Stage, Infrastructure as Code & Blue-Green Deployment)**
This pipeline integrates **Terraform** for infrastructure provisioning and implements **Blue-Green Deployment** using Azure App Service slots.

```yaml
trigger:
- main

variables:
  buildConfiguration: 'Release'
  azureSubscription: '<Your Azure Service Connection>'
  appName: '<Your-App-Service-Name>'
  slotName: 'staging'

stages:
- stage: Infrastructure
  jobs:
  - job: DeployInfra
    pool:
      vmImage: 'ubuntu-latest'
    steps:
    - task: TerraformInstaller@0
      inputs:
        terraformVersion: '1.2.0'

    - script: |
        terraform init
        terraform apply -auto-approve
      displayName: 'Deploy Infrastructure'

- stage: Build
  dependsOn: Infrastructure
  jobs:
  - job: BuildApp
    pool:
      vmImage: 'ubuntu-latest'
    steps:
    - script: dotnet build --configuration $(buildConfiguration)
      displayName: 'Build Project'

    - task: DotNetCoreCLI@2
      inputs:
        command: 'publish'
        publishWebProjects: true
        arguments: '--configuration $(buildConfiguration) --output $(Build.ArtifactStagingDirectory)'
        zipAfterPublish: true

    - task: PublishBuildArtifacts@1
      inputs:
        pathToPublish: '$(Build.ArtifactStagingDirectory)'
        artifactName: 'drop'

- stage: Deploy
  dependsOn: Build
  jobs:
  - deployment: DeployApp
    environment: Production
    strategy:
      runOnce:
        deploy:
          steps:
          - task: AzureWebApp@1
            inputs:
              azureSubscription: $(azureSubscription)
              appName: $(appName)
              package: '$(Pipeline.Workspace)/drop/**/*.zip'
              deployToSlotOrASE: true
              resourceGroupName: '<Your-Resource-Group>'
              slotName: $(slotName)

- stage: SwapSlots
  dependsOn: Deploy
  jobs:
  - job: SwapSlots
    pool:
      vmImage: 'ubuntu-latest'
    steps:
    - task: AzureCLI@2
      inputs:
        azureSubscription: $(azureSubscription)
        scriptType: 'bash'
        scriptLocation: 'inlineScript'
        inlineScript: |
          az webapp deployment slot swap --resource-group '<Your-Resource-Group>' --name $(appName) --slot $(slotName) --target-slot production
```

---

### **Summary**
| Level | Description |
|-------|-------------|
| **Basic** | Simple .NET build pipeline |
| **Medium** | CI/CD with build and deployment to Azure App Service |
| **Advanced** | Multi-stage pipeline with unit testing and approvals |
| **Hard** | Full automation with Terraform & Blue-Green Deployment |

Let me know if you need modifications! 🚀
