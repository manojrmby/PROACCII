using PROACCII.BL;
using PROACCII.BL.Common;
using PROACCII.BL.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace PROACCII.Controllers
{
    [AllowAnonymous]
    public class LoginController : Controller
    {
        Base _Base = new Base();
        LogHelper _Log = new LogHelper();
        // GET: Login
        public ActionResult Login()
        {
            if (Session["loginid"] != null)
            {
                return RedirectToAction("Home", "Home");
            }
            else
            {
                return View();
            }

        }
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            return RedirectToAction("Login");
        }


        [HttpPost]
        [ValidateAntiForgeryToken()]
        public ActionResult Validate(UserModel user, string ReturnUrl)
        {
            try
            {
                LogedUser logedUser = new LogedUser();
                logedUser.Username = user.Username;
                logedUser.Password = user.Password;

                if (!String.IsNullOrEmpty(user.Username) && (!String.IsNullOrEmpty(user.Password)))
                {
                    //Boolean a = Convert.ToBoolean(ConfigurationManager.AppSettings["Enc"].ToString());
                    //if (a)
                    //{
                    //    ProAccEntities db = new ProAccEntities();
                    //    var aa = db.UserMasters.ToList();
                    //    foreach (var item in aa)
                    //    {
                    //        ProAccEntities db1 = new ProAccEntities();
                    //        UserMaster us = new UserMaster();
                    //        us.UserId = item.UserId;
                    //        us.Name = item.Name;
                    //        us.EMail = item.EMail;
                    //        us.Phone = item.Phone;
                    //        us.LoginId = item.LoginId;
                    //        us.Password = _Base.PasswordEncrypt(item.Password);
                    //        us.RoleID = item.RoleID;
                    //        us.UserTypeID = item.UserTypeID;
                    //        us.Customer_Id = item.Customer_Id;

                    //        us.isActive = item.isActive;
                    //        us.IsDeleted = item.IsDeleted;


                    //        us.Cre_on = item.Cre_on;
                    //        us.Cre_By = item.Cre_By;


                    //        us.Modified_by = item.Modified_by;
                    //        us.Modified_On = item.Modified_On;
                    //        db1.Entry(us).State = EntityState.Modified;
                    //        db1.SaveChanges();

                    //    }
                    //}
                    logedUser = _Base.UserValidation(logedUser);
                    if (logedUser.ID != Guid.Empty)
                    {
                        FormsAuthentication.SetAuthCookie(logedUser.Username, false);
                        Session["loginid"] = logedUser.ID.ToString();
                        Session["UserName"] = logedUser.Name.ToString();
                        Session["InstanceId"] = Guid.Empty;
                        //Session["UserTypeID"] = logedUser.Type;
                        string UserType = "";
                        if (logedUser.Type == 1)
                        {
                            UserType = "Admin";
                        }
                        else if (logedUser.Type == 2)
                        {
                            UserType = "Consultant";
                        }
                        else if (logedUser.Type == 3)
                        {
                            UserType = "Customer";
                        }
                        else if (logedUser.Type == 4)
                        {
                            UserType = "Project Manager";
                        }
                        Session["UserType"] = UserType;
                        if (!string.IsNullOrEmpty(Request.Form["ReturnUrl"]))
                        {
                            String Controller = Request.Form["ReturnUrl"].Split('/')[1].ToString();
                            String Action = Request.Form["ReturnUrl"].Split('/')[2].ToString();

                            return RedirectToAction(Action, Controller);
                        }
                        else
                        {
                            return RedirectToAction("Home", "Home");
                        }
                    }
                    else
                    {
                        TempData["Message"] = "User name & password supplied doesn't Match";
                    }
                }
                else
                {
                    TempData["Message"] = "Enter User name & password";
                }
            }
            catch (Exception ex)
            {

                string Url = Request.Url.AbsoluteUri;
                _Log.createLog(ex, Url);
                TempData["Message"] = "Login failed.Error - " + ex.Message;
                //throw;
            }
            return RedirectToAction("Login", "Login");
        }
    }
}