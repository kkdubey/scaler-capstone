using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Scaler.Core.Models.Account;
using Scaler.Core.Models.Shop;
using Scaler.Core.Services.Account;
using Scaler.Server.ViewModels.Account;
using Scaler.Server.ViewModels.Shop;

namespace Scaler.Server.Configuration
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoMappingProfile : Profile
    {
        /// <summary>
        /// 
        /// </summary>
        public AutoMappingProfile()
        {
            CreateMap<Customer, CustomerVm>()
                .ReverseMap();

            CreateMap<Product, ProductVm>()
                .ReverseMap();

            CreateMap<Order, OrderVm>()
                .ReverseMap();

            CreateMap<ApplicationUser, UserVm>()
                   .ForMember(d => d.Roles, map => map.Ignore());
            CreateMap<UserVm, ApplicationUser>()
                .ForMember(d => d.Roles, map => map.Ignore())
                .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            CreateMap<ApplicationUser, UserEditVm>()
                .ForMember(d => d.Roles, map => map.Ignore());
            CreateMap<UserEditVm, ApplicationUser>()
                .ForMember(d => d.Roles, map => map.Ignore())
                .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            CreateMap<ApplicationUser, UserPatchVm>()
                .ReverseMap();

            CreateMap<ApplicationRole, RoleVm>()
                .ForMember(d => d.Permissions, map => map.MapFrom(s => s.Claims))
                .ForMember(d => d.UsersCount, map => map.MapFrom(s => s.Users.Count))
                .ReverseMap();
            CreateMap<RoleVm, ApplicationRole>()
                .ForMember(d => d.Id, map => map.Condition(src => src.Id != null));

            CreateMap<IdentityRoleClaim<string>, ClaimVm>()
                .ForMember(d => d.Type, map => map.MapFrom(s => s.ClaimType))
                .ForMember(d => d.Value, map => map.MapFrom(s => s.ClaimValue))
                .ReverseMap();

            CreateMap<ApplicationPermission, PermissionVm>()
                .ReverseMap();


            CreateMap<IdentityRoleClaim<string>, PermissionVm>()
                .ConvertUsing(s => ((PermissionVm)ApplicationPermissions.GetPermissionByValue(s.ClaimValue))!);

        }
    }
}
