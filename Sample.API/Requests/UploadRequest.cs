namespace Sample.API.Requests;

public class UploadRequest
{
    public string Path { get; set; }
    public IFormFile File { get; set; }
}