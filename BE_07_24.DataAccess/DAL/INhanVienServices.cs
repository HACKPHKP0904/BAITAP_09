using BE_07_24.DataAccess.DO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_07_24.DataAccess.DAL
{
    public interface INhanVienServices
    {
        ReturnData NhanVienInsert (NhanVien nhanVien);
        ReturnData TimKiemNhanVien(NhanVien nhanVien);
        ReturnData TaoSanLuongNhanVien(NhanVien nhanVien, List<CongDoanSanXuat> congDoanSanXuats, Dictionary<string, double> pricePerProcess);
        ReturnData XuatBaoCaoSanLuong(NhanVien nhanVien);
        ReturnData XuatBaoCaoSanLuong();
    }
}
