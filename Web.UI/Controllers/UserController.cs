using Microsoft.AspNetCore.Mvc;
using Web.UI.Models;

namespace Web.UI.Controllers
{
    public class UserController : Controller
    {
        public IActionResult Detail()
        {
            UserDetailVM model = new UserDetailVM();

            return View(model);
        }

        public IActionResult Info()
        {
            UserInfoVM model = new UserInfoVM()
            {
                Name = "Ali",
                Lastname = "Veli",
                JobTitle = "Developer"
            };

            return PartialView(model);
        }
    }
}
