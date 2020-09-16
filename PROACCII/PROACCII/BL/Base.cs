using PROACCII.BL.Common;
using PROACCII.BL.Model;
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
    }
}