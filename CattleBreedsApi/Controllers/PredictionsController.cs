using CattleBreedsApi.DTOs;
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

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPredictionJob(Guid id)
    {
        var job = await classifier.GetPredictionJob(id);
        if (job == null)
        {
            return NotFound();
        }

        if (!job.Processed)
        {
            return Ok(new NotCompletedPredictionJobDto
            {
                Id = job.Id,
                Status = "InProgress"
            });
        }

        if (job.Breed == null || job.BestResultImageId == null)
        {
            return Ok(new NotCompletedPredictionJobDto
            {
                Id = job.Id,
                Status = "NotValidResult"
            });
        }

        return Ok(new CompletedPredictionJobDto
        {
            Id = job.Id,
            Breed = job.Breed,
            Confidence = job.Confidence,
            ImageId = (Guid)job.BestResultImageId!,
            Status = "ValidResult",
            Weight = job.Weight
        });
    }

    [HttpGet("image/{fileId}")]
    public async Task<IActionResult> GetImage(Guid fileId)
    {
        var fileData = await fileStorage.GetFile(fileId);
        if (fileData == null)
        {
            return NotFound();
        }

        return File(fileData.Content, fileData.ContentType, fileData.FileName);
    }
}