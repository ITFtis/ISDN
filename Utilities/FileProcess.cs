using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class FileProcess
    {

        public static void  deleteFile(string uploadPath, params string[] fileName)
        {
            string x = null;
            foreach (string x_loopVariable in fileName)
            {
                x = x_loopVariable;
                if (((x != null) && x.Length > 0))
                {
                    try
                    {
                        File.Delete(uploadPath + x);
                    }
                    catch (Exception ex)
                    {
                        NLogManagement.SystemLogError("deleteFileError:" + ex.Message);
                    }
                }
            }
        }
    }
}
