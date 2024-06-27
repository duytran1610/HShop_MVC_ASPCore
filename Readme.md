## Create Layout
- Import layout from htmlcodex.com into directon wwwroot
- Create RazorLayout _LayoutCustomer
- Create RazorView _Footer
- Sử dụng @await Html.PartialAsync("_Footer"); để gọi Footer trong LayoutCustomer

## Tạo Menu hàng hóa
- Tạo thư mục ViewComponets với ViewModels
- Tạo class MenuLoaiVM trong ViewModels, MenuLoaiViewComponet trong ViewComponets
- Tạo Views/Shared/Components/MenuLoai để hiện view về MenuLoai
- Sử dụng @await Component.InvokeAsync("MenuLoai") để gọi Default.cshtml 

## Trang Danh sách hàng hóa
- Tạo layout _DanhSachHangHoa
- Sử dụng RenderSection trong layout _DanhSachHangHoa 
- Chỉnh sửa code trong view của action index của controller HangHoa
- Tạo action Search của controller HangHoa
- Thêm chức năng tìm kiếm và modal trong _LayoutCustomer

## Chi tiết hàng hóa
- Tạo view ProductItem of controller HangHoa để xem danh sách sản phẩm
- Import ProductItem vào view action Index và Search of controller HangHoa
- Tạo class DetailHangHoaVM
- Tạo view Detail and action Detail of controller HangHoa
- Tạo Layout _SearchPanel và import vào _DanhSachHoangHoa và view Detail of controller HangHoa

## Đưa hàng vào giỏ
- Sử dụng Session and state management in ASP.NET Core 
- Xây dựng method extenxion (Tạo static class SessionExtensions) cho session
- Tạo ViewModel CartItemVM and Controller CartController
- Xây dựng action and view Index cho CartController
- Xây dựng action Add and Remove để redirect to action Index trong CartController