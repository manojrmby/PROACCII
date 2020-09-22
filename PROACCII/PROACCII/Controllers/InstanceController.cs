using PROACCII.BL;
using PROACCII_DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROACCII.Controllers
{
    public class InstanceController : Controller
    {
        ProAccEntities db = new ProAccEntities();
        Base _Base = new Base();
        // GET: Instance
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Create()
        {
            ViewBag.project = (from e in db.Projects
                               join cu in db.Customers on e.Customer_Id equals cu.Customer_ID
                               where e.isActive == true && cu.isActive == true
                               select e).OrderBy(x => x.Project_Name).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(Instance instance)
        {
            var name = db.Instances.Where(p => p.InstaceName == instance.InstaceName && p.Project_ID == instance.Project_ID).Where(x => x.isActive == true).ToList();
            if (name.Count == 0)
            {
                instance.Cre_By = Guid.Parse(Session["loginid"].ToString());
                var Status = _Base.Instance_Add(instance);

                if (Status == true)
                {
                    return Json("success");
                }
                else
                {
                    return Json("error");
                }
                //instance.Instance_id = Guid.NewGuid();
                //instance.isActive = true;
                //instance.Cre_on = DateTime.UtcNow;
                //instance.LastUpdated_Dt = DateTime.UtcNow; ;
                //instance.Cre_By = Guid.Parse(Session["loginid"].ToString());
                //db.Instances.Add(instance);
                //db.SaveChanges();
                //return Json("success");
            }
            else
            {
                return Json("error");
            }
        }

        [HttpPost]
        public ActionResult Delete(Guid? id)
        {
            Instance instance = db.Instances.Find(id);
            instance.Modified_by = Guid.Parse(Session["loginid"].ToString());
            if (instance.Instance_id == id)
            {
                var Status = _Base.Instance_Delete(instance);
                if (Status == true)
                {
                    return Json("success");
                }
                else
                {
                    return Json("error");
                }

                //instance.isActive = false;
                //instance.IsDeleted = true;
                //instance.Modified_On = DateTime.UtcNow;
                //instance.Modified_by = Guid.Parse(Session["loginid"].ToString());
                //db.Entry(instance).State = EntityState.Modified;
                //db.SaveChanges();
            }
            return Json("success");
        }
    }
}