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
using System.Xml;

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
            var view = new Entries();
            
            IEnumerable<Entry> i;
            var filename = "coll2.xml";
            XmlSerializer ser = new XmlSerializer(typeof(Entries));
            using (Stream reader = new FileStream(filename, FileMode.Open)) 
            {
               var x = ser.Deserialize(reader);
                view = (Entries)x;
            }

            var xy = view.Entry.Select(i => new Entry()
            {
                Id = i.Id,
                CellPhone = i.CellPhone,
                Surname = i.Surname,
                UserName = i.UserName
            }).ToList();

            return PartialView("~/Views/PartialViews/_Entries.cshtml", xy);
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
        public IActionResult Create(Entry model)
        {

            var filename = "coll2.xml";
            if (!System.IO.File.Exists(filename))
            {
                XDocument xDoc = new XDocument();
                XElement root1 = new XElement("Entries");
                xDoc.Add(root1);
                xDoc.Save(filename);
            };

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);
            var dec = xmlDoc.GetElementsByTagName("Entry").Count;

            var entry = xmlDoc.CreateElement("Entry");
            dec += 1;
            entry.SetAttribute("Id", dec.ToString());

            var username = xmlDoc.CreateElement("UserName");
            username.InnerText = model.UserName;
            var surname = xmlDoc.CreateElement("Surname");
            surname.InnerText = model.Surname;
            var cellphone = xmlDoc.CreateElement("CellPhone");
            cellphone.InnerText = model.CellPhone;

            entry.AppendChild(username);
            entry.AppendChild(surname);
            entry.AppendChild(cellphone);
            var root = xmlDoc.DocumentElement;
            root.AppendChild(entry);
            xmlDoc.Save(filename);

            return null;
        }
    }
}
