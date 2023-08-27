using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace app
{
    public class SecretController : Controller
    {
        [Authorize(Policy = "OnlyAdmins")]
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Oops() => Index();
    }
}