# Create functions

In the following steps, you will add an Azure Function app to Azure API Management. Later, you will add a second function app to the same API Management instance to create a single serverless API from multiple functions. Let's start by using a script to create the functions:

>1.To clone the functions project, run the following command in Azure Cloud Shell on the right.

`git clone https://github.com/MicrosoftDocs/mslearn-apim-and-functions.git ~/OnlineStoreFuncs`

>2.Run the following commands in Cloud Shell to set up the necessary Azure resources we need for this exercise.

`cd ~/OnlineStoreFuncs        
bash setup.sh`

The `setup.sh` script creates the two function apps in the sandbox resource group that we've activated for this module. As the following graphic illustrates, each app hosts a single function - `OrderDetails` and `ProductDetails`. The script also sets up a storage account for the functions. The functions both have URLs in the **azurewebsites.net** domain. The function names include random numbers for uniqueness. The script takes a few minutes to complete.

<hr>

# Test the product details function

Now, let's test the ProductDetails function to see how it behaves before we add it to API Management.

>1.Sign in to the [Azure portal](https://portal.azure.com/learn.docs.microsoft.com) using the same account that you used to activate the sandbox.       

>2.On the Azure portal menu or from the Home page, select All resources. The All resources pane appears.

>3.Select the Function App whose name begins with ProductFunction. The Function App pane appears.

>4.In the Function App menu, under Functions, select Functions. The Functions pane appears for your Function App.





