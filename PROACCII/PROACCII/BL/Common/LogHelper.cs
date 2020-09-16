using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace PROACCII.BL.Common
{
    public sealed class LogHelper
    {
        Base _Base = new Base();
        #region Pivate Variables

        private static readonly LogHelper instance = new LogHelper();
        private string _FileName = DateTime.UtcNow.ToString("MM-dd-yyyy") + ".txt", _FileLocation = System.Web.Hosting.HostingEnvironment.MapPath(ConfigurationManager.AppSettings["Error_filePath"].ToString());
        private bool _FileAppend = true;
        private static bool _logType = false, _loggingOn = true;
        private string _nameSpace;
        private string _methodName;
        private string _className;
        private string _methodArgs;
        private StreamWriter Writer = null;

        #endregion

        #region Public Properties

        public string FileName
        {
            get
            {
                return _FileName;
            }
            set
            {
                _FileName = value;
            }
        }
        public string FileLocation
        {
            get
            {
                return _FileLocation;
            }
            set
            {
                _FileLocation = value;
            }
        }
        public bool FileAppend
        {
            get
            {
                return _FileAppend;
            }
            set
            {
                _FileAppend = value;
            }
        }
        public static bool LoggingOn
        {
            get
            {
                return _loggingOn;
            }
            set
            {
                _loggingOn = value;
            }
        }
        /// <summary>
        /// Set True to use FileLogTraceListener & False to StreamWriter
        /// </summary>
        public static bool LogType
        {
            get { return _logType; }
            set { _logType = value; }
        }
        #endregion

        #region Private Properties
        private string NameSpace
        {
            get
            {
                InitializeName();
                return _nameSpace;
            }
            set { _nameSpace = value; }
        }

        private string MethodName
        {
            get
            {
                InitializeName();
                return _methodName;
            }
            set { _methodName = value; }
        }

        private string MethodArguments
        {
            get
            {
                InitializeName();
                return _methodArgs;
            }
            set { _methodArgs = value; }
        }

        private string ClassName
        {
            get
            {
                InitializeName();
                return _className;
            }
            set { _className = value; }
        }

        private StreamWriter MyWriter
        {
            get { return Writer; }
            set { Writer = value; }
        }
        #endregion

        #region Public Static Method
        public static LogHelper GetLogHelper()
        {
            return instance;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Should Called at the Start Point of Log
        /// </summary>
        /// <returns>Boolean</returns>
        public bool StartLog()
        {
            if (LoggingOn)
            {
                if (LogType)
                {
                    //FileLogTraceListener logInfo = new FileLogTraceListener();
                    //try
                    //{
                    //    logInfo = (FileLogTraceListener)AssignProperty();

                    //    logInfo.WriteLine("");
                    //    logInfo.WriteLine("");
                    //    logInfo.WriteLine("");
                    //    logInfo.WriteLine("======================Logging Started At:" + DateTime.UtcNow + "======================");
                    //    //logInfo.Close();
                    //}
                    //catch (Exception ex)
                    //{
                    //    EventLog.WriteEntry("Application", ex.Message);
                    //}
                    //finally
                    //{
                    //    logInfo.Close();
                    //}
                }
                else
                {
                    lock (this)
                    {
                        GetWriter();
                        try
                        {
                            MyWriter.WriteLine("");
                            MyWriter.WriteLine("");
                            MyWriter.WriteLine("");
                            MyWriter.WriteLine("======================Logging Started At:" + DateTime.UtcNow + "======================");
                            //MyWriter.Close();
                        }
                        finally
                        {
                            MyWriter.Close();
                        }
                    }
                }
            }


            return true;
        }

        /// <summary>
        /// Should Called at the Start Point of Log
        /// </summary>
        /// <param name="strMsg">Message to Be passed with the StartLog</param>
        /// <returns>Boolean</returns>
        public bool StartLog(string strMsg)
        {

            if (LoggingOn)
            {
                if (LogType)
                {
                    //FileLogTraceListener logInfo = new FileLogTraceListener();
                    //try
                    //{
                    //    logInfo = (FileLogTraceListener)AssignProperty();

                    //    logInfo.WriteLine("");
                    //    logInfo.WriteLine("");
                    //    logInfo.WriteLine("");
                    //    logInfo.WriteLine("======================Logging for " + strMsg + " Started At:" + DateTime.UtcNow + "======================");
                    //    //logInfo.Close();
                    //}
                    //finally
                    //{
                    //    logInfo.Close();
                    //}
                }
                else
                {
                    lock (this)
                    {
                        GetWriter();
                        try
                        {
                            MyWriter.WriteLine("");
                            MyWriter.WriteLine("");
                            MyWriter.WriteLine("");
                            MyWriter.WriteLine("======================Logging for " + strMsg + " Started At:" + DateTime.UtcNow + "======================");
                            //MyWriter.Close();
                        }
                        finally
                        {
                            MyWriter.Close();
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Should Called at different points in method 
        /// </summary>
        /// <param name="strMsg">Message</param>
        /// <returns>Boolean</returns>
        public bool createLog(string strMsg)
        {

            if (LoggingOn)
            {
                if (LogType)
                {
                    //FileLogTraceListener logInfo = new FileLogTraceListener();
                    //try
                    //{
                    //    logInfo = new FileLogTraceListener();
                    //    logInfo = (FileLogTraceListener)AssignProperty();

                    //    //logInfo.WriteLine("=====Log Start DateTime:" + DateTime.UtcNow+ "=======");
                    //    //logInfo.WriteLine(strMsg);
                    //    //logInfo.WriteLine("=====Log End=======");
                    //    logInfo.WriteLine("============" + strMsg + " At " + DateTime.UtcNow + "============");
                    //    logInfo.WriteLine("");
                    //    //logInfo.Close();
                    //}
                    //finally
                    //{
                    //    logInfo.Close();
                    //}
                }
                else
                {
                    lock (this)
                    {
                        GetWriter();
                        try
                        {
                            //logInfo = new StreamWriter(FileLocation + "/" + FileName, true);

                            MyWriter.WriteLine("============" + strMsg + " At " + DateTime.UtcNow + "============");
                            MyWriter.WriteLine("");

                        }
                        finally
                        {
                            MyWriter.Close();
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a Log for Exception Raised, can be used in Catch Blocks
        /// </summary>
        /// <param name="ex">Exception object</param>
        /// <returns>Boolean</returns>
        public bool createLog(Exception ex)
        {

            if (LoggingOn)
            {
                if (LogType)
                {
                    //FileLogTraceListener logInfo = new FileLogTraceListener();
                    //try
                    //{
                    //    logInfo = new FileLogTraceListener();
                    //    logInfo = (FileLogTraceListener)AssignProperty();

                    //    logInfo.WriteLine("=====Exception Log Start DateTime:" + DateTime.UtcNow + "=======");
                    //    logInfo.WriteLine("");

                    //    logInfo.WriteLine("NameSpace : " + NameSpace);
                    //    logInfo.WriteLine("ClassName : " + ClassName);
                    //    logInfo.WriteLine("MethodName : " + MethodName);
                    //    logInfo.WriteLine("MethodArguments : " + MethodArguments);

                    //    logInfo.WriteLine("SOURCE    : " + ex.Source);
                    //    logInfo.WriteLine("Type      : " + ex.GetType().FullName);
                    //    logInfo.WriteLine("MESSAGE   : " + ex.Message);
                    //    logInfo.WriteLine("TARGETSITE: " + ex.TargetSite);
                    //    logInfo.WriteLine("STACKTRACE: " + ex.StackTrace);
                    //    if (ex.InnerException != null)
                    //        logInfo.WriteLine("InnerException: " + ex.InnerException);
                    //    logInfo.WriteLine("");
                    //    logInfo.WriteLine("=====Exception Log End=======");
                    //    logInfo.WriteLine("");
                    //    logInfo.WriteLine("");
                    //    //logInfo.Close();
                    //}
                    //finally
                    //{
                    //    logInfo.Close();
                    //}
                }
                else
                {
                    StringBuilder ExceptionMsg = new StringBuilder();
                    StringBuilder ExceptionType = new StringBuilder();
                    StringBuilder ExceptionSource = new StringBuilder();
                    ExceptionMsg.Append("ErrorLog:");
                    lock (this)
                    {
                        GetWriter();
                        try
                        {
                            MyWriter.WriteLine("=====Exception Log Start DateTime:" + DateTime.UtcNow + "=======");
                            MyWriter.WriteLine("");

                            MyWriter.WriteLine("NameSpace : " + NameSpace);
                            MyWriter.WriteLine("ClassName : " + ClassName);
                            MyWriter.WriteLine("MethodName : " + MethodName);
                            MyWriter.WriteLine("MethodArguments : " + MethodArguments);

                            MyWriter.WriteLine("SOURCE    : " + ex.Source);
                            ExceptionSource.Append(ex.Source);
                            MyWriter.WriteLine("Type      : " + ex.GetType().FullName);
                            ExceptionType.Append(ex.GetType().FullName);
                            MyWriter.WriteLine("MESSAGE   : " + ex.Message);
                            ExceptionMsg.Append(ex.Message);
                            MyWriter.WriteLine("TARGETSITE: " + ex.TargetSite);
                            MyWriter.WriteLine("STACKTRACE: " + ex.StackTrace);
                            if (ex.InnerException != null)
                            {
                                MyWriter.WriteLine("InnerException: " + ex.InnerException);
                                ExceptionMsg.Append("InnerException:" + ex.InnerException);
                            }
                            MyWriter.WriteLine("");
                            MyWriter.WriteLine("=====Exception Log End=======");
                            MyWriter.WriteLine("");
                            MyWriter.WriteLine("");
                        }

                        finally
                        {
                            MyWriter.Close();
                        }
                        _Base.SendExcepToDB(ExceptionMsg.ToString(), ExceptionType.ToString(), "", ExceptionSource.ToString());
                    }
                }
            }



            return true;
        }
        /// <summary>
        /// Creates a Log for Exception Raised
        /// </summary>
        /// <param name="ex">Exception Raised</param>
        /// <param name="strExceptionMsg">Exception Name or UserDefined Message</param>
        /// <returns>Boolean</returns>
        public bool createLog(Exception ex, string strExceptionMsg)
        {

            if (LoggingOn)
            {
                if (LogType)
                {
                    //FileLogTraceListener logInfo = new FileLogTraceListener();
                    //try
                    //{
                    //    logInfo = new FileLogTraceListener();
                    //    logInfo = (FileLogTraceListener)AssignProperty();

                    //    logInfo.WriteLine("=====Exception Log DateTime:" + DateTime.UtcNow + "=======");
                    //    logInfo.WriteLine(strExceptionMsg);
                    //    logInfo.WriteLine("");

                    //    logInfo.WriteLine("NameSpace : " + NameSpace);
                    //    logInfo.WriteLine("ClassName : " + ClassName);
                    //    logInfo.WriteLine("MethodName : " + MethodName);
                    //    logInfo.WriteLine("MethodArguments : " + MethodArguments);

                    //    logInfo.WriteLine("SOURCE    : " + ex.Source);
                    //    logInfo.WriteLine("MESSAGE   : " + ex.Message);
                    //    logInfo.WriteLine("TARGETSITE: " + ex.TargetSite);
                    //    logInfo.WriteLine("STACKTRACE: " + ex.StackTrace);
                    //    if (ex.InnerException != null)
                    //        logInfo.WriteLine("InnerException: " + ex.InnerException);
                    //    logInfo.WriteLine("");
                    //    logInfo.WriteLine("=====Exception Log End=======");
                    //    logInfo.WriteLine("");
                    //    logInfo.WriteLine("");
                    //}
                    //finally
                    //{
                    //    logInfo.Close();
                    //}
                }
                else
                {
                    StringBuilder ExceptionMsg = new StringBuilder();
                    StringBuilder ExceptionType = new StringBuilder();
                    StringBuilder ExceptionSource = new StringBuilder();
                    ExceptionMsg.Append("ErrorLog:");
                    lock (this)
                    {
                        GetWriter();
                        try
                        {
                            MyWriter.WriteLine("=====Exception Log DateTime:" + DateTime.UtcNow + "=======");
                            MyWriter.WriteLine(strExceptionMsg);
                            MyWriter.WriteLine("");

                            MyWriter.WriteLine("NameSpace : " + NameSpace);
                            MyWriter.WriteLine("ClassName : " + ClassName);
                            MyWriter.WriteLine("MethodName : " + MethodName);
                            MyWriter.WriteLine("MethodArguments : " + MethodArguments);

                            MyWriter.WriteLine("SOURCE    : " + ex.Source);
                            ExceptionSource.Append(ex.Source);
                            MyWriter.WriteLine("Type      : " + ex.GetType().FullName);
                            ExceptionType.Append(ex.GetType().FullName);
                            MyWriter.WriteLine("MESSAGE   : " + ex.Message);
                            ExceptionMsg.Append(ex.Message);
                            MyWriter.WriteLine("TARGETSITE: " + ex.TargetSite);
                            MyWriter.WriteLine("STACKTRACE: " + ex.StackTrace);
                            if (ex.InnerException != null)
                            {
                                MyWriter.WriteLine("InnerException: " + ex.InnerException);
                                ExceptionMsg.Append("InnerException:" + ex.InnerException);
                            }
                            MyWriter.WriteLine("");
                            MyWriter.WriteLine("=====Exception Log End=======");
                            MyWriter.WriteLine("");
                            MyWriter.WriteLine("");
                        }
                        finally
                        {
                            MyWriter.Close();
                        }

                        _Base.SendExcepToDB(ExceptionMsg.ToString(), ExceptionType.ToString(), strExceptionMsg, ExceptionSource.ToString());
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Creates User-Defined message which can be used in Exception Logs
        /// </summary>
        /// <param name="strMsg">Message</param>
        /// <returns>Boolean</returns>
        public bool createExMsgLog(string strMsg)
        {

            if (LoggingOn)
            {
                if (LogType)
                {
                    //FileLogTraceListener logInfo = new FileLogTraceListener();
                    //try
                    //{
                    //    logInfo = (FileLogTraceListener)AssignProperty();

                    //    logInfo.WriteLine("===" + strMsg + " At " + DateTime.UtcNow);
                    //    logInfo.WriteLine("");
                    //    //logInfo.Close();
                    //}
                    //finally
                    //{
                    //    logInfo.Close();
                    //}
                }
                else
                {
                    lock (this)
                    {
                        GetWriter();
                        try
                        {
                            MyWriter = new StreamWriter(FileLocation + "/" + FileName, true);

                            MyWriter.WriteLine("===" + strMsg + " At " + DateTime.UtcNow);
                            MyWriter.WriteLine("");
                            MyWriter.Close();
                        }
                        finally
                        {
                            MyWriter.Close();
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Should be used at the End of the Log
        /// </summary>
        /// <returns></returns>
        public bool EndLog()
        {

            if (LoggingOn)
            {
                if (LogType)
                {
                    //FileLogTraceListener logInfo = new FileLogTraceListener();
                    //try
                    //{
                    //    logInfo = new FileLogTraceListener();
                    //    logInfo = (FileLogTraceListener)AssignProperty();

                    //    logInfo.WriteLine("======================Logging Ended At:" + DateTime.UtcNow + "======================");
                    //    logInfo.WriteLine("");
                    //    logInfo.WriteLine("");
                    //    logInfo.WriteLine("");
                    //    logInfo.WriteLine("");
                    //    logInfo.Close();
                    //}
                    //finally
                    //{
                    //    logInfo.Close();
                    //}
                }
                else
                {
                    lock (this)
                    {
                        GetWriter();
                        try
                        {
                            MyWriter = new StreamWriter(FileLocation + "/" + FileName, true);

                            MyWriter.WriteLine("======================Logging Ended At:" + DateTime.UtcNow + "======================");
                            MyWriter.WriteLine("");
                            MyWriter.WriteLine("");
                            MyWriter.WriteLine("");
                            MyWriter.WriteLine("");
                            //MyWriter.Close();
                        }
                        finally
                        {
                            MyWriter.Close();
                        }
                    }
                }
            }

            return true;
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// Creates a FileTraceListener object and assigns a File name & path to it properties.
        /// </summary>
        /// <returns>FileTraceListener Object</returns>
        //private FileLogTraceListener AssignProperty()
        //{
        //    try
        //    {
        //        FileLogTraceListener logInfo;
        //        logInfo = new FileLogTraceListener();

        //        logInfo.Append = FileAppend;
        //        logInfo.BaseFileName = FileName;
        //        logInfo.CustomLocation = FileLocation;
        //        logInfo.Location = Microsoft.VisualBasic.Logging.LogFileLocation.Custom;
        //        logInfo.AutoFlush = true;
        //        return logInfo;
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        /// <summary>
        /// Assigns a StreamWriter
        /// </summary>
        private void GetWriter()
        {
            lock (this)
            {
                CloseFile();
                _Base.CreateIfMissing(FileLocation);
                MyWriter = new StreamWriter(FileLocation + "/" + FileName, FileAppend);
            }
        }

        /// <summary>
        /// Closes a StreamWriter
        /// </summary>
        private void CloseFile()
        {
            if (MyWriter != null)
            {
                try
                {
                    MyWriter.Close();
                }
                catch
                {
                }
            }
        }

        /// <summary>
        /// Gets the name of NameSpace, ClassName, MethodName and its Arguments Name
        /// </summary>
        private void InitializeName()
        {
            string strParam = "";

            StackTrace objSTrace = new StackTrace(true);
            if (objSTrace.FrameCount >= 3)
            {
                StackFrame objSFrame = objSTrace.GetFrame(3);

                //Gets the MethodBase
                MethodBase objMethodBase = objSFrame.GetMethod();
                ParameterInfo[] objParamInfo = objMethodBase.GetParameters();


                //Gets the Parameter of the Method
                foreach (ParameterInfo objParam in objParamInfo)
                {
                    if (strParam != "")
                        strParam += objParam.ParameterType.Name;
                    else
                        strParam += " , " + objParam.ParameterType.Name;

                    strParam += " , " + objParam.Name;
                }

                //Gets the NameSpace Name
                NameSpace = objMethodBase.ReflectedType.Namespace;
                //Gets the ClassName 
                ClassName = objMethodBase.ReflectedType.Name;
                //Gets the Method Name
                MethodName = objMethodBase.Name;
                //Gets the Method Parameter Name
                MethodArguments = strParam == "" ? "No Arguments" : strParam;

            }
        }
        #endregion

        #region UnUsed Code
        //private object AssignProperty(bool blnType)
        //{

        //        if (blnType)
        //        {
        //            try
        //            {
        //                FileLogTraceListener logInfo;
        //                logInfo = new FileLogTraceListener();

        //                logInfo.Append = FileAppend;
        //                logInfo.BaseFileName = FileName;
        //                logInfo.CustomLocation = FileLocation;
        //                logInfo.Location = Microsoft.VisualBasic.Logging.LogFileLocation.Custom;
        //                logInfo.AutoFlush = true;
        //                return logInfo;
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }

        //        }
        //        else
        //        {
        //            try
        //            {
        //                if (!File.Exists(FileLocation + "/" + FileName))
        //                {
        //                    FileStream fs = new FileStream(FileLocation + "/" + FileName, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        //                    fs.Close();
        //                }
        //                StreamWriter logInfo = new StreamWriter(FileLocation + "/" + FileName, true);
        //                return logInfo;
        //            }
        //            catch (Exception ex)
        //            {
        //                throw ex;
        //            }
        //        }

        //    } 
        #endregion

    }
}