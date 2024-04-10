using Azure;
using Azure.AI.OpenAI;
using Microsoft.Extensions.Options;
using System.Text.Json;
using TravelMakerII.Contracts;
using TravelMakerII.Interfaces;
using TravelMakerII.Models;

namespace TravelMakerII.Services;

public class TravelService : ITravelService
{
    private readonly AzureAICredentials _config;
    public TravelService(IOptions<AzureAICredentials> options)
    {
        _config = options.Value;
    }
    public List<DestinationModel> GetItinerary(ItineraryRequestModel request)
    {
        OpenAIClient client = new(new Uri(_config.Endpoint), new AzureKeyCredential(_config.Key));

        IList<ChatRequestMessage> messages = new List<ChatRequestMessage>();

        messages.Add(new ChatRequestSystemMessage(PromptConstants.ItineraryQuestion));
        string question = $"Destino {request.Name} {request.days} dias";

        messages.Add(new ChatRequestUserMessage(question));

        var chatCompletionsOptions = new ChatCompletionsOptions(_config.DeploymentModel, messages);

        Response<ChatCompletions> response = client.GetChatCompletions(chatCompletionsOptions);

        Console.WriteLine(response.Value.Choices[0].Message.Content);
        var jsonResponse = JsonSerializer.Deserialize<List<DestinationModel>>(response.Value.Choices[0].Message.Content);
        return jsonResponse;
    }

    public PlaceInfomation GetPlaceInformation(PlaceInformationRequestModel request)
    {
        OpenAIClient client = new(new Uri(_config.Endpoint), new AzureKeyCredential(_config.Key));

        IList<ChatRequestMessage> messages = new List<ChatRequestMessage>();

        messages.Add(new ChatRequestSystemMessage(PromptConstants.PlacePrepareQuestion + PromptConstants.PlaceSampleResponse));
        string question = string.Format(PromptConstants.PlaceQuestion, request.PlaceName, request.GeoReference);

        messages.Add(new ChatRequestUserMessage(question));

        var chatCompletionsOptions = new ChatCompletionsOptions(_config.DeploymentModel, messages);

        Response<ChatCompletions> response = client.GetChatCompletions(chatCompletionsOptions);

        Console.WriteLine(response.Value.Choices[0].Message.Content);
        var jsonResponse = DeserializePlaceInformation(response);
        return jsonResponse;
    }
    public PlaceInfomation DeserializePlaceInformation(ChatCompletions response)
    {
        try
        {
            return JsonSerializer.Deserialize<PlaceInfomation>(response.Choices[0].Message.Content);
        }
        catch (JsonException ex)
        {
            // Log or handle the exception as needed
            throw new ApplicationException("An error occurred while deserializing the PlaceInformation.", ex);
        }
    }

}
