using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WebApiHash.Context;
using WebApiHash.Models;

namespace WebApiHash.Controllers
{
    public class ImeiController : Controller
    {
        HashContext db = new HashContext();
        public object DeviceId { get; private set; }
        public ActionResult Index()
        {
            return View(db.Devices.ToList());
        }
        public ActionResult Create(String imei)   //create imei dla urzadzenia
        {
            Device device = new Device();
            ViewBag.imei = imei;
            device.DeviceUniqueId = imei;
            db.Devices.Add(device);
            db.SaveChanges();
            return View();
        }

        public ActionResult Delete(int id = 0)    //delete imei dla urzadzenia
        {
            Device DeviceId = db.Devices.Find(id);
            if (DeviceId == null)
            {
                return HttpNotFound();
            }
            return View(DeviceId);
        }
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id = 0)
        {
            Device DeviceId = db.Devices.Find(id);
            db.Devices.Remove(DeviceId);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}