# **Analyze the limitations of a polling-based web app**

# Download sample app code
>1.Run the following command in Azure Cloud Shell from the portal to clone the app from GitHub.

`git clone https://github.com/MicrosoftDocs/mslearn-advocates.azure-functions-and-signalr.git serverless-demo`

>2.Run the following command to go to the new folder into which you cloned the repo:

`cd serverless-demo`

>3.The beginning state of the app is located in the start folder. Make sure you are in that folder for the rest of this module. Run the following command to open the start folder in Visual Studio Code:

`code start`

# Create a Storage account

>1.Run the following command in Cloud Shell to define a name for your Azure Storage account.

`export STORAGE_ACCOUNT_NAME=mslsigrstorage$(openssl rand -hex 5)`    
`echo "Storage Account Name: $STORAGE_ACCOUNT_NAME"`

>2.Run the following `az storage account create` command to create a storage account for your function and static website.

`az storage account create \`      
`--name $STORAGE_ACCOUNT_NAME \`    
`--resource-group learn-b61d7cad-6e55-4a9b-8b5d-e6a0a48dee88 \`    
`--kind StorageV2 \`    
`--sku Standard_LRS`
  
# Create an Azure Cosmos DB account

>1.Run the following `az cosmosdb create` command in Cloud Shell to create a new Azure Cosmos DB account in your sandbox resource group.

`az cosmosdb create  \`    
`--name msl-sigr-cosmos-$(openssl rand -hex 5) \`    
`--resource-group learn-b61d7cad-6e55-4a9b-8b5d-e6a0a48dee88`

# Update local settings

>1.Run the following commands in Cloud Shell to get the connection strings for the resources we created in this exercise.

`STORAGE_CONNECTION_STRING=$(az storage account show-connection-string \`    
`--name $(az storage account list \`    
`--resource-group learn-b61d7cad-6e55-4a9b-8b5d-e6a0a48dee88 \`    
`--query [0].name -o tsv) \`    
`--resource-group learn-b61d7cad-6e55-4a9b-8b5d-e6a0a48dee88 \`     
`--query "connectionString" -o tsv)`      
      
`COSMOSDB_ACCOUNT_NAME=$(az cosmosdb list \`     
`--resource-group learn-b61d7cad-6e55-4a9b-8b5d-e6a0a48dee88 \`     
`--query [0].name -o tsv)`    
     
`COSMOSDB_CONNECTION_STRING=$(az cosmosdb list-connection-strings  \`   
`--name $COSMOSDB_ACCOUNT_NAME \`    
`--resource-group learn-b61d7cad-6e55-4a9b-8b5d-e6a0a48dee88 \`     
`--query "connectionStrings[?description=='Primary SQL Connection String'].connectionString" -o tsv)`      
     
`COSMOSDB_MASTER_KEY=$(az cosmosdb list-keys \`     
`--name $COSMOSDB_ACCOUNT_NAME \`     
`--resource-group learn-b61d7cad-6e55-4a9b-8b5d-e6a0a48dee88 \`      
`--query primaryMasterKey -o tsv)`     
      
`printf "\n\nReplace <STORAGE_CONNECTION_STRING> with:\n$STORAGE_CONNECTION_STRING\n\nReplace <COSMOSDB_CONNECTION_STRING> with:\n$COSMOSDB_CONNECTION_STRING\n\nReplace <COSMOSDB_MASTER_KEY> with:\n$COSMOSDB_MASTER_KEY\n\n"`

 
 >2.Go to where you cloned the application, and open the **start** folder in Visual Studio Code. Open **local.settings.json** in the editor so you can update the file.

>3.In **local.settings.json**, update the variables `AzureWebJobsStorage`, `AzureCosmosDBConnectionString`, and `AzureCosmosDBMasterKey` with the values listed in the Cloud Shell and save the file. The local.settings.json file should only exist on your local computer.

#Run the application

>1.In the Visual Studio Code terminal window, run the following command to install dependencies and set up the database:

`npm install`

>2.Press F5 to start debugging the function app. The function app startup is shown in a terminal window.

>3.To run the web application on your machine, open a second integrated terminal instance and run the following command to start the web app.    

`npm start`

 
<hr>
<hr>


# **Enable Automatic Updates in a web application using SignalR Service**
 
# Create a SignalR account

>1.The first step is to run the following command in the Cloud Shell to create a new SignalR account in the sandbox resource group. This command can take a couple of minutes to complete, so please wait for it to finish before proceeding to the next step.

`SIGNALR_SERVICE_NAME=msl-sigr-signalr$(openssl rand -hex 5) az signalr create \`      
`--name $SIGNALR_SERVICE_NAME \`      
`--resource-group learn-06362b92-41a9-4f72-a1d6-f84c56e45fba \`     
`--sku Free_DS2 \`      
`--unit-count 1`       

>2.For SignalR Service to work properly with Azure Functions, you need to set its service mode to Serverless. Configure the service mode using the following command.

`az resource update \`      
`--resource-type Microsoft.SignalRService/SignalR \`     
`--name $SIGNALR_SERVICE_NAME \`      
`--resource-group learn-06362b92-41a9-4f72-a1d6-f84c56e45fba \`      
`--set properties.features[flag=ServiceMode].value=Serverless`      

# Update local settings

>1.Run the following commands in the Cloud Shell to get the connection strings for the resources we created in this exercise.

`SIGNALR_CONNECTION_STRING=$(az signalr key list \`       
`--name $(az signalr list \`       
`--resource-group learn-06362b92-41a9-4f72-a1d6-f84c56e45fba \`     
`--query [0].name -o tsv) \`     
`--resource-group learn-06362b92-41a9-4f72-a1d6-f84c56e45fba \`     
`--query primaryConnectionString -o tsv)`     

`printf "\n\nReplace <SIGNALR_CONNECTION_STRING> with:\n$SIGNALR_CONNECTION_STRING\n\n"`     


>2.Navigate to where you cloned the application and open the **start** folder in Visual Studio Code. Open **local.settings.json** in the editor so you can update the file.

>3.In **local.settings.json**, update the variable `AzureSignalRConnectionString` with the value listed in the Cloud Shell and save the file.



