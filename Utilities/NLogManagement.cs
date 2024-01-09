using NLog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public class NLogManagement
    {
        //public static Logger UserLog = NLog.LogManager.GetLogger("SystemLog");
        public static Logger SystemLog = NLog.LogManager.GetLogger("SystemLog");
        public static Logger _DBLog = NLog.LogManager.GetLogger("DBLog");
        public static Logger _TradeLog = NLog.LogManager.GetLogger("TradeLog");
        public static Logger _ExerciseLog = NLog.LogManager.GetLogger("ExerciseLog");
        public static Dictionary<string, Logger> DictLogger = new Dictionary<string, Logger>();
        static public void TradeLog(string message)
        {
            _TradeLog.Info(message);
        }
        static public void DBLogInfo(string message)
        {
            _DBLog.Info(message);
        }
        static public void SystemLogInfo(string message)
        {
            SystemLog.Info(message);
        }
        static public void SystemLogError(string message)
        {
            SystemLog.Error(message);
        }
        static public void ExerciseLog(string message)
        {
            _ExerciseLog.Info(message);
        }
        static public void CreateLog(string logname)
        {
            if (DictLogger.ContainsKey(logname)==false) {
                Logger templog = NLog.LogManager.GetLogger(logname);
                DictLogger.Add(logname,templog);
            }
        }
        static public void WriteLogInfo(string logname,string message)
        {
            if (DictLogger.ContainsKey(logname) )
            {
                DictLogger[logname].Info(message);
            }
        }
    }
}
