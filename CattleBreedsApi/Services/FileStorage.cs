using CattleBreedsApi.Data;
using CattleBreedsApi.Models;

namespace CattleBreedsApi.Services;

public class FileStorage(ApiDbContext dbContext)
{
    public async Task<UploadFile> SaveFileAsync(IFormFile file)
    {
        var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploadDirectory);
        var filePath = BuildFilePath(uploadDirectory, file);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var entity = new UploadFile()
        {
            FilePath = filePath,
            Id = Guid.NewGuid()
        };

        await dbContext.UploadFiles.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    private string BuildFilePath(string directory, IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{Guid.NewGuid()}{extension}";
        return Path.Combine(directory, fileName);
    }
}