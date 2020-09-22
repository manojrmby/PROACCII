using PROACCII.BL;
using PROACCII.BL.Common;
using PROACCII_DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PROACCII.Controllers
{
    [CheckSessionTimeOut]
    [Authorize(Roles = "Admin")]
    public class HomeController : Controller
    {
        ProAccEntities db = new ProAccEntities();
        LogHelper _Log = new LogHelper();
        Base _Base = new Base();


        // GET: Home
        public ActionResult Home()
        {
            return View();
        }


        #region Customer

        public ActionResult Index()
        {
            List<PROACCII_DB.Customer> customers = null;
            try
            {
                customers = db.Customers
               .Where(a => a.isActive == true)
               .OrderByDescending(x => x.Cre_on).ToList();
            }
            catch (Exception ex)
            {
                _Log.createLog(ex, "-->Customers Index" + ex.Message.ToString());
            }
            return PartialView(customers);
        }
        public ActionResult Customer()
        {
            ViewBag.IndustrySector = db.IndustrySectors.Where(x => x.IsActive == true);
            ViewBag.customersIndex = db.Customers.Where(x => x.isActive == true).ToList();
            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            try
            {
                var name = db.Customers.Where(p => p.Company_Name == customer.Company_Name).Where(x => x.isActive == true).ToList();
                if (name.Count == 0)
                {
                    customer.isActive = true;
                    customer.Cre_on = DateTime.UtcNow;
                    customer.Cre_By = Guid.Parse(Session["loginid"].ToString());
                    bool result = _Base.Sp_InserCustomer(customer);
                    if (result == true)
                    {
                        return Json("success");
                    }
                    else
                    {
                        return Json("error");
                    }
                }
                else
                {
                    return Json("error");
                }

            }
            catch (Exception Ex)
            {
                string Url = Request.Url.AbsoluteUri;
                _Log.createLog(Ex, "-->Create Post" + Url);
                throw;
            }

        }

        public ActionResult GetCustomerById(Guid? id)
        {
            Customer customer = null;
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                customer = db.Customers.Find(id);
                if (customer == null)
                {
                    return HttpNotFound();
                }
                var Data = db.Customers.Find(id);
                ViewBag.IndustrySector = db.IndustrySectors.Where(x => x.IsActive == true);
            }
            catch (Exception Ex)
            {
                _Log.createLog(Ex, "-->Create Custoemr Post" + Url);
            }

            return PartialView(customer);
        }

        [HttpPost]
        public ActionResult Edit(Customer customer)
        {
            try
            {
                var name = db.Customers.Where(p => p.Company_Name == customer.Company_Name).Where(x => x.Customer_ID != customer.Customer_ID).Where(x => x.isActive == true).ToList();
                if (name.Count == 0)
                {
                    customer.Modified_On = DateTime.UtcNow;
                    customer.Modified_by = Guid.Parse(Session["loginid"].ToString());
                    customer.isActive = true;
                    bool result = _Base.Sp_EditCustomer(customer);
                    if (result == true)
                    {
                        return Json("success");
                    }
                    else
                    {
                        return Json("error");
                    }
                }
                else
                {
                    return Json("error");
                }
            }
            catch (Exception Ex)
            {
                string Url = Request.Url.AbsoluteUri;
                _Log.createLog(Ex, "-->Edit Customer Post" + Url);
                throw;
            }

        }

        public ActionResult Delete(Guid? id)
        {
            Customer customer = null;
            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                customer = db.Customers.Find(id);
                if (customer == null)
                {
                    return HttpNotFound();
                }
            }
            catch (Exception Ex)
            {
                string Url = Request.Url.AbsoluteUri;
                _Log.createLog(Ex, "-->Delete Customer" + Url);
                throw;
            }

            return View(customer);
        }

        // POST: Customers/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Guid id)
        {
            try
            {
                var del = (from a in db.Customers
                           join b in db.Projects
                           on a.Customer_ID equals b.Customer_Id
                           where a.Customer_ID == id && a.isActive == true && b.isActive == true
                           select b).ToList();

                if (del.Count != 0)
                {
                    return Json("fail");
                }
                else
                {
                    Customer customer = db.Customers.Find(id);
                    if (customer.Customer_ID == id)
                    {
                        customer.isActive = false;
                        customer.IsDeleted = true;
                        customer.Modified_On = DateTime.UtcNow;
                        customer.Modified_by = Guid.Parse(Session["loginid"].ToString());
                        db.Entry(customer).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                return Json("success");
            }
            catch (Exception Ex)
            {
                string Url = Request.Url.AbsoluteUri;
                _Log.createLog(Ex, "-->Delete Customer Post" + Url);
                throw;
            }
        }

        public JsonResult CheckCustomersNameAvailability(string namedata, Guid? id)
        {
            if (id != null)
            {
                var SearchDt = db.Customers.Where(x => x.Company_Name == namedata).Where(x => x.Customer_ID != id).Where(x => x.isActive == true).FirstOrDefault();
                if (SearchDt != null)
                {
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var SearchDt = db.Customers.Where(x => x.Company_Name == namedata).Where(x => x.isActive == true).FirstOrDefault();
                if (SearchDt != null)
                {
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
            }

        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion


        #region Role

        public ActionResult RoleIndex()
        {
            List<PROACCII_DB.RoleMaster> RoleList = null;
            try
            {
                //var adminRoleId = db.RoleMaster.Where(x => x.RoleName == "Admin" && x.isActive == true).FirstOrDefault().RoleId;
                //var pmRoleId = db.RoleMaster.Where(x => x.RoleName == "Project Manager" && x.isActive == true).FirstOrDefault().RoleId;
                RoleList = db.RoleMasters.Where(x => x.isActive == true).OrderByDescending(x => x.Cre_on).ToList();
            }
            catch (Exception ex)
            {
                _Log.createLog(ex, "-->GetRolesList" + ex.Message.ToString());
            }
            return PartialView(RoleList);
        }

        public ActionResult CreateRole()
        {
            return PartialView();
        }

        [HttpPost]
        public ActionResult CreateRole(RoleMaster role)
        {
            role.Cre_By = Guid.Parse(Session["loginid"].ToString());
            role.Cre_on = DateTime.UtcNow;
            role.isActive = true;
            bool result = _Base.Sp_CreateRole(role);
            if (result == true)
            {
                return Json("success");
            }
            else
            {
                return Json("Error");
            }
        }



        [HttpGet]
        public ActionResult GetRoleById(int id)
        {
            try
            {
                var team = db.RoleMasters.Find(id);
                RoleMaster master = new RoleMaster();
                master.RoleId = id;
                master.RoleName = team.RoleName;
                master.Cre_on = team.Cre_on;
                master.Cre_By = team.Cre_By;
                TempData["Cre_on"] = team.Cre_on;
                return PartialView(master);
            }
            catch (Exception ex)
            {
                string Url = Request.Url.AbsoluteUri;
                _Log.createLog(ex, "-->GetRoleById" + Url);
                throw;
            }
        }

        [HttpPost]
        public ActionResult EditRole(RoleMaster model)
        {
            try
            {
                model.Cre_on = Convert.ToDateTime(TempData["Cre_on"]);
                model.Modified_On = DateTime.UtcNow;
                model.Modified_by = Guid.Parse(Session["loginid"].ToString());
                model.isActive = true;
                bool Result = _Base.Sp_UpdateRole(model);
                if (Result == true)
                {
                    return Json("Success");
                }
                else
                {
                    return Json("success");
                }
            }
            catch (Exception ex)
            {
                string Url = Request.Url.AbsoluteUri;
                _Log.createLog(ex, "-->Role Edit Post" + Url);
                throw;
            }


        }

        [HttpPost]
        public ActionResult DeleteRole(int id)
        {
            try
            {
                RoleMaster tmaster = db.RoleMasters.Find(id);
                tmaster.isActive = false;
                tmaster.IsDeleted = true;
                db.Entry(tmaster).State = EntityState.Modified;
                db.SaveChanges();

                return Json("success");
            }
            catch (Exception ex)
            {
                string Url = Request.Url.AbsoluteUri;
                _Log.createLog(ex, "-->Role Delete Post" + Url);
                throw;
            }
        }

        [HttpGet]
        public ActionResult CheckRole(string name, int? id)
        {
            if (id != null)
            {
                var em = db.RoleMasters.Where(p => p.RoleName == name).Where(x => x.RoleId != id).Where(x => x.isActive == true).ToList();
                if (em.Count > 0)
                {
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                var em = db.RoleMasters.Where(p => p.RoleName == name).Where(x => x.isActive == true).ToList();
                if (em.Count > 0)
                {
                    return Json("error", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("success", JsonRequestBehavior.AllowGet);
                }
            }
        }
        #endregion


    }
}