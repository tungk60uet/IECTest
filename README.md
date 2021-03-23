# IECTest
2/ Tạo Cheat Menu tap góc trên phải 3 lần -> option

7/ Một số hướng cải thiện game:
- Phần core game xử lý nhiều thứ ở trong Update. Nên đưa ra các event,trigger, xử lý theo sự kiện (Event-Driven)
- Chưa có hệ thống quản lý UI
- Tổ chức project đang để chung phần asset của project và các thư viện thứ 3 cùng trong asset. Các asset của project nên để vào trong một thư mục riêng (_GameAsset) để dễ quản lý.
- Nhiều popup để thẳng trên Scene -> chưa module hóa người trong team sửa hai popup khác nhau nhưng vẫn bị conflict scene. Nên kéo ra các prefab, hoặc load từ resource/ addresable hoặc scene riêng ta có thể control việc Load/Unload để tiết kiệm bộ nhớ

