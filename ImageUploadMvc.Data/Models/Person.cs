using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ImageUploadMvc.Data.Models;

public class Person
{
    public int Id { get; set; }
    [Required,MaxLength(20)]
    public string? FirstName { get; set; }
    [Required,MaxLength(20)]
    public string? LastName { get; set; }
    public string? ProfilePicture { get; set; }

    public IFormFile? ImageFile { get; set; }

}
