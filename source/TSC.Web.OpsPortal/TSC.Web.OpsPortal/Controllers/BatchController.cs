using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TSC.Web.OpsPortal.Controllers
{
    public class BatchController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
