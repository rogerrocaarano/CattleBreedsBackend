namespace CattleBreedsApi.Models;

public class CattlePredictionJob
{
    public Guid Id { get; set; }
    public bool Processed { get; set; }
    public string? Breed { get; set; }
    public float Confidence { get; set; }
    public ICollection<UploadFile> UploadFiles { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ProcessedAt { get; set; }
}