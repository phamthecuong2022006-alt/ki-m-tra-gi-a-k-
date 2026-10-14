# BIT240049 - Đề kiểm tra giữa kỳ: Quản lý món ăn (ASP.NET Core MVC + EF Core)

## Cách chạy

1. Mở `Database/BIT240049_MID.sql` trong SSMS, bấm **Ctrl + A** rồi **F5** (tạo database `BIT240049_MID`, 3 bảng, dữ liệu mẫu).
2. Sửa chuỗi kết nối trong `appsettings.json` cho đúng máy của bạn:
   - SQL Server mặc định: `Server=localhost;...`
   - SQL Express: `Server=.\\SQLEXPRESS;...`
   - LocalDB: `Server=(localdb)\\MSSQLLocalDB;...`
3. Kiểm tra `<TargetFramework>` trong `BIT240049_Mid.csproj` đúng với .NET SDK trên máy (`dotnet --version`).
   Nếu là .NET 9 thì đổi `net8.0` -> `net9.0` và `Version="8.0.*"` -> `Version="9.0.*"` (tương tự với .NET 10).
4. Chạy:
   ```bash
   dotnet restore
   dotnet run
   ```
   Mở địa chỉ hiện trên terminal (vd: http://localhost:5xxx).

**Không** chạy `dotnet ef migrations` / `dotnet ef database update` vì database đã được tạo bằng script SQL.
Giao diện dùng Bootstrap qua CDN nên cần có Internet khi demo.

## Cấu trúc

| Thư mục / file | Nội dung |
|---|---|
| `Models/` | `DishCategory_BIT240049`, `Dish_BIT240049`, `DishImage_BIT240049` (validation + ánh xạ bảng/cột có tiền tố MSSV) |
| `Data/AppDbContext.cs` | Quan hệ 1-n, Restrict khi xóa loại, Cascade khi xóa món |
| `Controllers/DishesController.cs` | Danh sách + lọc ở tầng DB, Create/Edit/Detail/Delete, thêm ảnh, đổi ảnh đại diện |
| `Controllers/DishCategoriesController.cs` | CRUD loại món, chặn xóa loại đang có món |
| `Controllers/HomeController.cs` | Đổi ngôn ngữ (cookie), trang lỗi |
| `Resources/SharedResource.resx` / `.vi.resx` | Toàn bộ chuỗi English / Tiếng Việt |
| `Views/` | Giao diện Razor |

## Checklist demo (theo mục 5 của đề)

1. SSMS: show DB `BIT240049_MID`, 3 bảng, các cột có tiền tố `BIT240049_`; mở code show tên Model.
2. Trang Dishes: tìm theo tên, lọc loại + trạng thái, khoảng giá, sắp xếp; bấm Filter rồi xem điều kiện vẫn còn trên form. Thử giá từ 100000 đến 5000 để thấy báo lỗi khoảng giá.
3. Cột "Preparation level": Quick (<= 15 phút) / Regular (> 15 phút). Thử Create / Edit với dữ liệu sai (tên trống, giá 0, thời gian 0, chưa chọn loại, trùng tên trong cùng loại).
4. Detail: thêm ảnh bằng URL, xem nhiều ảnh, bấm "Set as thumbnail" để đổi ảnh đại diện. Món "Trà đá" / "Chè thái"... chưa có ảnh để demo trường hợp không có ảnh.
5. Categories: bấm Delete loại đang có món (báo lỗi). Vào `/Dishes/Detail/9999`, `/Dishes/Edit/9999`, `/DishCategories/Edit/9999` (trang 404).
6. Lần đầu mở web phải là English (xóa cookie `.AspNetCore.Culture` trước khi quay video). Bấm "Tiếng Việt", kiểm tra menu, nhãn, validation, trạng thái, Quick/Regular; chuyển giữa List / Create / Edit / Detail vẫn giữ tiếng Việt; đổi ngôn ngữ ở trang danh sách khi đang lọc vẫn giữ điều kiện lọc.

## Lưu ý

- Đề cho phép dùng AI nhưng giảng viên có thể yêu cầu giải thích/sửa trực tiếp code. Nên đọc kỹ ba phần: truy vấn lọc (`DishesController.Index`), đổi ảnh đại diện (`SetThumbnail`), và đa ngôn ngữ (`Program.cs`, `HomeController.SetLanguage`, file resx).
- Giá hiển thị theo ngôn ngữ (50,000 hoặc 50.000) nhưng cách nhập số luôn dùng dấu chấm cho phần thập phân.
