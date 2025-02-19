<<<<<<< HEAD
Nhớ tạo nhanh riêng để commit và push lên trược r mới merge nhé
=======
﻿Sau khi clone project:

1. Chạy file build.bat
* build.bat sẽ build lại project để source code của mọi người đồng nhất.

2. Mở project như bình thường, chọn file .sln để code 
>>>>>>>
Trong trường hợp lỗi database:
B1: Kiểm tra context trong appsetting.json, uid, pwd: "server=YOUR_SQLSERVER;database=MyStore;uid=sa;pwd=123456;TrustServerCer
tificate=true". Nếu vẫn lỗi sang b2
B2: Drop Db cũ và chạy Db mới nhất. Nếu vẫn lỗi thì sang b3.
B3: re-scaffold db:
dotnet ef dbcontext scaffold Name=ConnectionStrings:DbConnection Microsoft.EntityFrameworkCore.SqlServer --output-dir Models --force
B4: Liên hệ Phong :)