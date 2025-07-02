namespace CattleBreedsApi.DTOs;

public class CompletedPredictionJobDto
{
    public Guid Id { get; set; }
    public string? Breed { get; set; }
    public float Confidence { get; set; }
    public Guid ImageId { get; set; }
    public string Status { get; set; }
    public float Weight { get; set; }
}