using Microsoft.AspNetCore.Mvc;

namespace animal_shelter_app.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
