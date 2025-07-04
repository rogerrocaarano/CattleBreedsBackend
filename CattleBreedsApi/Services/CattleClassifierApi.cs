using System.Net.Http.Headers;
using CattleBreedsApi.Data;
using CattleBreedsApi.Models;

namespace CattleBreedsApi.Services;

public class CattleClassifierApi(
    ApiDbContext dbContext,
    IHttpClientFactory httpClientFactory,
    FileStorage fileStorage,
    string baseUrl
)
{
    public async Task<CattlePrediction> ClassifyImage(Guid imageId)
    {
        // read file
        var fileStream = await fileStorage.GetFileStreamAsync(imageId);
        // create a new HttpClient
        var httpClient = httpClientFactory.CreateClient();
        httpClient.BaseAddress = new Uri(baseUrl);
        httpClient.DefaultRequestHeaders.Accept.Clear();
        httpClient.DefaultRequestHeaders.Accept.Add(
            new MediaTypeWithQualityHeaderValue("application/json"));
        // build the request
        var content = new MultipartFormDataContent();
        content.Add(new StreamContent(fileStream), "file", fileStream.Name);

        var response = await httpClient.PostAsync("/classify", content);

        if (!response.IsSuccessStatusCode)
        {
            // throw new Exception($"Error classifying image: {response.ReasonPhrase}");
            return new CattlePrediction()
            {
                Breed = "BadRequest",
                Confidence = 0.0f,
                Id = Guid.NewGuid(),
                UploadFileId = imageId
            };
        }

        var responseContent = await response.Content.ReadAsStringAsync();
        // deserialize the response, json properties: breed, confidence
        var result = System.Text.Json.JsonSerializer.Deserialize<CattlePrediction>(
            responseContent,
            new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true }
        );
        if (result == null)
        {
            throw new Exception("Failed to deserialize classification result.");
        }

        result.Id = Guid.NewGuid();
        result.UploadFileId = imageId;

        // save the result to the database
        await dbContext.CattlePredictions.AddAsync(result);
        await dbContext.SaveChangesAsync();
        return result;
    }
}