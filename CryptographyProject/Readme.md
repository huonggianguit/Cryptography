* Các phần mềm hổ trợ.
- IDE: InteliJ (java), Quản lý Build Java: Gradle
- Phiên bản sử dung JDK 24.0.1, Gradle(Kotlin DSL) 8.14
- Client WPF (.Net 8.0, Target OS version 7.0)
- Sử dụng Laragon 6.0 làm môi trường phát triển tích hợp, bao gồm MySQL.
+ MySQL (8.4.5)
+ phpMyAdmin (5.2.2)
+ PHP version: 8.1.10
+ Web Server:Apache: 2.4.54 (Win64), OpenSSL: 1.1.1q
Lưu ý khi sử dụng môi trường phát triển tích hợp khác hoặc MySQL ver thấp hơn, có thể xảy ra xung đột API vói MySQL (ver 8). Tạo lại SQL để thích nghi với ver thấp hơn của MySQL.
* Tạo thư mục DataServerDoAn ở Desktop máy Server làm nơi lưu trữ File người dùng Up.
* Tạo thư mục OutputClientDoAn ở Desktop máy Client làm nơi tải file khi down từ Server.
* Đồ án mật mã học lớp thầy Nguyễn Ngọc Tự