using PROACCII_DB;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PROACCII.BL.Model
{
    public class ProjectViewModel
    {
        public System.Guid Project_Id { get; set; }
        public string Project_Name { get; set; }
        public string Description { get; set; }
        public System.Guid Customer_Id { get; set; }
        public System.Guid ProjectManager_Id { get; set; }
        public int ScenarioId { get; set; }
        public bool isActive { get; set; }
        public System.DateTime Cre_on { get; set; }
        public System.Guid Cre_By { get; set; }
        public Nullable<System.DateTime> Modified_On { get; set; }
        public Nullable<System.Guid> Modified_by { get; set; }
        public bool IsDeleted { get; set; }
        public string Company_Name { get; set; }
        public string Name { get; set; }
        public string ScenarioName { get; set; }
        public virtual List<Instance> Instances { get; set; }
        public virtual Customer Customer { get; set; }
        public virtual UserMaster UserMaster { get; set; }
        public virtual ScenarioMaster ScenarioMaster { get; set; }

        public int InstanceCount { get; set; }
        public string InstanceName { get; set; }
    }
}