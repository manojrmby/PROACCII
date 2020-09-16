using PROACCII.BL.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROACCII.Controllers
{
    [CheckSessionTimeOut]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        // GET: Home
        public ActionResult Home()
        {
            return View();
        }
    }
}