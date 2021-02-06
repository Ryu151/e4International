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
            if (!System.IO.File.Exists(filename))
            {
                return PartialView("~/Views/PartialViews/_Entries.cshtml", new List<Entry>());
            }
            else
            {
                using (Stream reader = new FileStream(filename, FileMode.Open))
                {
                    if (new FileInfo(filename).Length > 3)
                    {
                        try
                        {
                            var x = ser.Deserialize(reader);
                            view = (Entries)x;

                            var xy = view.Entry.Select(i => new Entry()
                            {
                                Id = i.Id,
                                CellPhone = i.CellPhone,
                                Surname = i.Surname,
                                UserName = i.UserName
                            }).ToList();

                            return PartialView("~/Views/PartialViews/_Entries.cshtml", xy);
                        }
                        catch { return PartialView("~/Views/Shared/Error.cshtml"); }
                    }
                    else
                    {
                        return PartialView("~/Views/PartialViews/_Entries.cshtml", new List<Entry>());
                    }
                }
            }            
        }

        public IActionResult Privacy()
        {
            return View();
        }

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return PartialView("~/Views/Shared/Error.cshtml");
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
                var internalSubset = @"<!ELEMENT Entries ANY>
                                       <!ELEMENT Entry ANY>
                                       <!ATTLIST Entry Id ID #REQUIRED>";
                XDocument xDoc = new XDocument(new XDocumentType("Pubs", null, null, internalSubset));
                XElement root1 = new XElement("Entries");
                xDoc.Add(root1);
                xDoc.Save(filename);
            };

            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load(filename);
            int item = 0;
            var count = xmlDoc.GetElementsByTagName("Entry").Count;

            if (count == 0)
            {
                item++;
            }
            else
            {
                item = Int32.Parse(xmlDoc.GetElementsByTagName("Entry")?.Item(count - 1).Attributes.GetNamedItem("Id").InnerText.Remove(0, 1));
                item += 1;
            }
                
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

            return Index();
        }

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
            return Index();
        }
    }
}
