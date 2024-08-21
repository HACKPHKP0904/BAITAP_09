using BE_07_24.DataAccess.DAL;
using BE_07_24.DataAccess.DO;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BE_07_24.DataAccess.DALImpl
{
    public class NhanVienServicesDALImpl : INhanVienServices
    {
        // viết hàm add nhân viên vào 
        List<NhanVien> danhsachNhanVien = new List<NhanVien>();


        public ReturnData NhanVienInsert(NhanVien nhanVien)
        {
            var returnData = new ReturnData();
            try
            {
                // Check null
                if (nhanVien == null || string.IsNullOrEmpty(nhanVien.GetTen()))
                {
                    returnData.ReturnCode = -1;
                    returnData.ReturnMsg = "Dữ liệu đầu vào không hợp lệ";
                    return returnData;
                }

                // Validate input data
                if (!BE_07_24.Common.ValidateData.CheckNull_Data(nhanVien.GetTen())
                    || !BE_07_24.Common.ValidateData.CheckLength_Name(nhanVien.GetTen())
                    || !BE_07_24.Common.ValidateData.CheckXSSInput(nhanVien.GetTen()))
                {
                    returnData.ReturnCode = -2;
                    returnData.ReturnMsg = "Dữ liệu đầu vào không hợp lệ";
                    return returnData;
                }

                // Check for duplicates
                if (danhsachNhanVien != null && danhsachNhanVien.Count > 0)
                {
                    var nhanvienlDb = danhsachNhanVien.FirstOrDefault(s => s.GetId() == nhanVien.GetId());
                    if (nhanvienlDb != null)
                    {
                        // Employee already exists
                        returnData.ReturnCode = -3;
                        returnData.ReturnMsg = "Nhân viên này đã tồn tại";
                        return returnData;
                    }
                }

                // Add new employee
                danhsachNhanVien.Add(nhanVien);
                returnData.ReturnCode = 1;
                returnData.ReturnMsg = "Thêm nhân viên thành công";
                return returnData;
            }
            catch (Exception ex)
            {
                returnData.ReturnCode = -99;
                returnData.ReturnMsg = ex.Message;
                return returnData;
            }
        }
        //
        public ReturnData TimKiemNhanVien(NhanVien nhanVien)
        {
            var returnData = new ReturnData();
            //kiểm tra danh sách nhân viên
            try
            {
                if (danhsachNhanVien == null || danhsachNhanVien.Count == 0)
                {
                    returnData.ReturnCode = -1;
                    returnData.ReturnMsg = "Danh sách nhân viên trống";
                    return returnData;
                }
                // Tìm nhân viên theo ID
                var nhanvienlDB = danhsachNhanVien.FirstOrDefault(s => s.GetId() == nhanVien.GetId());
                if (nhanvienlDB == null)
                {
                    returnData.ReturnCode = -2;
                    returnData.ReturnMsg = "Không tìm thấy nhân viên";
                    return returnData;
                }
                else
                {
                    returnData.ReturnCode = 1;
                    returnData.ReturnMsg = " Đã tìm thấy nhân viên ";
                    returnData.Data = nhanvienlDB; // trả về thông tin nhân viên tìm được
                }
                return returnData;
            }
            catch (Exception ex)
            {
                returnData.ReturnCode = -98;
                returnData.ReturnMsg = ex.Message;
                return returnData;
            }


        }
        //
        public ReturnData TaoSanLuongNhanVien(NhanVien nhanVien, List<CongDoanSanXuat> congDoanSanXuats, Dictionary<string, double> pricePerProcess)
        {
            var returnData = new ReturnData();
            try
            {
                if (nhanVien == null || congDoanSanXuats == null || congDoanSanXuats.Count == 0)
                {
                    returnData.ReturnCode = -1;
                    returnData.ReturnMsg = "Dữ liệu đầu vào không hợp lệ";
                    return returnData;
                }

                // Cập nhật danh sách công đoạn sản xuất cho nhân viên
                var nhanVienFound = danhsachNhanVien.FirstOrDefault(nv => nv.GetId() == nhanVien.GetId());
                if (nhanVienFound == null)
                {
                    returnData.ReturnCode = -2;
                    returnData.ReturnMsg = "Nhân viên không tồn tại";
                    return returnData;
                }
                nhanVienFound.congDoanSanXuats = congDoanSanXuats;

                double totalQty = 0;
                double totalSum = 0;
                foreach (var item in congDoanSanXuats)
                {
                    double price = pricePerProcess.ContainsKey(item.Ma_Cong_Doan) ? pricePerProcess[item.Ma_Cong_Doan] : 0;
                    double total = item.So_Luong_San_Pham * price;
                    totalQty += item.So_Luong_San_Pham;
                    totalSum += total;
                }

                returnData.ReturnCode = 1;
                returnData.ReturnMsg = "Đã thêm số lượng thành công và tính tổng";
                returnData.AdditionData = new
                {
                    TotalQty = totalQty,
                    totalSum = totalSum
                };

            }
            catch (Exception ex)
            {
                returnData.ReturnCode = -97;
                returnData.ReturnMsg = ex.Message;
                return returnData;
            }
            return returnData;
        }
        //
        public ReturnData XuatBaoCaoSanLuong(NhanVien nhanVien)
        {
            var returnData = new ReturnData();
            try
            {
                if (nhanVien == null)
                {
                    returnData.ReturnCode = -1;
                    returnData.ReturnMsg = "Dữ liệu nhân viên không hợp lệ";
                    return returnData;
                }

                // Lấy nhân viên từ danh sách nhân viên
                var nhanVienFound = danhsachNhanVien.FirstOrDefault(nv => nv.GetId() == nhanVien.GetId());
                if (nhanVienFound == null)
                {
                    returnData.ReturnCode = -2;
                    returnData.ReturnMsg = "Không tìm thấy nhân viên";
                    return returnData;
                }

                // Khởi tạo giá cho từng công đoạn
                Dictionary<string, double> pricePerProcess = new Dictionary<string, double>
        {
            { "may", 10000 },
            { "cat", 20000 },
            { "va", 15000 }
        };

                // Đường dẫn lưu file
                string filePath = @"C:\Users\phi16\source\repos\BE_07_24_QuanLySanXuat\BaoCaoSanXuat.xlsx";

                using (var workBook = new XLWorkbook())
                {
                    var workSheet = workBook.Worksheets.Add("Báo cáo sản xuất");

                    workSheet.Cell(1, 1).Value = "Name";
                    workSheet.Cell(1, 2).Value = "Process";
                    workSheet.Cell(1, 3).Value = "Process_Name";
                    workSheet.Cell(1, 4).Value = "Qty";
                    workSheet.Cell(1, 5).Value = "Price";
                    workSheet.Cell(1, 6).Value = "Total";

                    int row = 2; // Bắt đầu từ hàng thứ hai để ghi dữ liệu
                    double grandTotalQty = 0;
                    double grandTotalSum = 0;

                    foreach (var cd in nhanVienFound.congDoanSanXuats)
                    {
                        double price = pricePerProcess.ContainsKey(cd.Ten_Cong_Doan) ? pricePerProcess[cd.Ten_Cong_Doan] : 0;
                        double total = cd.So_Luong_San_Pham * price;

                        workSheet.Cell(row, 1).Value = nhanVienFound.GetTen();
                        workSheet.Cell(row, 2).Value = cd.Ma_Cong_Doan;
                        workSheet.Cell(row, 3).Value = cd.Ten_Cong_Doan;
                        workSheet.Cell(row, 4).Value = cd.So_Luong_San_Pham;
                        workSheet.Cell(row, 5).Value = price;
                        workSheet.Cell(row, 6).Value = total;

                        grandTotalQty += cd.So_Luong_San_Pham;
                        grandTotalSum += total;
                        row++;
                    }

                    // Thêm dòng tổng cộng vào
                    workSheet.Cell(row, 3).Value = "Tổng cộng";
                    workSheet.Cell(row, 4).Value = grandTotalQty;
                    workSheet.Cell(row, 6).Value = grandTotalSum;

                    try
                    {
                        workBook.SaveAs(filePath);
                        returnData.ReturnCode = 1;
                        returnData.ReturnMsg = $"Xuất báo cáo thành công tại {filePath}";
                    }
                    catch (IOException ioEx)
                    {
                        returnData.ReturnCode = -96;
                        returnData.ReturnMsg = $"Lỗi khi lưu file: {ioEx.Message}. Vui lòng kiểm tra xem file có đang được mở hay không.";
                    }
                }
            }
            catch (Exception ex)
            {
                returnData.ReturnCode = -95;
                returnData.ReturnMsg = ex.Message;
            }
            return returnData;
        }

        public ReturnData XuatBaoCaoSanLuong()
        {
            var returnData = new ReturnData();
            // Khởi tạo giá cho từng công đoạn
            Dictionary<string, double> pricePerProcess = new Dictionary<string, double>
        {
            { "may", 10000 },
            { "cat", 20000 },
            { "va", 15000 }
        };
            try
            {
                // Đường dẫn lưu file
                string filePath = @"C:\Users\phi16\source\repos\BE_07_24_QuanLySanXuat\BaoCaoSanXuat2.xlsx";

                using (var workBook = new XLWorkbook())
                {
                    var workSheet = workBook.Worksheets.Add("Báo cáo sản xuất");

                    workSheet.Cell(1, 1).Value = "Name";
                    workSheet.Cell(1, 2).Value = "Process";
                    workSheet.Cell(1, 3).Value = "Process_Name";
                    workSheet.Cell(1, 4).Value = "Qty";
                    workSheet.Cell(1, 5).Value = "Price";
                    workSheet.Cell(1, 6).Value = "Total";

                    int row = 2; // Bắt đầu từ hàng thứ hai để ghi dữ liệu
                    double grandTotalQty = 0;
                    double grandTotalSum = 0;

                    // Lặp qua tất cả nhân viên
                    foreach (var nhanVien in danhsachNhanVien)
                    {
                        foreach (var cd in nhanVien.congDoanSanXuats)
                        {
                            double price = pricePerProcess.ContainsKey(cd.Ten_Cong_Doan) ? pricePerProcess[cd.Ten_Cong_Doan] : 0;
                            double total = cd.So_Luong_San_Pham * price;

                            workSheet.Cell(row, 1).Value = nhanVien.GetTen();
                            workSheet.Cell(row, 2).Value = cd.Ma_Cong_Doan;
                            workSheet.Cell(row, 3).Value = cd.Ten_Cong_Doan;
                            workSheet.Cell(row, 4).Value = cd.So_Luong_San_Pham;
                            workSheet.Cell(row, 5).Value = price;
                            workSheet.Cell(row, 6).Value = total;

                            grandTotalQty += cd.So_Luong_San_Pham;
                            grandTotalSum += total;
                            row++;
                        }
                    }

                    // Thêm dòng tổng cộng vào
                    workSheet.Cell(row, 3).Value = "Tổng cộng";
                    workSheet.Cell(row, 4).Value = grandTotalQty;
                    workSheet.Cell(row, 6).Value = grandTotalSum;

                    try
                    {
                        workBook.SaveAs(filePath);
                        returnData.ReturnCode = 1;
                        returnData.ReturnMsg = $"Xuất báo cáo tổng hợp thành công tại {filePath}";
                    }
                    catch (IOException ioEx)
                    {
                        returnData.ReturnCode = -96;
                        returnData.ReturnMsg = $"Lỗi khi lưu file: {ioEx.Message}. Vui lòng kiểm tra xem file có đang được mở hay không.";
                    }
                }
            }
            catch (Exception ex)
            {
                returnData.ReturnCode = -95;
                returnData.ReturnMsg = ex.Message;
            }
            return returnData;
        }




    }
}
