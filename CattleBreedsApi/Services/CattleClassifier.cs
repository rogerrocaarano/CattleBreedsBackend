using CattleBreedsApi.Data;
using CattleBreedsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CattleBreedsApi.Services;

public class CattleClassifier(ApiDbContext dbContext, CattleClassifierApi cattleClassifierApi)
{
    public async Task<CattlePredictionJob> CreatePredictionJob(List<UploadFile> files)
    {
        var job = new CattlePredictionJob
        {
            Id = Guid.NewGuid(),
            UploadFiles = files,
            Processed = false,
            CreatedAt = DateTime.UtcNow
        };

        await dbContext.CattlePredictionJobs.AddAsync(job);
        await dbContext.SaveChangesAsync();
        return job;
    }

    public async Task ExecutePredictionJob(Guid id)
    {
        var job = await dbContext.CattlePredictionJobs
            .Include(job => job.UploadFiles)
            .FirstOrDefaultAsync(job => job.Id == id);
        if (job == null)
        {
            throw new Exception("Job not found");
        }

        if (job.Processed)
        {
            throw new Exception("Job already processed");
        }

        var predictions = new List<CattlePrediction>();

        foreach (var file in job.UploadFiles)
        {
            var prediction = await cattleClassifierApi.ClassifyImage(file.Id);
            predictions.Add(prediction);
        }

        // getting the best prediction based on confidence
        var bestPrediction = ValidPrediction(predictions);
        
        job.Breed = bestPrediction?.Breed;
        job.Confidence = bestPrediction?.Confidence ?? 0.0f;
        job.Processed = true;
        job.ProcessedAt = DateTime.UtcNow;
        job.BestResultImageId = bestPrediction?.UploadFileId;
        job.Weight = bestPrediction?.Weight ?? 0.0f;
        dbContext.CattlePredictionJobs.Update(job);
        await dbContext.SaveChangesAsync();
    }

    private CattlePrediction? ValidPrediction(List<CattlePrediction> predictions)
    {
        const float requiredValidSupport = 0.3f; // Al menos el 30% deben respaldar
        const float confidenceThreshold = 80f;

        if (predictions.Count == 0)
            return null;

        // Ordenar por confianza descendente
        var sorted = predictions.OrderByDescending(p => p.Confidence).ToList();
        var best = sorted.First();

        // Contar cuántas predicciones cumplen con:
        // - misma raza
        // - confianza ≥ 80%
        var supportCount = predictions.Count(p =>
            p.Breed == best.Breed && p.Confidence >= confidenceThreshold);

        var requiredCount = Math.Max(1, (int)(predictions.Count * requiredValidSupport));

        return supportCount >= requiredCount ? best : null;
    }


    public async Task<CattlePredictionJob?> GetPredictionJob(Guid id)
    {
        return await dbContext.CattlePredictionJobs
            .Include(job => job.BestResultImage)
            .FirstOrDefaultAsync(job => job.Id == id);
    }
}