using CattleBreedsApi.Data;
using CattleBreedsApi.Models;
using Microsoft.EntityFrameworkCore;

namespace CattleBreedsApi.Services;

public class FileStorage(ApiDbContext dbContext)
{
    public async Task<FileData?> GetFile(Guid fileId)
    {
        var filePath = await dbContext.UploadFiles
            .Where(f => f.Id == fileId)
            .Select(f => f.FilePath)
            .FirstOrDefaultAsync();
        if (filePath == null || !File.Exists(filePath))
        {
            return null;
        }

        return new FileData()
        {
            Content = new FileStream(filePath, FileMode.Open, FileAccess.Read),
            ContentType = "application/octet-stream",
            FileName = Path.GetFileName(filePath)
        };
    }

    public async Task<UploadFile> SaveFileAsync(IFormFile file)
    {
        var uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "uploads");
        Directory.CreateDirectory(uploadDirectory);
        var fileId = Guid.NewGuid();
        var filePath = BuildFilePath(fileId, uploadDirectory, file);
        await using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var entity = new UploadFile()
        {
            FilePath = filePath,
            Id = fileId,
            Timestamp = DateTime.UtcNow
        };

        await dbContext.UploadFiles.AddAsync(entity);
        await dbContext.SaveChangesAsync();
        return entity;
    }

    private string BuildFilePath(Guid id, string directory, IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName);
        var fileName = $"{id}{extension}";
        return Path.Combine(directory, fileName);
    }

    public async Task<FileStream> GetFileStreamAsync(Guid fileId)
    {
        var file = await dbContext.UploadFiles.FindAsync(fileId);
        if (file == null)
        {
            throw new FileNotFoundException("File not found", fileId.ToString());
        }

        return new FileStream(file.FilePath, FileMode.Open, FileAccess.Read);
    }
}