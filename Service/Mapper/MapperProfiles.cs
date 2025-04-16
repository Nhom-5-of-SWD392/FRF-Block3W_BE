using AutoMapper;
using Data.Entities;
using Data.Models;

namespace Service.Mapper
{
    public class MapperProfiles : Profile
    {
        public MapperProfiles()
        {
            //User
            CreateMap<UserCreateModel, User>();
            CreateMap<User, UserViewModel>();
            CreateMap<UserUpdateModel, User>()
                .ForAllMembers(opt => opt.Condition((src, des, obj) => obj != null));
        }
    }
}
