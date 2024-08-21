using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_07_24.DataAccess.DO
{
    public class ReturnData
    {
        public int ReturnCode { get; set; }
        public string ReturnMsg { get; set; }
        public string Extend { get; set; }
       
        // chứa thông tin nhân viên 
        public NhanVien Data { get; set; } 
        // chứa dữ liệu tổng hợp 
        public object AdditionData { get; set; }
    }
}
