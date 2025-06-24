using CattleBreedsApi.Models;
using CattleBreedsApi.Services;
using Hangfire;
using Microsoft.AspNetCore.Mvc;

namespace CattleBreedsApi.Controllers;

[ApiController]
[Route("[controller]")]
public class PredictionsController(
    FileStorage fileStorage,
    CattleClassifier classifier,
    IBackgroundJobClient backgroundJobs
) : ControllerBase
{
    [HttpPost("classify")]
    public async Task<IActionResult> ClassifyImages(List<IFormFile> files)
    {
        if (files.Count == 0)
        {
            return BadRequest("No files uploaded.");
        }

        var uploads = await UploadImages(files);
        var job = await classifier.CreatePredictionJob(uploads);
        backgroundJobs.Enqueue(() => classifier.ExecutePredictionJob(job.Id));

        return Ok(job.Id);
    }

    private async Task<List<UploadFile>> UploadImages(List<IFormFile> files)
    {
        var uploads = new List<UploadFile>();

        foreach (var file in files)
        {
            var uploadedFile = await fileStorage.SaveFileAsync(file);
            uploads.Add(uploadedFile);
        }

        return uploads;
    }
}