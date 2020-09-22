using PROACCII.BL.Common;
using PROACCII.BL.Model;
using PROACCII_DB;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;

namespace PROACCII.BL
{
    public class Base
    {

        #region Login
        public LogedUser UserValidation(LogedUser user)
        {
            DataSet ds = new DataSet();

            DBHelper dB = new DBHelper("SP_Login", CommandType.StoredProcedure);
            dB.addIn("@Type", "Login");
            dB.addIn("@UserName", user.Username);
            dB.addIn("@Password", Encrypt(user.Password));
            ds = dB.ExecuteDataSet();
            DataTable dt = new DataTable();
            if (ds.Tables.Count != 0)
            {
                //DataTable dt1 = new DataTable();
                dt = ds.Tables[0];
                //dt1 = ds.Tables[1];
                if (dt.Rows.Count == 1)
                {
                    if (!string.IsNullOrEmpty(dt.Rows[0][0].ToString()))
                    {


                        user.ID = Guid.Parse(dt.Rows[0][0].ToString());
                        user.Type = Convert.ToInt32(dt.Rows[0][1].ToString());
                        user.Name = dt.Rows[0][2].ToString();

                        //User_ID = user.ID;
                        //User_Name = user.Name;
                        //for (int i = 0; i < dt1.Rows.Count; i++)
                        //{
                        //    if (user.Type == Convert.ToInt32(dt1.Rows[i]["id"]))
                        //    {
                        //        User_Type = dt1.Rows[0]["UserType"].ToString().ToUpper();
                        //        break;
                        //    }
                        //}
                    }
                }
            }
            return user;

        }
        #endregion

        #region Common
        public string _salt = "d5cc07aa70fd47";
        public void CreateIfMissing(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void SendExcepToDB(string ExceptionMsg, string ExceptionType, string ExceptionURL, string ExceptionSource)
        {
            DBHelper dB = new DBHelper("Sp_ExceptionLogging", CommandType.StoredProcedure);
            dB.addIn("@ExceptionMsg", ExceptionMsg);
            dB.addIn("@ExceptionType", ExceptionType);
            dB.addIn("@ExceptionURL", ExceptionURL);
            dB.addIn("@ExceptionSource", ExceptionSource);
            dB.ExecuteScalar();
        }

        public string Encrypt(string st)
        {
            return Cipher.Encrypt(st, _salt);
        }
        public string Decrypt(string st)
        {
            return Cipher.Decrypt(st, _salt);
        }
        #endregion

        #region project
        
        public List<ProjectViewModel> Sp_GetProjectViewData()
        {
            List<ProjectViewModel> pvm = new List<ProjectViewModel>();
            DataTable dt = new DataTable();
            DBHelper dB = new DBHelper("SP_Project", CommandType.StoredProcedure);
            dB.addIn("@Type", "PullData");
           
            dt = dB.ExecuteDataTable();
            if (dt.Rows.Count > 0)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    ProjectViewModel P = new ProjectViewModel();
                    P.Project_Id = Guid.Parse(dr["Project_Id"].ToString());
                    P.Project_Name = dr["Project_Name"].ToString();
                    P.Cre_on = Convert.ToDateTime(dr["Cre_on"].ToString());
                    P.Company_Name = dr["Company_Name"].ToString();
                    P.Name = dr["Name"].ToString();
                    P.ScenarioName = dr["ScenarioName"].ToString();
                    P.InstanceCount = Convert.ToInt32(dr["InstanceCount"].ToString());
                    //P.InstanceName = dr["InstaceName"].ToString();
                    //P.EndDate = Convert.ToDateTime(dr["EndDate"].ToString());
                    //P.LastUpdatedDate = Convert.ToDateTime(dr["LastUpdatedDate"].ToString());
                    //P.AssignedTo = Guid.Parse(dr["AssignedTo"].ToString());
                    //P.Status = dr["Status"].ToString();
                    pvm.Add(P);
                }
            }
            return pvm;
        }

        public Boolean Project_Master_Add_Update(ProjectViewModel project)
        {
            bool Status = false;
            LogHelper _log = new LogHelper();
            try
            {
                DBHelper dB = new DBHelper("SP_Project", CommandType.StoredProcedure);
                if (project.Project_Id == Guid.Empty)
                {
                    dB.addIn("@Type", "ProjectAdd");
                    dB.addIn("@ProjectId", Guid.NewGuid());
                }
                else
                {
                    dB.addIn("@Type", "ProjectUpdate");
                    dB.addIn("@Project_Id", project.Project_Id);
                }

                dB.addIn("@Project_Name", project.Project_Name);
                dB.addIn("@Description", project.Description);
                dB.addIn("@Customer_Id", project.Customer_Id);
                dB.addIn("@ProjectManager_Id", project.ProjectManager_Id);
                dB.addIn("@ScenarioId", project.ScenarioId);
                dB.addIn("@Cre_By", project.Cre_By);
                dB.ExecuteScalar();

                Status = true;
            }
            catch (Exception ex)
            {
                _log.createLog(ex, "");
            }
            return Status;
        }

        public Boolean Project_Delete(Project project)
        {
            bool Status = false;
            LogHelper _log = new LogHelper();
            try
            {
                DBHelper dB = new DBHelper("SP_Project", CommandType.StoredProcedure);
                if (project.Project_Id != Guid.Empty)
                {
                    dB.addIn("@Type", "ProjectDelete");
                    dB.addIn("@Project_Id", project.Project_Id);
                    dB.addIn("@Cre_By", project.Modified_by);
                }

                dB.ExecuteScalar();

                Status = true;
            }
            catch (Exception ex)
            {
                _log.createLog(ex, "");
            }
            return Status;
        }
        #endregion

        #region Instance

        public Boolean Instance_Add(Instance instance)
        {
            bool Status = false;
            LogHelper _log = new LogHelper();
            try
            {
                DBHelper dB = new DBHelper("SP_Instance", CommandType.StoredProcedure);
                if (instance.Instance_id == Guid.Empty)
                {
                    dB.addIn("@Type", "instanceAdd");              
                    dB.addIn("@InstaceName", instance.InstaceName);
                    dB.addIn("@Project_ID", instance.Project_ID);
                    dB.addIn("@Cre_By", instance.Cre_By);
                    dB.ExecuteScalar();

                    Status = true;
                }
            }
            catch (Exception ex)
            {
                _log.createLog(ex, "");
            }
            return Status;
        }

        public Boolean Instance_Delete(Instance instance)
        {
            bool Status = false;
            LogHelper _log = new LogHelper();
            try
            {
                DBHelper dB = new DBHelper("SP_Instance", CommandType.StoredProcedure);
                if (instance.Instance_id != Guid.Empty)
                {
                    dB.addIn("@Type", "instanceDelete");
                    dB.addIn("@Instance_id", instance.Instance_id);
                    dB.addIn("@Cre_By", instance.Modified_by);
                    dB.ExecuteScalar();
                    Status = true;
                }  
            }
            catch (Exception ex)
            {
                _log.createLog(ex, "");
            }
            return Status;
        }

        #endregion

        #region CUSTOMER-CRUD
        public bool Sp_InserCustomer(Customer customer)
        {
            Boolean Result = false;
            DataTable dt = new DataTable();
            DBHelper dB = new DBHelper("Sp_Customer", CommandType.StoredProcedure);
            dB.addIn("@Type", "Insert");
            dB.addIn("@Company_Name", customer.Company_Name);
            dB.addIn("@IndustrySector_ID", customer.IndustrySector_ID);
            dB.addIn("@Contact", customer.Contact);
            dB.addIn("@Phone", customer.Phone);
            dB.addIn("@Email", customer.Email);
            dB.addIn("@Cre_By", customer.Cre_By);
            dB.addIn("@Cre_on", customer.Cre_on);
            dB.addIn("@isActive", customer.isActive);
            dB.ExecuteScalar();
            Result = true;
            return Result;
        }



        public bool Sp_EditCustomer(Customer customer)
        {
            Boolean Result = false;
            DataTable dt = new DataTable();
            DBHelper dB = new DBHelper("Sp_Customer", CommandType.StoredProcedure);
            dB.addIn("@Type", "Update");
            dB.addIn("@Id", customer.Customer_ID);
            dB.addIn("@Company_Name", customer.Company_Name);
            dB.addIn("@IndustrySector_ID", customer.IndustrySector_ID);
            dB.addIn("@Contact", customer.Contact);
            dB.addIn("@Phone", customer.Phone);
            dB.addIn("@Email", customer.Email);
            dB.addIn("@Modified_by", customer.Modified_by);
            dB.addIn("@Modified_On", customer.Modified_On);
            dB.addIn("@isActive", customer.isActive);
            dB.ExecuteScalar();
            Result = true;
            return Result;
        }
        #endregion

        #region ROLE
        public bool Sp_CreateRole(RoleMaster role)
        {
            Boolean Result = false;
            DataTable dt = new DataTable();
            DBHelper dB = new DBHelper("Sp_RoleMaster", CommandType.StoredProcedure);
            dB.addIn("@Type", "Insert");
            dB.addIn("@RoleName", role.RoleName);
            dB.addIn("@Cre_By", role.Cre_By);
            dB.addIn("@Cre_on", role.Cre_on);
            dB.addIn("@isActive", role.isActive);
            dB.ExecuteScalar();
            Result = true;
            return Result;
        }
        public bool Sp_UpdateRole(RoleMaster role)
        {
            Boolean Result = false;
            DataTable dt = new DataTable();
            DBHelper dB = new DBHelper("Sp_RoleMaster", CommandType.StoredProcedure);
            dB.addIn("@Type", "Update");
            dB.addIn("@RoleId", role.RoleId);
            dB.addIn("@RoleName", role.RoleName);
            dB.addIn("@Modified_by", role.Modified_by);
            dB.addIn("@Modified_On", role.Modified_On);
            dB.addIn("@isActive", role.isActive);
            dB.ExecuteScalar();
            Result = true;
            return Result;
        }

        #endregion
    }
}