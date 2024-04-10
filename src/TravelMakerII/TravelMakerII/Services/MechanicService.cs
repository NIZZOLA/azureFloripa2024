using Azure.AI.OpenAI;
using Azure;
using System.Text.Json;
using TravelMakerII.Models;
using Microsoft.Extensions.Options;
using TravelMakerII.Interfaces;

namespace TravelMakerII.Services;

public class MechanicService : IMechanicService
{
    private readonly AzureAICredentials _config;
    public MechanicService(IOptions<AzureAICredentials> options)
    {
        _config = options.Value;
    }
    public List<MechanicSolutionModel> GetMechanic(ProblemsRequestModel request)
    {
        OpenAIClient client = new(new Uri(_config.Endpoint), new AzureKeyCredential(_config.Key));

        IList<ChatRequestMessage> messages = new List<ChatRequestMessage>();

        messages.Add(new ChatRequestSystemMessage(PromptConstants.MechanicQuestion));
        messages.Add(new ChatRequestSystemMessage(PromptConstants.JokeAnswer));
        string question = $"tenho um veículo modelo: {request.VehicleModel} com o problema {request.Problem} ";

        messages.Add(new ChatRequestUserMessage(question));

        var chatCompletionsOptions = new ChatCompletionsOptions(_config.DeploymentModel, messages);

        Response<ChatCompletions> response = client.GetChatCompletions(chatCompletionsOptions);

        return JsonSerializer.Deserialize<List<MechanicSolutionModel>>(response.Value.Choices[0].Message.Content);
    }
}
