using AutoMapper;
using HShop.Models;
using HShop.ViewModels;

namespace HShop.Utils
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile() 
        {
            CreateMap<RegisterVM, KhachHang>();
                //// Nếu thuộc tính trong object src and dest không cùng tên thì dùng method ForMember để config
                //.ForMember(dest => dest.HoTen, opt => opt.MapFrom(src => src.HoTen))
                //.ReverseMap();
        }
    }
}
