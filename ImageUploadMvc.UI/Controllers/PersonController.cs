using ImageUploadMvc.Data.Repositories;
using ImageUploadMvc.Data.Shared;
using Microsoft.AspNetCore.Mvc;

namespace ImageUploadMvc.UI.Controllers;

public class PersonController : Controller
{
    private readonly IFileService _fileService;
    private readonly IPersonRepository _personRepo;

    public PersonController(IFileService fileService,IPersonRepository personRepo)
    {
        _fileService = fileService;
        _personRepo = personRepo;
    }
    public async Task<IActionResult> Index()
    {
        return View(await _personRepo.GetPeople());
    }

    public IActionResult AddPerson()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddPerson(Person person)
    {
        if(!ModelState.IsValid)
        {
            return View(person);
        }
        try
        {
            if(person.ImageFile!=null)
            {
                if (person.ImageFile.Length>1*1024*1024)
                {
                    throw new InvalidOperationException("File size can not exceed 1 mb");
                }
                person.ProfilePicture = await _fileService.SaveFile(person.ImageFile, "Images", new string[] { ".jpg", ".jpeg", ".png" });
            }
            await _personRepo.AddPerson(person);
            TempData[nameof(NotificationTypes.SuccessMessage)] = "Saved successfully";
            return RedirectToAction(nameof(AddPerson));
        }
        catch(InvalidOperationException ex)
        {
            TempData[nameof(NotificationTypes.ErrorMessage)] = ex.Message;
            return View(person);
        }
        catch (FileNotFoundException ex)
        {
            TempData[nameof(NotificationTypes.ErrorMessage)] = ex.Message;
            return View(person);
        }
        catch (Exception ex)
        {
            TempData[nameof(NotificationTypes.ErrorMessage)] = "Something went wrong!!!";
            return View(person);
        }
    }


    public async Task<IActionResult> UpdatePerson(int id)
    {
        var person = await _personRepo.GetPersonById(id);
        if(person==null)
        {
            TempData[nameof(NotificationTypes.ErrorMessage)] = "Person does not found";
            return RedirectToAction(nameof(Index));
        }
        return View(person);
    }

    [HttpPost]
    public async Task<IActionResult> UpdatePerson(Person person)
    {
        if (!ModelState.IsValid)
        {
            return View(person);
        }
        try
        {
            string? oldProfilePicture = "";
            if (person.ImageFile != null)
            {
                if (person.ImageFile.Length > 1 * 1024 * 1024)
                {
                    throw new InvalidOperationException("File size can not exceed 1 mb");
                }
                oldProfilePicture = person.ProfilePicture;
                person.ProfilePicture = await _fileService.SaveFile(person.ImageFile, "Images", new string[] { ".jpg", ".jpeg", ".png" });
            }
            await _personRepo.UpdatePerson(person);
            if(!string.IsNullOrWhiteSpace(oldProfilePicture))
            {
                _fileService.DeleteFile(oldProfilePicture, "Images");
            }
            TempData[nameof(NotificationTypes.SuccessMessage)] = "Saved successfully";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData[nameof(NotificationTypes.ErrorMessage)] = ex.Message;
            return View(person);
        }
        catch (FileNotFoundException ex)
        {
            TempData[nameof(NotificationTypes.ErrorMessage)] = ex.Message;
            return View(person);
        }
        catch (Exception ex)
        {
            TempData[nameof(NotificationTypes.ErrorMessage)] = "Something went wrong!!!";
            return View(person);
        }
    }

    public async Task<IActionResult> DeletePerson(int id)
    {
        var person = await _personRepo.GetPersonById(id);
        if (person == null)
        {
            TempData[nameof(NotificationTypes.ErrorMessage)] = "Person does not found";
            return RedirectToAction(nameof(Index));
        }
        await _personRepo.DeletePerson(id);
        if(!string.IsNullOrWhiteSpace(person.ProfilePicture))
        {
            _fileService.DeleteFile(person.ProfilePicture, "Images");
        }
        return RedirectToAction(nameof(Index));
    }
}
