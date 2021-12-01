<h2 id="clone-and-open-the-starter-application">Clone and open the starter application</h2>
<ol>
<li><p>Run the following command in Azure Cloud Shell to clone the Git project solution.</p>
<pre><code class="lang-bash">cd ~
git clone https://github.com/MicrosoftDocs/mslearn-connect-services-together.git
</code></pre>
</li>
<li><p>Run the following command to go the start folder in your cloned project and open Cloud Shell editor.</p>
<pre><code class="lang-bash">cd ~/mslearn-connect-services-together/implement-message-workflows-with-service-bus/src/start
code .
</code></pre>
</li>
</ol>
<h2 id="configure-a-connection-string-to-a-service-bus-namespace">Configure a connection string to a Service Bus Namespace</h2>
<p>You must configure two pieces of information in your two console apps to access your Service Bus Namespace and to use the queue within that namespace:</p>
<ul>
<li>Endpoint for your namespace</li>
<li>Shared access key for authentication</li>
</ul>
<p>These values can be obtained from the connection string.</p>
<div class="NOTE">
<p>Note</p>
<p>For simplicity, the following tasks will instruct you to hard-code the connection string in the <strong>Program.cs</strong> file of both console applications. In a production application, you should use a configuration file or Azure Key Vault to store the connection string.</p>
</div>
<p>The following Azure command will return the complete connection string.</p>
<ol>
<li><p>In Cloud Shell, run the following command, replacing <code>&lt;namespace-name&gt;</code> with the Service Bus Namespace that you created in Unit 3.</p>
<pre><code class="lang-azurecli">az servicebus namespace authorization-rule keys list \
    --resource-group &lt;rgn&gt;[sandbox resource group name]&lt;/rgn&gt; \
    --name RootManageSharedAccessKey \
    --query primaryConnectionString \
    --output tsv \
    --namespace-name &lt;namespace-name&gt;
</code></pre>
<p>The last line in the response is the connection string, which contains the endpoint for your namespace and the shared access key. It should resemble the following example:</p>
<pre><code class="lang-C#">Endpoint=sb://example.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=AbCdEfGhIjKlMnOpQrStUvWxYz==
</code></pre>
</li>
<li><p>Copy the connection string from Cloud Shell. You'll need this connection string several times throughout this module, so you might want to save it somewhere handy.</p>
</li>
</ol>
<h2 id="write-code-that-sends-a-message-to-the-queue">Write code that sends a message to the queue</h2>
<ol>
<li><p>In the Cloud Shell editor, open <strong>privatemessagesender/Program.cs</strong> and locate the following line of code.</p>
<pre><code class="lang-C#">const string ServiceBusConnectionString = &quot;&quot;;
</code></pre>
</li>
<li><p>Paste the connection string between the quotation marks.</p>
</li>
</ol>
<p>To complete the component that sends messages about sales, you must add an <code>await</code> operator to suspend evaluation of the async method until the asynchronous operation completes.</p>
<ol>
<li><p>Locate the <code>SendSalesMessageAsync()</code> method.</p>
</li>
<li><p>Within that method, locate the following line of code.</p>
<pre><code class="lang-C#">// Create a Service Bus client here
</code></pre>
</li>
<li><p>Replace that line of code with the following code.</p>
<pre><code class="lang-C#">// By leveraging &quot;await using&quot;, the DisposeAsync method will be called automatically once the client variable goes out of scope. 
// In more realistic scenarios, you would want to store off a class reference to the client (rather than a local variable) so that it can be used throughout your program.

await using var client = new ServiceBusClient(ServiceBusConnectionString);
</code></pre>
</li>
<li><p>Within that method, locate the following line of code.</p>
<pre><code class="lang-C#">// Create a sender here
</code></pre>
</li>
<li><p>Replace that comment with the following code.</p>
<pre><code class="lang-C#">await using ServiceBusSender sender = client.CreateSender(QueueName);
</code></pre>
</li>
<li><p>Within the <code>try...catch</code> block, locate the following line of code.</p>
<pre><code class="lang-C#">// Create and send a message here
</code></pre>
</li>
<li><p>Replace that line of code with the following lines of code.</p>
<pre><code class="lang-C#">string messageBody = $&quot;$10,000 order for bicycle parts from retailer Adventure Works.&quot;;
var message = new ServiceBusMessage(messageBody);
</code></pre>
</li>
<li><p>Insert the following code on a new line directly below what you just added to display the message in the console.</p>
<pre><code class="lang-C#">Console.WriteLine($&quot;Sending message: {messageBody}&quot;);
</code></pre>
</li>
<li><p>Insert the following code on the next line.</p>
<pre><code class="lang-C#">await sender.SendMessageAsync(message);
</code></pre>
</li>
<li><p>Near the end of the file, locate the following comment.</p>
<pre><code class="lang-C#">// Close the connection to the sender here
</code></pre>
</li>
<li><p>Replace that line with the following code to close the connection.</p>
<pre><code class="lang-C#">await queueClient.CloseAsync(); 
</code></pre>
</li>
<li><p>Your final code for <strong>privatemessagesender/Program.cs</strong> should resemble the following example:</p>
<pre><code class="lang-C#">using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace privatemessagesender
{
    class Program
    {
        const string ServiceBusConnectionString = &quot;Endpoint=sb://example.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=AbCdEfGhIjKlMnOpQrStUvWxYz==&quot;;
        const string QueueName = &quot;salesmessages&quot;;

        static void Main(string[] args)
        {
            Console.WriteLine(&quot;Sending a message to the Sales Messages queue...&quot;);
            SendSalesMessageAsync().GetAwaiter().GetResult();
            Console.WriteLine(&quot;Message was sent successfully.&quot;);
        }

        static async Task SendSalesMessageAsync()
        {
            await using var client = new ServiceBusClient(ServiceBusConnectionString);

            await using ServiceBusSender sender = client.CreateSender(QueueName);
            try
            {
                string messageBody = $&quot;$10,000 order for bicycle parts from retailer Adventure Works.&quot;;
                var message = new ServiceBusMessage(messageBody);
                Console.WriteLine($&quot;Sending message: {messageBody}&quot;);
                await sender.SendMessageAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($&quot;{DateTime.Now} :: Exception: {exception.Message}&quot;);
            }
        await queueClient.CloseAsync();    
        }
    }
}
</code></pre>
</li>
<li><p>Save the <strong>privatemessagesender/Program.cs</strong> file using either the <strong>...</strong> icon, or the accelerator key (<kbd>Ctrl+S</kbd> on Windows and Linux, <kbd>Cmd+S</kbd> on macOS).</p>
</li>
</ol>
<h2 id="send-a-message-to-the-queue">Send a message to the queue</h2>
<ol>
<li><p>To run the component that sends a message about a sale, run the following command in Cloud Shell. The first line ensures that you are in the correct path.</p>
<pre><code class="lang-bash"></code></pre>
</li>
</ol>
<p>cd ~/mslearn-connect-services-together/implement-message-workflows-with-service-bus/src/start
dotnet run -p ./privatemessagesender
```</p>
<pre><code>&gt; [!NOTE]
&gt; The first time you run the apps in this exercise, allow for time for `dotnet` to restore packages from remote sources and build the apps.

As the program runs, messages are printed to the console indicating that the app is sending a message. 
</code></pre>
<ol>
<li><p>When the app has finished, run the following command, replacing &lt;namespace-name&gt; with the name of your Service Bus Namespace. This command will return the number of messages that are in the queue.</p>
<pre><code class="lang-azurecli">az servicebus queue show \
    --resource-group &lt;rgn&gt;[sandbox resource group name]&lt;/rgn&gt; \
    --name salesmessages \
    --query messageCount \
    --namespace-name &lt;namespace-name&gt;
</code></pre>
</li>
<li><p>Run the <code>dotnet run</code> command again, and then run the <code>servicebus queue show</code> command again. Each time you run the dotnet app, a new message will be added to the queue. You'll see the <code>messageCount</code> increase each time you run the Azure command.</p>
</li>
</ol>
<h2 id="write-code-that-receives-a-message-from-the-queue">Write code that receives a message from the queue</h2>
<ol>
<li><p>In the editor, open <strong>privatemessagereceiver/Program.cs</strong> and locate the following line of code.</p>
<pre><code class="lang-C#">const string ServiceBusConnectionString = &quot;&quot;;
</code></pre>
</li>
<li><p>Paste the connection string that you saved earlier between the quotation marks.</p>
</li>
<li><p>Locate the <code>ReceiveSalesMessageAsync()</code> method.</p>
</li>
<li><p>Within that method, locate the following line of code.</p>
<pre><code class="lang-C#">// Create a Service Bus client that will authenticate using a connection string
</code></pre>
</li>
<li><p>Replace that line with the following code.</p>
<pre><code class="lang-C#">var client = new ServiceBusClient(ServiceBusConnectionString);
</code></pre>
</li>
<li><p>Locate the following line of code.</p>
<pre><code class="lang-C#">// Create the options to use for configuring the processor
</code></pre>
</li>
<li><p>Replace that line with the following lines of code, which configures message handling options.</p>
<pre><code class="lang-C#">var processorOptions = new ServiceBusProcessorOptions
{
    MaxConcurrentCalls = 1,
    AutoCompleteMessages = false
};
</code></pre>
</li>
<li><p>Locate the following line of code.</p>
<pre><code class="lang-C#">// Create a processor that we can use to process the messages
</code></pre>
</li>
<li><p>Replace that line with the following code to create a processor.</p>
<pre><code class="lang-C#">await using ServiceBusProcessor processor = client.CreateProcessor(QueueName, processorOptions);
</code></pre>
</li>
<li><p>Locate the following line of code.</p>
<pre><code class="lang-C#">// Configure the message and error handler to use
</code></pre>
</li>
<li><p>To configure the handlers, replace that line with the following code.</p>
<pre><code class="lang-C#">processor.ProcessMessagesAsync += MessageHandler;
processor.ProcessErrorAsync += ErrorHandler;
</code></pre>
</li>
<li><p>Locate the following line of code.</p>
<pre><code class="lang-C#">// Start processing
</code></pre>
</li>
<li><p>To start processing, replace that line with the following code.</p>
<pre><code class="lang-C#">await processor.StartProcessingAsync();
</code></pre>
</li>
<li><p>Locate the following line of code.</p>
<pre><code class="lang-C#">// Close the processor here
</code></pre>
</li>
<li><p>To close the connection to Service Bus, replace that line with the following code:</p>
<pre><code class="lang-C#">await processor.CloseAsync();
</code></pre>
</li>
<li><p>Locate the <code>ProcessMessagesAsync()</code> method. You've registered this method as the one that handles incoming messages.</p>
</li>
<li><p>To display incoming messages in the console, replace the code within that method with the following code.</p>
<pre><code class="lang-C#">Console.WriteLine($&quot;Received message: SequenceNumber:{args.Message.SequenceNumber} Body:{args.Message.Body}&quot;);
</code></pre>
</li>
<li><p>To remove the received message from the queue, insert the following code on the next line.</p>
<pre><code class="lang-C#">await args.CompleteMessageAsync(args.Message);
</code></pre>
</li>
<li><p>Your final code for <strong>privatemessagereceiver/Program.cs</strong> should resemble the following example.</p>
<pre><code class="lang-C#">using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace privatemessagereceiver
{
    class Program
    {

        const string ServiceBusConnectionString = &quot;Endpoint=sb://example.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=AbCdEfGhIjKlMnOpQrStUvWxYz==&quot;;
        const string QueueName = &quot;salesmessages&quot;;

        static void Main(string[] args)
        {

            ReceiveSalesMessageAsync().GetAwaiter().GetResult();

        }

        static async Task ReceiveSalesMessageAsync()
        {
            Console.WriteLine(&quot;======================================================&quot;);
            Console.WriteLine(&quot;Press ENTER on the keyboard to exit after receiving all the messages.&quot;);
            Console.WriteLine(&quot;======================================================&quot;);

            var client = new ServiceBusClient(ServiceBusConnectionString);

            var processorOptions = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false
            };

            ServiceBusProcessor processor = client.CreateProcessor(QueueName, processorOptions);

            await processor.StartProcessingAsync();

            Console.Read();

            // Since we didn't use the &quot;await using&quot; syntax here, we need to explicitly dispose the processor and client
            await processor.DisposeAsync();
            await client.DisposeAsync();
        }

        static async Task MessageHandler(ProcessMessageEventArgs args)
        {
            Console.WriteLine($&quot;Received message: SequenceNumber:{args.Message.SequenceNumber} Body:{args.Message.Body}&quot;);
            await args.CompleteMessageAsync(args.Message);
        }

        static Task ErrorHandler(ProcessErrorEventArgs args)
        {
            Console.WriteLine($&quot;Message handler encountered an exception {args.Exception}.&quot;);
            Console.WriteLine(&quot;Exception context for troubleshooting:&quot;);
            Console.WriteLine($&quot;- Endpoint: {args.FullyQualifiedNamespace}&quot;);
            Console.WriteLine($&quot;- Entity Path: {args.EntityPath}&quot;);
            Console.WriteLine($&quot;- Executing Action: {args.ErrorSource}&quot;);
            return Task.CompletedTask;
        }   
    }
}
</code></pre>
</li>
<li><p>Save the file either through the <strong>☰</strong> menu, or the accelerator key (<kbd>Ctrl+S</kbd> on Windows and Linux, <kbd>Cmd+S</kbd> on macOS).</p>
</li>
</ol>
<h2 id="retrieve-a-message-from-the-queue">Retrieve a message from the queue</h2>
<ol>
<li><p>To run the component that receives a message about a sale, run this command in Cloud Shell.</p>
<pre><code class="lang-bash">dotnet run -p privatemessagereceiver
</code></pre>
</li>
<li><p>Check the notifications in Cloud Shell and in the Azure portal, navigate to your Service Bus Namespace and check your Messages chart.</p>
</li>
<li><p>When you see that the messages have been received in the Cloud Shell, press <kbd>Enter</kbd> to stop the app. Then, run the following code to confirm that all of the messages have been removed from the queue, remembering to replace &lt;namespace-name&gt; with your Service Bus Namespace.</p>
<pre><code class="lang-azurecli">az servicebus queue show \
    --resource-group &lt;rgn&gt;[sandbox resource group name]&lt;/rgn&gt; \
    --name salesmessages \
    --query messageCount \
    --namespace-name &lt;namespace-name&gt;
</code></pre>
<p>The output will be <code>0</code> if all the messages have been removed.</p>
</li>
</ol>
<p>You've written code that sends a message about individual sales to a Service Bus queue. In the sales force distributed application, you should write this code in the mobile app that sales personnel use on devices.</p>
<p>You've also written code that receives a message from the Service Bus queue. In the sales force distributed application, you should write this code in the web service that runs in Azure and processes received messages.</p>
