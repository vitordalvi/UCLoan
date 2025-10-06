using Microsoft.AspNetCore.Mvc;

namespace UCLoan.Controllers
{
    public class EquipmentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
