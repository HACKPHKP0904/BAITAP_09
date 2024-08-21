using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_07_24.Common
{
    public static class ValidateData
    {
        // check null 
        public static bool CheckNull_Data(string input)
        {
            return string.IsNullOrEmpty(input) ? false : true;
        }
        // check độ dài tên 
        public static bool CheckLength_Name (string ten)
        {
            return ten.Length > 100 ? false : true;
        }
        // check số 
        public static bool IsNumberic ( string input)
        {
            int n;
            bool isNumberic = int.TryParse(input,out n);
            return isNumberic; 
        }
        // check XSSI
        public static bool CheckXSSInput(string input)
        {
            try
            {
                var listdangerousString = new List<string> { "<applet", "<body", "<embed", "<frame", "<script", "<frameset", "<html", "<iframe", "<img", "<style", "<layer", "<link", "<ilayer", "<meta", "<object", "<h", "<input", "<a", "&lt", "&gt" };
                if (string.IsNullOrEmpty(input))
                {
                    return false;
                }
                foreach (var dangerous in listdangerousString)
                {
                    if (input.Trim().ToLower().IndexOf(dangerous) >= 0)
                    {
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
    }
}
