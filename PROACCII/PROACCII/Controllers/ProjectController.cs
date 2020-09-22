using PROACCII.BL;
using PROACCII.BL.Model;
using PROACCII_DB;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROACCII.Controllers
{
    public class ProjectController : Controller
    {
        private ProAccEntities db = new ProAccEntities();
        Base _Base = new Base();
        // GET: ProjectCreation
        public ActionResult Index()
        {
            List<ProjectViewModel> model = new List<ProjectViewModel>();
            var projects = db.Projects.Where(x => x.isActive == true);
            foreach (Project project in projects)
            {
                model.Add(new ProjectViewModel
                {
                    Project_Id = project.Project_Id,
                    Project_Name = project.Project_Name,
                    Cre_on = project.Cre_on,
                    Customer_Id = project.Customer_Id,
                    ProjectManager_Id = project.ProjectManager_Id,
                    ScenarioId = project.ScenarioId,
                    Instances = db.Instances.Where(x => x.Project_ID == project.Project_Id && x.isActive == true).ToList(),
                    Customer = db.Customers.Where(x => x.Customer_ID == project.Customer_Id).FirstOrDefault(),
                    UserMaster = db.UserMasters.Where(x => x.UserId == project.ProjectManager_Id).FirstOrDefault(),
                    ScenarioMaster = db.ScenarioMasters.Where(x => x.ScenarioId == project.ScenarioId).FirstOrDefault(),
                    InstanceCount = db.Instances.Where(x => x.Project_ID == project.Project_Id && x.isActive == true).Count()
                });
            }
            return View(model);
            //List<ProjectViewModel> PVM = new List<ProjectViewModel>();            
            //PVM = _Base.Sp_GetProjectViewData();

            //var projects = db.Projects.Where(x => x.isActive == true);
            //foreach (var project in projects)
            //{
            //    PVM.Add(new ProjectViewModel { 
            //        Instances = db.Instances.Where(x => x.Project_ID == project.Project_Id && x.isActive == true).ToList() 
            //    });

            //}
            //return View(PVM);
        }

        public ActionResult Create()
        {
            ViewBag.Customer = db.Customers.Where(x => x.isActive == true).OrderBy(x => x.Company_Name).ToList();            
            ViewBag.ProjectManager = db.UserMasters.Where(x => x.UserTypeID == 4 && x.isActive == true).OrderBy(x => x.Name).ToList();
            ViewBag.Scenario = db.ScenarioMasters.Where(x => x.isActive == true).OrderBy(x => x.ScenarioName).ToList();
            
            return View();
        }

        [HttpPost]
        public ActionResult Create(ProjectViewModel project)
        {
            var name = db.Projects.Where(p => p.Project_Name == project.Project_Name).Where(x => x.isActive == true).ToList();
            if (name.Count == 0)
            {
                project.Cre_By = Guid.Parse(Session["loginid"].ToString());
                var Status = _Base.Project_Master_Add_Update(project);

                if(Status==true)
                {
                    return Json("success");
                }
                else
                {
                    return Json("error");
                }
                //project.Project_Id = Guid.NewGuid();
                //project.isActive = true;
                //project.Cre_on = DateTime.UtcNow;
                //project.Cre_By = Guid.Parse(Session["loginid"].ToString());
                //db.Projects.Add(project);
                //db.SaveChanges();
                //return Json("success");
            }
            else
            {
                return Json("error");
            }
        }


        [HttpGet]
        public ActionResult GetProjectById(Guid? id)
        {
            try
            {
                ViewBag.Customer = db.Customers.Where(x => x.isActive == true).OrderBy(x => x.Company_Name).ToList();
                ViewBag.ProjectManager = db.UserMasters.Where(x => x.UserTypeID == 4 && x.isActive == true).OrderBy(x => x.Name).ToList();
                ViewBag.Scenario = db.ScenarioMasters.Where(x => x.isActive == true).OrderBy(x => x.ScenarioName).ToList();
                Project project = db.Projects.Find(id);
                //var Project = db.Projects.Where(x => x.isActive == true && x.Project_Id == id).Select(
                //    p => new
                //    {
                //        p.Project_Id,
                //        p.Project_Name,
                //        p.Description,
                //        p.Customer_Id,
                //        p.ProjectManager_Id,
                //        p.ScenarioId,
                //    }).FirstOrDefault();
                return View(project);
            }
            catch (Exception ex)
            {
                string Url = Request.Url.AbsoluteUri;
                throw;
            }
        }

        [HttpPost]
        public ActionResult Edit(ProjectViewModel project)
        {
            var name = db.Projects.Where(p => p.Project_Name == project.Project_Name && p.Project_Id!=project.Project_Id).Where(x => x.isActive == true).ToList();
            if (name.Count == 0)
            {
                project.Cre_By = Guid.Parse(Session["loginid"].ToString());
                var Status = _Base.Project_Master_Add_Update(project);

                if (Status == true)
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

        [HttpPost]
        public ActionResult Delete(Guid id)
        {
            Project project = db.Projects.Find(id);
            project.Modified_by = Guid.Parse(Session["loginid"].ToString());
            if (project.Project_Id == id)
            {
                var Status = _Base.Project_Delete(project);
                if (Status == true)
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


    }
}