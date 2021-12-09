Create a storage account with the Azure CLI
 Tip
Normally, you'd start a new project by creating a resource group to hold all the associated resources. In this case, we'll be using the Azure sandbox which provides a resource group named learn-fe63b111-d6f6-4766-92e9-c8b6baee9948.
Use the az storage account create command to create the storage account. You can enter the command into the Cloud Shell window on the right.
The command needs several parameters:
Parameter	Value
--name	Sets the name. Remember that storage accounts use the name to generate a public URL - so it has to be unique. In addition, the account name must be between 3 and 24 characters, and be composed of numbers and lowercase letters only. We recommend you use the prefix articles with a random number suffix but you can use whatever you like.
-g	Supplies the Resource Group. Use learn-fe63b111-d6f6-4766-92e9-c8b6baee9948 as the value.
--kind	Sets the Storage Account type: StorageV2 to create a general-purpose V2.account.
--sku	Sets the Replication and Storage type. It defaults to Standard_RAGRS. Let's use Standard_LRS, which means it's only locally redundant within the datacenter.
-l	Sets the Location independent of the resource group owner. It's optional, but you can use it to place the queue in a different region than the resource group. Place it close to you, choosing from the following list of available regions in the sandbox.
The free sandbox allows you to create resources in a subset of the Azure global regions. Select a region from this list when you create resources:
westus2
southcentralus
centralus
eastus
westeurope
southeastasia
japaneast
brazilsouth
australiasoutheast
centralindia
Here's an example command line that uses these parameters. Make sure to change the --name parameter.
Azure CLI

Copy
az storage account create --name [unique-name] -g learn-fe63b111-d6f6-4766-92e9-c8b6baee9948 --kind StorageV2 --sku Standard_LRS
![image](https://user-images.githubusercontent.com/63580515/145407780-e2be9a26-288c-46db-bb66-1c167355fe2b.png)
