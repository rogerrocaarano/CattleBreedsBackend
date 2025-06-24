using CattleBreedsApi.Data;
using CattleBreedsApi.Models;

namespace CattleBreedsApi.Services;

public class CattleClassifier(ApiDbContext dbContext)
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
}