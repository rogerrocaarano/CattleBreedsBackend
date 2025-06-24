namespace CattleBreedsApi.Models;

public class CattlePrediction
{
    public Guid Id { get; set; }
    public Guid UploadFileId { get; set; }
    public required string Breed { get; set; }
    public float Confidence { get; set; }
}