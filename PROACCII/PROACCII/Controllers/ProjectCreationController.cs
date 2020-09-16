using PROACCII.BL.Model;
using PROACCII_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PROACCII.Controllers
{
    public class ProjectCreationController : Controller
    {
        private ProAccEntities db = new ProAccEntities(); 
        // GET: ProjectCreation
        public ActionResult Index()
        {
            List<ProjectViewModel> model = new List<ProjectViewModel>();
            var projects = db.Projects.Where(x => x.isActive == true);
            foreach (Project project in projects)
            {
                model.Add(new ProjectViewModel
                {
                    Project_Name= project.Project_Name,
                    Cre_on=project.Cre_on,
                    Customer_Id=project.Customer_Id,
                    ProjectManager_Id=project.ProjectManager_Id,
                    ScenarioId=project.ScenarioId,
                    Instances=db.Instances.Where(x=>x.Project_ID==project.Project_Id).ToList()
                });
            }

            return View(model);
        }
    }
}