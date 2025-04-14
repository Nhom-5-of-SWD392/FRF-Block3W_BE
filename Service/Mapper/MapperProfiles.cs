using AutoMapper;
using Data.Entities;
using Data.Models;

namespace Service.Mapper
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            //Role
            CreateMap<RoleCreateModel, Role>();
            CreateMap<Role, RoleViewModel>();
            CreateMap<RoleUpdateModel, Role>()
                .ForAllMembers(opt => opt.Condition((src, des, obj) => obj != null));

            //Permission
            CreateMap<PermissionCreateModel, Permission>();
            CreateMap<Permission, PermissionViewModel>();
            CreateMap<PermissionUpdateModel, Permission>()
                .ForAllMembers(opt => opt.Condition((src, des, obj) => obj != null));

            //User
            CreateMap<UserCreateModel, User>();
            CreateMap<User, UserViewModel>();
            CreateMap<UserUpdateModel, User>()
                .ForAllMembers(opt => opt.Condition((src, des, obj) => obj != null));
        }
    }
}
