using Microsoft.AspNetCore.Mvc;

namespace UCLoan.Controllers
{
    public class LoanController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
