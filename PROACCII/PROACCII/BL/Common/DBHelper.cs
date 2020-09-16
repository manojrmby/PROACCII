using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PROACCII.BL.Common
{
    public class DBHelper : IDisposable
    {
        Base @base = new Base();
        private void openConnection()
        {
            if (this.conn != null && this.conn.State == ConnectionState.Closed)
            {
                this.conn.Open();
            }
        }

        private void closeConnection()
        {
            if (this.conn != null && this.conn.State == ConnectionState.Open)
            {
                this.conn.Close();
            }
        }

        public DBHelper(string commandText)
            : this(commandText, CommandType.Text)
        {
        }


        //public DBHelper(string commandText, CommandType type, ConnectionStrings connectionkey)
        public DBHelper(string commandText, CommandType type)
        {
            //ConnectionStringInfoAttribute conninfo = connectionkey.GetAttributeOf<ConnectionStringInfoAttribute>();
            // string constr = ConfigurationManager.ConnectionStrings[conninfo.ConnKey].ConnectionString;
            //constr = PartialDecryption(constr, _salt);
            string constr = ConfigurationManager.ConnectionStrings["DBconnection"].ConnectionString;




            // string en = encrypt(constr);

            //var strEncryptred = Cipher.Encrypt(constr, _salt);
            //var DBconnection = Cipher.Decrypt(constr, @base._salt);

            //constr = Decrypt(constr);
            //string constr = ConfigurationManager.ConnectionStrings["DBconnection"].ConnectionString;
            this.conn = new SqlConnection(constr);
            this.comm = new SqlCommand(commandText, this.conn);
            this.comm.CommandType = type;
        }
        public static T ConvertTo<T>(object argValue)
        {
            if (argValue == DBNull.Value)
                return default(T);
            try
            {
                return (T)argValue;
            }
            catch
            {
                return default(T);
            }
        }

        public DBHelper beginTransact()
        {
            if (!this.transactOpened)
            {
                this.openConnection();
                this.trans = this.conn.BeginTransaction();
                this.comm.Transaction = this.trans;
                this.transactOpened = true;
            }
            return this;
        }

        public void saveTransPoint(string pointName)
        {
            if (this.transactOpened && this.trans != null)
            {
                this.trans.Save(pointName);
            }
        }

        public void commit()
        {
            if (this.transactOpened)
            {
                if (this.trans != null)
                {
                    this.trans.Commit();
                }
                this.closeConnection();
                this.transactOpened = false;
            }
        }

        public void rollback()
        {
            if (this.transactOpened)
            {
                if (this.trans != null)
                {
                    this.trans.Rollback();
                }
                this.closeConnection();
                this.transactOpened = false;
            }
        }

        public void rollback(string pointName)
        {
            if (this.transactOpened)
            {
                if (this.trans != null)
                {
                    this.trans.Rollback(pointName);
                }
                this.transactOpened = false;
            }
        }

        public DBHelper addIn(string parameterName, object parameterValue)
        {
            SqlParameter value = new SqlParameter(parameterName, parameterValue);
            this.comm.Parameters.Add(value);
            return this;
        }

        public DBHelper addIn(string parameterName, object parameterValue, SqlDbType type)
        {
            SqlParameter sqlParameter = new SqlParameter(parameterName, parameterValue);
            sqlParameter.SqlDbType = type;
            this.comm.Parameters.Add(sqlParameter);
            return this;
        }

        public DBHelper addIn(string parameterName, object parameterValue, SqlDbType type, int size)
        {
            SqlParameter sqlParameter = new SqlParameter(parameterName, parameterValue);
            sqlParameter.SqlDbType = type;
            sqlParameter.Size = size;
            this.comm.Parameters.Add(sqlParameter);
            return this;
        }

        public DBHelper addIn(string parameterName, object parameterValue, SqlDbType type, byte precision, byte scale)
        {
            SqlParameter sqlParameter = new SqlParameter(parameterName, parameterValue);
            sqlParameter.SqlDbType = type;
            sqlParameter.Precision = precision;
            sqlParameter.Scale = scale;
            this.comm.Parameters.Add(sqlParameter);
            return this;
        }

        public DBHelper addOut(string parameterName, SqlDbType type, int size)
        {
            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = parameterName;
            sqlParameter.Direction = ParameterDirection.Output;
            sqlParameter.SqlDbType = type;
            sqlParameter.Size = size;
            this.comm.Parameters.Add(sqlParameter);
            return this;
        }

        public DBHelper addOut(string parameterName, SqlDbType type, byte precision, byte scale)
        {
            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = parameterName;
            sqlParameter.Direction = ParameterDirection.Output;
            sqlParameter.SqlDbType = type;
            sqlParameter.Precision = precision;
            sqlParameter.Scale = scale;
            this.comm.Parameters.Add(sqlParameter);
            return this;
        }

        public DBHelper addReturn()
        {
            SqlParameter sqlParameter = new SqlParameter();
            sqlParameter.ParameterName = "@DBHELPER_RETURN_VALUE";
            sqlParameter.Direction = ParameterDirection.ReturnValue;
            this.comm.Parameters.Add(sqlParameter);
            return this;
        }

        public int getReturned()
        {
            return int.Parse(this.getValue("@DBHELPER_RETURN_VALUE").ToString());
        }

        public object getValue(string parameterName)
        {
            return this.comm.Parameters[parameterName].Value;
        }

        public DBHelper CreateNewCommand(string commandText)
        {
            return this.CreateNewCommand(commandText, CommandType.Text);
        }

        public DBHelper CreateNewCommand(string commandText, CommandType commandType)
        {
            this.comm.CommandText = commandText;
            this.comm.Parameters.Clear();
            this.comm.CommandType = commandType;
            return this;
        }

        public int Execute()
        {
            this.openConnection();
            int result = this.comm.ExecuteNonQuery();
            if (!this.transactOpened)
            {
                this.closeConnection();
            }
            return result;
        }

        public DataTable ExecuteDataTable()
        {
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(this.comm);
            DataTable dataTable = new DataTable();
            sqlDataAdapter.Fill(dataTable);
            return dataTable;
        }

        public DataSet ExecuteDataSet()
        {
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(this.comm);
            DataSet dataSet = new DataSet();
            sqlDataAdapter.Fill(dataSet);
            return dataSet;
        }

        public DataRow ExecuteDataRow()
        {
            DataRow result = null;
            DataTable dataTable = this.ExecuteDataTable();
            if (dataTable.Rows.Count > 0)
            {
                result = dataTable.Rows[0];
            }
            return result;
        }

        public object ExecuteScalar()
        {
            this.openConnection();
            object result = this.comm.ExecuteScalar();
            if (!this.transactOpened)
            {
                this.closeConnection();
            }
            return result;
        }

        public bool HasRows()
        {
            DataTable dataTable = this.ExecuteDataTable();
            int count = dataTable.Rows.Count;
            dataTable.Dispose();
            return count > 0;
        }

        public void Dispose()
        {
            if (this.trans != null)
            {
                this.trans.Dispose();
            }
            if (this.comm != null)
            {
                this.comm.Dispose();
            }
            this.closeConnection();
        }

        private SqlConnection conn;

        private SqlCommand comm;

        private SqlTransaction trans;

        private bool transactOpened;
    }
}