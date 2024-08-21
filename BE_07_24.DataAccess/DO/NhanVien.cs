using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_07_24.DataAccess.DO
{
    /*
     * Nguyễn Phi Hùng
     * Ngày viết : 21/08/2024
     * Ngày sửa : 
     * Lí do sửa : 
     */
    public  class NhanVien
    {
        private int Id { get; set; }
        private string Ten { get; set; }  
        public string Gioi_Tinh { get; set; }
        public int Tuoi { get; set; }
        public double Luong_Co_Ban { get; set; }
        public float He_So_Luong { get ; set; }
        private double Phu_Cap { get; set ; }
        private double Tong_Luong { get; set; }

        public List<CongDoanSanXuat> congDoanSanXuats { get; set; } = new List<CongDoanSanXuat>();
        public NhanVien(int id, string ten, string gioiTinh, int tuoi, double luongCoBan, float heSoLuong, double phuCap)
        {
            Id = id;
            Ten = ten;
            Gioi_Tinh = gioiTinh;
            Tuoi = tuoi;
            Luong_Co_Ban = luongCoBan;
            He_So_Luong = heSoLuong;
            Phu_Cap = phuCap;
            Tong_Luong = CalculateTongLuong();
        }
        // hàm để trả về dữ liệu có tính đóng gói 
        public int GetId()
        {
            return Id;
        }
        public string GetTen()
        {
            return Ten;
        }
        public double GetPhuCap()
        {
            return Phu_Cap;
        }
        public double GetTongLuong()
        {
            return Tong_Luong;
        }
        private double CalculateTongLuong()
        {
            return Luong_Co_Ban * He_So_Luong + Phu_Cap;
        }
    }
}
