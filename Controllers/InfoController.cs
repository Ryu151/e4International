using e4International.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace e4International.Controllers
{
    public class InfoController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Create(InfoViewModel model)
        {
            var filename = "coll.xml";
            XmlSerializer ser = new XmlSerializer(typeof(InfoViewModel));
            TextWriter writer = new StreamWriter(filename);
            ser.Serialize(writer, model);
            return null;
        }
    }
}
