using BE_07_24.DataAccess.DALImpl;
using BE_07_24.DataAccess.DO;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_07_24_ConsoleApp
{
    public class Programs_SanXuat
    {
        private static NhanVienServicesDALImpl nhanVienServices = new NhanVienServicesDALImpl();
        public static void MeNu_SanXuat()
        {
            while(true)
            {
                Console.Clear();
                Console.WriteLine("=== QUẢN LÝ SẢN XUẤT ===");
                Console.WriteLine("1. Thêm nhân viên.");
                Console.WriteLine("2. Tạo sản lượng theo công đoạn của từng nhân viên.");
                Console.WriteLine("3. Xuất và hiển thị báo cáo.");
                Console.WriteLine("4. Thoát.");
                Console.Write("Chọn một chức năng (1-4): ");
                var choice = Console.ReadLine();
                switch(choice)
                {
                    case "1":
                        NhanVienInPut();
                        break;
                    case "2":
                        SanLuongInPutToNV();
                        break;
                    case "3":
                        XuatBaoCaoSanXuat();
                        break;
                    case "4":
                        Console.WriteLine("Thoát chương trình");
                        break;
                    default:
                        Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng chọn lại.");
                        break;
                }

                Console.WriteLine("Nhấn phím bất kỳ để tiếp tục...");
                Console.ReadKey();
            }
        }
        // hàm nhập nhân viên 
        public static void NhanVienInPut()
        {
            try
            {
                Console.WriteLine("Nhập vào ID nhân viên :");
                int id = int.Parse(Console.ReadLine());
                Console.WriteLine("Nhập vào Tên Nhân Viên :");
                string ten = Console.ReadLine();
                Console.WriteLine("Nhập vào giới tính (Nam/Nữ)");
                string gioitinh = Console.ReadLine();
                Console.WriteLine("Nhập vào tuổi:");
                int tuoi = int.Parse(Console.ReadLine());
                Console.WriteLine("Nhập vào lương cơ bản:");
                double luongCoBan = double.Parse(Console.ReadLine());
                Console.WriteLine("Nhập vào hệ số lương:");
                float heSoLuong = float.Parse(Console.ReadLine());
                Console.WriteLine("Nhập vào phụ cấp:");
                double phuCap = double.Parse(Console.ReadLine());
                // Tạo đối tượng NhanVien
                NhanVien nhanvien = new NhanVien(id, ten, gioitinh, tuoi, luongCoBan, heSoLuong, phuCap);
                // check dữ liệu cho nhanvien
                var returnData = nhanVienServices.NhanVienInsert(nhanvien);
                // hiển thị kết quả
                Console.WriteLine(returnData.ReturnMsg);
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Dữ liệu nhập vào không hợp lệ. Vui lòng kiểm tra lại.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
        }
        // Hàm tạo sản lượng cho nhân viên 
        public static void SanLuongInPutToNV()
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                Console.WriteLine("Nhập vào ID nhân viên:");
                int id = int.Parse(Console.ReadLine());

                // Kiểm tra nếu nhân viên tồn tại
                var nhanVienResult = nhanVienServices.TimKiemNhanVien(new NhanVien(id, "", "", 0, 0, 0, 0));
                if (nhanVienResult.ReturnCode != 1)
                {
                    Console.WriteLine(nhanVienResult.ReturnMsg);
                    return; // Kết thúc hàm nếu không tìm thấy nhân viên
                }

                Console.WriteLine("Nhập vào số lượng công đoạn:");
                int numProcesses = int.Parse(Console.ReadLine());
                List<CongDoanSanXuat> congDoanSanXuats = new List<CongDoanSanXuat>();

                for (int i = 0; i < numProcesses; i++)
                {
                    Console.WriteLine($"Nhập vào mã công đoạn {i + 1}:");
                    string macongdoan = Console.ReadLine();
                    Console.WriteLine($"Nhập vào tên công đoạn {i + 1}:");
                    string tencongdoan = Console.ReadLine();
                    Console.WriteLine($"Nhập vào số lượng sản phẩm sản xuất {i + 1}:");
                    double soluongsanpham = double.Parse(Console.ReadLine());

                    CongDoanSanXuat congDoanSanXuat = new CongDoanSanXuat
                    {
                        Ma_Cong_Doan = macongdoan,
                        Ten_Cong_Doan = tencongdoan,
                        So_Luong_San_Pham = soluongsanpham
                    };
                    congDoanSanXuats.Add(congDoanSanXuat);

                    // Hiển thị thông tin công đoạn sản xuất để kiểm tra
                    Console.WriteLine($"Công đoạn {i + 1}: Mã: {macongdoan}, Tên: {tencongdoan}, Số lượng: {soluongsanpham}");
                }

                // Khởi tạo giá cho từng công đoạn
                Dictionary<string, double> pricePerProcess = new Dictionary<string, double>
        {
            { "may", 10000 },  // Giá cho công đoạn "may"
            { "cat", 20000 },  // Giá cho công đoạn "cắt"
            { "va", 15000 }    // Giá cho công đoạn "vá"
        };

                // Thực hiện tạo sản lượng cho nhân viên đã được tìm thấy
                var nhanVien = nhanVienResult.Data as NhanVien; // Lấy thông tin nhân viên
                var taoSanLuongResult = nhanVienServices.TaoSanLuongNhanVien(nhanVien, congDoanSanXuats, pricePerProcess);
                Console.WriteLine(taoSanLuongResult.ReturnMsg);

            }
            catch (FormatException ex)
            {
                Console.WriteLine("Dữ liệu nhập vào không hợp lệ. Vui lòng kiểm tra lại.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
        }


        // hàm xuất báo cáo cho nhân viên 
        public static void XuatBaoCaoSanXuat()
        {
            Console.OutputEncoding = Encoding.UTF8;
            try
            {
                Console.WriteLine("Chọn một tùy chọn để xuất báo cáo:");
                Console.WriteLine("1. Xuất báo cáo cho từng nhân viên.");
                Console.WriteLine("2. Xuất báo cáo cho tất cả nhân viên.");
                Console.Write("Chọn một chức năng (1-2): ");
                var choice = Console.ReadLine();

                if (choice == "1")
                {
                    Console.WriteLine("Nhập vào ID nhân viên để xuất báo cáo:");
                    int id = int.Parse(Console.ReadLine());

                    // Tìm kiếm nhân viên
                    var nhanVienResult = nhanVienServices.TimKiemNhanVien(new NhanVien(id, "", "", 0, 0, 0, 0));
                    if (nhanVienResult.ReturnCode != 1)
                    {
                        Console.WriteLine(nhanVienResult.ReturnMsg);
                        return;
                    }
                    var nhanVien = nhanVienResult.Data as NhanVien; // Lấy thông tin nhân viên

                    // In dữ liệu nhân viên ra màn hình để kiểm tra
                    Console.WriteLine("Dữ liệu nhân viên:");
                    Console.WriteLine($"ID: {nhanVien.GetId()}");
                    Console.WriteLine($"Tên: {nhanVien.GetTen()}");
                    Console.WriteLine("Danh sách công đoạn sản xuất:");
                    foreach (var cd in nhanVien.congDoanSanXuats)
                    {
                        Console.WriteLine($"Mã công đoạn: {cd.Ma_Cong_Doan}, Tên công đoạn: {cd.Ten_Cong_Doan}, Số lượng: {cd.So_Luong_San_Pham}");
                    }

                    // Xuất báo cáo sản xuất cho nhân viên cụ thể
                    Console.WriteLine("Xuất báo cáo sản xuất...");
                    var returnData = nhanVienServices.XuatBaoCaoSanLuong(nhanVien); // Truyền đối tượng NhanVien vào
                    Console.WriteLine(returnData.ReturnMsg);
                }
                else if (choice == "2")
                {
                    // Xuất báo cáo tổng hợp cho tất cả nhân viên
                    Console.WriteLine("Xuất báo cáo tổng hợp cho tất cả nhân viên...");
                    var returnData = nhanVienServices.XuatBaoCaoSanLuong(); // Không truyền đối tượng NhanVien
                    Console.WriteLine(returnData.ReturnMsg);
                }
                else
                {
                    Console.WriteLine("Lựa chọn không hợp lệ. Vui lòng chọn lại.");
                }
            }
            catch (FormatException ex)
            {
                Console.WriteLine("Dữ liệu nhập vào không hợp lệ. Vui lòng kiểm tra lại.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi: {ex.Message}");
            }
        }

    }
}
