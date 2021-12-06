using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Admin.Controllers
{
    public class BOMController : Controller
    {
        // GET: BOM
        public ActionResult BOM_AddList()
        {
            return View();
        }
    }
}