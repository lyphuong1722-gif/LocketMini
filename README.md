# 📷 LocketMini
LocketMini là một ứng dụng mạng xã hội được xây dựng nhằm mô phỏng các chức năng cơ bản của nền tảng chia sẻ hình ảnh và kết nối bạn bè. Hệ thống cho phép người dùng đăng ký tài khoản, đăng nhập, đăng bài viết kèm hình ảnh, bình luận, kết bạn, tìm kiếm người dùng và quản lý tài khoản cá nhân.
Dự án được phát triển bằng **ASP.NET Core MVC**, áp dụng mô hình **Clean Architecture** nhằm tách biệt các tầng xử lý, giúp hệ thống dễ bảo trì, mở rộng và phát triển trong tương lai.

## 🚀 Công nghệ sử dụng
- ASP.NET Core MVC
- C#
- Entity Framework Core
- SQL Server
- HTML5
- CSS3
- Bootstrap 5
- JavaScript
- Clean Architecture

## ✨ Chức năng chính
- 🔐 Đăng ký tài khoản
- 🔑 Đăng nhập / Đăng xuất
- 👤 Quản lý thông tin người dùng
- 📷 Đăng bài viết kèm hình ảnh
- ❤️ Thích bài viết
- 💬 Bình luận bài viết
- 👥 Kết bạn
- 🔍 Tìm kiếm người dùng
- 🔒 Thay đổi mật khẩu

## 📂 Cấu trúc dự án
```
LocketMini
│
├── LocketSystem.Application
├── LocketSystem.Domain
├── LocketSystem.Infrastructure
├── LocketSystem.Web
└── LocketSystem.sln
```
- **Application**: Chứa nghiệp vụ của hệ thống.
- **Domain**: Chứa Entity, Interface và Business Model.
- **Infrastructure**: Xử lý truy cập cơ sở dữ liệu và các dịch vụ.
- **Web**: Giao diện người dùng và Controller.

## ⚙️ Yêu cầu hệ thống
- Visual Studio 2022
- .NET 8 SDK
- SQL Server 2019 hoặc SQL Server Express
- SQL Server Management Studio (SSMS)

## 📥 Cài đặt
### Bước 1. Clone dự án
```bash
git clone https://github.com/lyphuong1722-gif/LocketMini.git
```
### Bước 2. Mở Solution
Mở file
LocketSystem.sln
bằng Visual Studio 2022.
### Bước 3. Cấu hình Database
Mở file
LocketSystem.Web/appsettings.json
Cập nhật chuỗi kết nối SQL Server:
```json
"ConnectionStrings": {
  "DefaultConnection": "Server=.;Database=LocketMini;Trusted_Connection=True;TrustServerCertificate=True;"
}
### Bước 4. Tạo cơ sở dữ liệu
Mở **Package Manager Console**
```powershell
Update-Database
```
hoặc
```bash
dotnet ef database update
```
### Bước 5. Chạy chương trình
Đặt
```
LocketSystem.Web
```
làm Startup Project.
Nhấn **F5** hoặc **Ctrl + F5**.
Website sẽ chạy tại:
```
https://localhost:xxxx
```

## 🗄️ Cơ sở dữ liệu
Hệ thống sử dụng SQL Server với các bảng chính:
- Users
- Profiles
- Posts
- Comments
- Friends
- Likes
- Notifications

## 👨‍💻 Kiến trúc hệ thống
Dự án được xây dựng theo mô hình **Clean Architecture**, gồm bốn tầng:
- Domain
- Application
- Infrastructure
- Web
Mỗi tầng có nhiệm vụ riêng nhằm giảm sự phụ thuộc giữa các thành phần và tăng khả năng mở rộng hệ thống.

## 📌 Chức năng sẽ phát triển
- Chat thời gian thực
- Thông báo thời gian thực
- Đăng nhập bằng Google
- Quên mật khẩu
- Responsive trên thiết bị di động
- Triển khai lên Cloud

## 📄 Giấy phép
Dự án được phát hành theo giấy phép **MIT License**.

## 👤 Tác giả
**Lyly*

GitHub:
https://github.com/lyphuong1722-gif
---

## ⭐ Nếu dự án hữu ích

Hãy để lại một ⭐ trên GitHub để ủng hộ dự án.
