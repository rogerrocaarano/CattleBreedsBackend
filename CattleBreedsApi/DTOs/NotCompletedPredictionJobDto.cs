namespace CattleBreedsApi.DTOs;

public class NotCompletedPredictionJobDto
{
    public Guid Id { get; set; }
    public string Status { get; set; } = "InProgress"; // Default status
}