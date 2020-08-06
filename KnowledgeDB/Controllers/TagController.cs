using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace KnowledgeDB.Controllers
{
    public class TagController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// Shows Overview of the most used Tags
        /// </summary>
        /// <returns></returns>
        public IActionResult List()
        {
            return View();
        }
    }

}
