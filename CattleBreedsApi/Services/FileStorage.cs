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