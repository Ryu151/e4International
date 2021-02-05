using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using e4International.Models;
using System.Xml.Serialization;
using System.IO;
using System.Xml.Linq;

namespace e4International.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            InfoViewModel i;
            var filename = "coll.xml";
            XmlSerializer ser = new XmlSerializer(typeof(InfoViewModel));
            using (Stream reader = new FileStream(filename, FileMode.Open)) 
            {
                i = (InfoViewModel)ser.Deserialize(reader);
            }
            var test = i;
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult Create(InfoViewModel model)
        {
            var filename = "coll.xml";
            XDocument doc = XDocument.Load(filename);
            XElement info = doc.Element("InfoViewModel");
            info.Add(new XElement("Username", model.UserName), new XElement("Surname", model.Surname), new XElement("CellPhone", model.CellPhone));
            doc.Save(filename);
              
            return null;
        }
    }
}
