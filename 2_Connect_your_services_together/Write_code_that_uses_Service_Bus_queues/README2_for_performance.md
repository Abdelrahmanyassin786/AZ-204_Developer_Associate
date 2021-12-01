<p>You have decided to use an Azure Service Bus topic to distribute sales performance messages in your salesforce application. Sales personnel will use the app on their mobile devices to send messages that summarize sales figures for each area and time period. Those messages will be distributed to web services located in the company's operational regions, including the Americas and Europe.</p>
<p>You have already implemented the necessary infrastructure in your Azure subscriptions for the topic. Now, you want to write the code that sends messages to the topic and retrieves messages from a subscription.</p>
<p>Make sure you are working in the correct directory by running the following commands in the Azure Cloud Shell:</p>
<pre><code class="lang-bash">cd ~/mslearn-connect-services-together/implement-message-workflows-with-service-bus/src/start
code .
</code></pre>
<h2 id="write-code-that-sends-a-message-to-the-topic">Write code that sends a message to the topic</h2>
<p>To complete the component that sends messages about sales performance, follow these steps:</p>
<ol>
<li><p>In the Azure Cloud Shell editor, open <strong>performancemessagesender/Program.cs</strong> and locate the following line of code.</p>
<pre><code class="lang-C#">const string ServiceBusConnectionString = &quot;&quot;;
</code></pre>
<p>Paste the connection string that you saved in the previous exercise between the quotation marks.</p>
</li>
<li><p>Locate the <code>SendPerformanceMessageAsync()</code> method. (Hint: located at or near line 26.)</p>
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
<li><p>Replace that line of code with the following code.</p>
<pre><code class="lang-C#">await using ServiceBusSender sender = client.CreateSender(TopicName);
</code></pre>
</li>
<li><p>Within the <code>try...catch</code> block, locate the following line of code.</p>
<pre><code class="lang-C#">// Create and send a message here
</code></pre>
</li>
<li><p>Replace that line of code with the following code.</p>
<pre><code class="lang-C#">string messageBody = &quot;Total sales for Brazil in August: $13m.&quot;;
var message = new ServiceBusMessage(messageBody);
</code></pre>
</li>
<li><p>To display the message in the console, insert the following code on the next line.</p>
<pre><code class="lang-C#">Console.WriteLine($&quot;Sending message: {messageBody}&quot;);
</code></pre>
</li>
<li><p>To send the message to the topic, insert the following code on the next line.</p>
<pre><code class="lang-C#">await sender.SendMessageAsync(message);
</code></pre>
</li>
<li><p>Your final code should resemble the following example:</p>
<pre><code class="lang-C#">using System;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace performancemessagesender
{
    class Program
    {
        const string ServiceBusConnectionString = &quot;Endpoint=sb://example.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=AbCdEfGhIjKlMnOpQrStUvWxYz==&quot;;
        const string TopicName = &quot;salesperformancemessages&quot;;

        static void Main(string[] args)
        {
            Console.WriteLine(&quot;Sending a message to the Sales Performance topic...&quot;);
            SendPerformanceMessageAsync().GetAwaiter().GetResult();
            Console.WriteLine(&quot;Message was sent successfully.&quot;);
        }

        static async Task SendPerformanceMessageAsync()
        {
            // By leveraging &quot;await using&quot;, the DisposeAsync method will be called automatically once the client variable goes out of scope.
            // In more realistic scenarios, you would want to store off a class reference to the client (rather than a local variable) so that it can be used throughout your program.
            await using var client = new ServiceBusClient(ServiceBusConnectionString);

            await using ServiceBusSender sender = client.CreateSender(TopicName);

            try
            {
                string messageBody = &quot;Total sales for Brazil in August: $13m.&quot;;
                var message = new ServiceBusMessage(messageBody);
                Console.WriteLine($&quot;Sending message: {messageBody}&quot;);
                await sender.SendMessageAsync(message);
            }
            catch (Exception exception)
            {
                Console.WriteLine($&quot;{DateTime.Now} :: Exception: {exception.Message}&quot;);
            }
        }
    }
}
</code></pre>
</li>
<li><p>Save the file using the editor's <strong>☰</strong> menu, or the accelerator key (<kbd>Ctrl+S</kbd> on Windows and Linux, <kbd>Cmd+S</kbd> on macOS).</p>
</li>
</ol>
<h2 id="send-a-message-to-the-topic">Send a message to the topic</h2>
<ol>
<li><p>To run the component that sends a message about a sale, run the following command in Cloud Shell.</p>
<pre><code class="lang-bash">dotnet run -p performancemessagesender
</code></pre>
<p>As the program executes, you'll see notifications in the Azure Cloud Shell indicating that it's sending a message. Each time you run the app, one more message will be added to the topic and each subscriber will receive a copy.</p>
</li>
<li><p>When you see <strong>Message was sent successfully</strong>, run the following command to see how many messages are in the Americas subscription. Remember to replace &lt;namespace-name&gt; with your Service Bus Namespace.</p>
<pre><code class="lang-azurecli">az servicebus topic subscription show \
    --resource-group &lt;rgn&gt;[sandbox resource group name]&lt;/rgn&gt; \
    --namespace-name &lt;namespace-name&gt; \
    --topic-name salesperformancemessages \
    --name Americas \
    --query messageCount
</code></pre>
<p>If you replace <code>Americas</code> with <code>EuropeAndAsia</code>, and run the command again, you should see that both subscriptions have the same number of messages.</p>
</li>
</ol>
<h2 id="write-code-that-receives-a-message-from-a-topic-subscription">Write code that receives a message from a topic subscription</h2>
<p>To complete the component that retrieves messages about sales performance, follow these steps:</p>
<ol>
<li><p>In the editor, open <strong>performancemessagereceiver/Program.cs</strong> and locate the following line of code:</p>
<pre><code class="lang-C#">const string ServiceBusConnectionString = &quot;&quot;;
</code></pre>
<p>Paste the connection string that you saved in the previous exercise between the quotation marks.</p>
</li>
<li><p>Locate the <code>MainAsync()</code> method.</p>
</li>
<li><p>Within that method, locate the following line of code.</p>
<pre><code class="lang-C#">// Create a Service Bus client that will authenticate using a connection string
</code></pre>
</li>
<li><p>To create a Service Bus client, replace that line with the following code.</p>
<pre><code class="lang-C#">var client = new ServiceBusClient(ServiceBusConnectionString);
</code></pre>
</li>
<li><p>Locate the following line of code:</p>
<pre><code class="lang-C#">// Create the options to use for configuring the processor
</code></pre>
</li>
<li><p>To configure message handling options, replace that line with the following code.</p>
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
<li><p>To create a processor, replace that line with the following code.</p>
<pre><code class="lang-C#">ServiceBusProcessor processor = client.CreateProcessor(TopicName, SubscriptionName, processorOptions);
</code></pre>
</li>
<li><p>Locate the following line of code.</p>
<pre><code class="lang-C#">// Configure the message and error handler to use
</code></pre>
</li>
<li><p>To configure the handler, replace that line with the following code.</p>
<pre><code class="lang-C#">processor.ProcessMessageAsync += MessageHandler;
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
<li><p>Locate the <code>MessageHandler()</code> method. You have registered this method to handle incoming messages.</p>
</li>
<li><p>To display incoming messages in the console, replace all the code within that method with the following code.</p>
<pre><code class="lang-C#">Console.WriteLine($&quot;Received message: SequenceNumber:{args.Message.SequenceNumber} Body:{args.Message.Body}&quot;);
</code></pre>
</li>
<li><p>To remove the received message from the subscription, on the next line, add the following code.</p>
<pre><code class="lang-C#">await args.CompleteMessageAsync(args.Message);
</code></pre>
</li>
<li><p>Your final code should resemble the following example.</p>
<pre><code class="lang-C#">using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;

namespace performancemessagereceiver
{
    class Program
    {
        const string ServiceBusConnectionString = &quot;Endpoint=sb://alexgeddyneil.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=LIWIyxs8baqQ0bRf5zJLef6OTfrv0kBEDxFM/ML37Zs=&quot;;
        const string TopicName = &quot;salesperformancemessages&quot;;
        const string SubscriptionName = &quot;Americas&quot;;

        static void Main(string[] args)
        {
            MainAsync().GetAwaiter().GetResult();
        }

        static async Task MainAsync()
        {
            var client = new ServiceBusClient(ServiceBusConnectionString);

            Console.WriteLine(&quot;======================================================&quot;);
            Console.WriteLine(&quot;Press ENTER key to exit after receiving all the messages.&quot;);
            Console.WriteLine(&quot;======================================================&quot;);

            var processorOptions = new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 1,
                AutoCompleteMessages = false
            };

            ServiceBusProcessor processor = client.CreateProcessor(TopicName, SubscriptionName, processorOptions);

            processor.ProcessMessageAsync += MessageHandler;
            processor.ProcessErrorAsync += ErrorHandler;

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
<li><p>Save the file using either the <strong>☰</strong> menu, or the accelerator key (<kbd>Ctrl+S</kbd> on Windows and Linux, <kbd>Cmd+S</kbd> on macOS).</p>
</li>
</ol>
<h2 id="retrieve-a-message-from-a-topic-subscription">Retrieve a message from a topic subscription</h2>
<ol>
<li><p>To run the component that retrieves a message about sales performance, run the following command.</p>
<pre><code class="lang-bash">dotnet run -p performancemessagereceiver
</code></pre>
</li>
<li><p>When the program has returned notifications that it is receiving messages, press <kbd>Enter</kbd> to stop the app. Then, run the following command to confirm that there are zero remaining messages in the <code>Americas</code> subscription. Be sure to replace &lt;namespace-name&gt; with your Service Bus Namespace.</p>
<pre><code class="lang-azurecli">az servicebus topic subscription show \
    --resource-group &lt;rgn&gt;[sandbox resource group name]&lt;/rgn&gt; \
    --namespace-name &lt;namespace-name&gt; \
    --topic-name salesperformancemessages \
    --name Americas \
    --query messageCount
</code></pre>
</li>
<li><p>If you replace <code>Americas</code> with <code>EuropeAndAsia</code>, you'll see that the message count has not changed. The application only received messages from the <code>Americas</code> subscription.</p>
</li>
</ol>
