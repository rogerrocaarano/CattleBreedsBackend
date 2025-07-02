namespace CattleBreedsApi.Models;

public class FileData
{
    public Stream Content { get; set; } // o ReadOnlyStream
    public string ContentType { get; set; }
    public string FileName { get; set; }
}