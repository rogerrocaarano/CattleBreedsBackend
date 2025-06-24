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
            Processed = false
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
        var bestPrediction = predictions
            .OrderByDescending(p => p.Confidence)
            .ToList()
            .First();
        job.Breed = bestPrediction.Confidence < 30 ? null : bestPrediction.Breed;
        job.Confidence = bestPrediction.Confidence;
        job.Processed = true;
        dbContext.CattlePredictionJobs.Update(job);
        await dbContext.SaveChangesAsync();
    }
}