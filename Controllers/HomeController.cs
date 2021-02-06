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

        [HttpGet]
        public IActionResult Create()
        {
            return PartialView("~/Views/PartialViews/_CreateEntry.cshtml", new Entry());
        }
        [HttpPost]
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

            var count = xmlDoc.GetElementsByTagName("Entry").Count;
            var item = Int32.Parse(xmlDoc.GetElementsByTagName("Entry").Item(count - 1).Attributes.GetNamedItem("Id").InnerText.Remove(0,1));
            item += 1;
            var entry = xmlDoc.CreateElement("Entry");
            
            entry.SetAttribute("Id", "_" + item.ToString());

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

        [HttpGet]
        public IActionResult Delete(string id)
        {
            var filename = "coll2.xml";
            XDocument xmlDoc = XDocument.Load(filename);
            XElement element = (from el in xmlDoc.Root.Elements("Entry")
                                           where (string)el.Attribute("Id") == id
                                           select el).FirstOrDefault();
            element.Remove();
            xmlDoc.Save(filename);
            return null;
        }


            [HttpPost]
        public IActionResult Delete(Entry entry)
        {
            var filename = "coll2.xml";
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);
            var dec = xmlDoc.GetElementById(entry.Id);
            return null;
        }

        [HttpGet]
        public IActionResult Edit(string id)
        {
            var filename = "coll2.xml";
            XDocument xmlDoc = XDocument.Load(filename);
            Entry element = (from el in xmlDoc.Root.Elements("Entry")
                                where (string)el.Attribute("Id") == id
                                select new Entry { 
                                    Id = el.Attribute("Id").Value,
                                    Surname = el.Element("Surname").Value,
                                    CellPhone = el.Element("CellPhone").Value,
                                    UserName = el.Element("UserName").Value
                                }).FirstOrDefault();
            return PartialView("~/Views/PartialViews/_EditEntry.cshtml",element);
        }

        [HttpPost]
        public IActionResult Edit(Entry model)
        {
            var filename = "coll2.xml";
            XDocument xmlDoc = XDocument.Load(filename);
            XElement element = (from el in xmlDoc.Root.Elements("Entry")
                             where (string)el.Attribute("Id") == model.Id
                             select el).FirstOrDefault();

            element.Element("UserName").Value = model.UserName;
            element.Element("Surname").Value = model.Surname;
            element.Element("CellPhone").Value = model.CellPhone;
            xmlDoc.Save(filename);
            return null;
        }
    }
}
