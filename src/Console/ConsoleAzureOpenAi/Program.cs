using Azure.AI.OpenAI;
using Azure;
using static System.Environment;
using System.Threading.Tasks.Dataflow;
using ConsoleAzureOpenAi;

OpenAIClient client = new(new Uri(OpenAiCredentials.Endpoint), new AzureKeyCredential(OpenAiCredentials.Key));

IList<ChatRequestMessage> messages = new List<ChatRequestMessage>();

messages.Add(new ChatRequestSystemMessage(PromptConstants.TravelPrompt));  
string question = string.Empty;
do
{
    Console.Write("Digite sua pergunta:");
    question = Console.ReadLine();
    if (question != string.Empty)
    {
        messages.Add(new ChatRequestUserMessage(question));

        var chatCompletionsOptions = new ChatCompletionsOptions(OpenAiCredentials.DeploymentName, messages);
        Response<ChatCompletions> response = client.GetChatCompletions(chatCompletionsOptions);
        
        Console.WriteLine(response.Value.Choices[0].Message.Content);
        messages.Add(new ChatRequestAssistantMessage(response.Value.Choices[0].Message.Content));
    }
} while (!String.IsNullOrEmpty(question));
