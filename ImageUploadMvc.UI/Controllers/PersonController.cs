using Microsoft.AspNetCore.Mvc;

namespace ImageUploadMvc.UI.Controllers;

public class PersonController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
