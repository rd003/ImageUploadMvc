using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace ImageUploadMvc.Data.Shared;

public interface IFileService
{
    void DeleteFile(string fileName, string directory);
    Task<string> SaveFile(IFormFile file, string directory, string[] allowedExtensions);
}

public class FileService : IFileService
{
    private readonly IWebHostEnvironment _webHostEnvironment;
    public FileService(IWebHostEnvironment webHostEnvironment)
    {
        _webHostEnvironment = webHostEnvironment;
    }

    public async Task<string> SaveFile(IFormFile file, string directory, string[] allowedExtensions)
    {
        var wwwPath = _webHostEnvironment.WebRootPath;
        var path = Path.Combine(wwwPath, directory); // wwwroot/images
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }
        // check the extesion is allowed or not
        var extension = Path.GetExtension(file.FileName);
        if (!allowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"Only {string.Join(",", allowedExtensions)} extensions are allowed");
        }
        // create the unique file name
        var newFileName = $"{Guid.NewGuid()}{extension}";  //asdfasd-sdfjadksfj-dda.jpeg
        // create fullPath = path+newFileName
        var fullPath = Path.Combine(path, newFileName);
        // create a filestream
        using var fileStream = new FileStream(fullPath, FileMode.Create);
        await file.CopyToAsync(fileStream);
        return newFileName;
    }


    public void DeleteFile(string fileName, string directory)
    {
        var fullPath = Path.Combine(_webHostEnvironment.WebRootPath, directory, fileName);
        if (!Path.Exists(fullPath))
        {
            throw new FileNotFoundException($"File {fileName} does not exists");
        }
        File.Delete(fullPath);
    }
}
