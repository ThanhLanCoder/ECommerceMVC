using AutoMapper;
using ECommerceMVC.Models.Entities;
using ECommerceMVC.Models.ViewModels;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.Blazor;

namespace ECommerceMVC.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DangKyVM, KhachHang>();
            
        }
    }
}
