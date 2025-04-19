using AutoMapper;
using Data.Entities;
using Data.Models;

namespace Service.Mapper;

public class MapperProfiles : Profile
{
    public MapperProfiles()
    {
        //User
        CreateMap<UserCreateModel, User>();
        CreateMap<User, UserViewModel>();
        CreateMap<UserUpdateModel, User>()
            .ForAllMembers(opt => opt.Condition((src, des, obj) => obj != null));

        //Quiz
        CreateMap<QuizCreateModel, Quiz>();
        CreateMap<Quiz, QuizViewModel>();

        //QuizQuestion
        CreateMap<QuizQuestionCreateModel, QuizQuestion>();

        //QuizAnswer
        CreateMap<QuizAnswerModel, QuizAnswer>();

        //Topic
        CreateMap<TopicCreateModel, Topic>();
        CreateMap<Topic, TopicViewModel>();
		CreateMap<TopicUpdateModel, Topic>()
			.ForAllMembers(opt => opt.Condition((src, des, obj) => obj != null));

		//Post
		CreateMap<PostCreateModel, Post>();
		CreateMap<Post, PostViewModel>();

		//Reaction
		CreateMap<ReactionCreateModel, Reaction>();
		CreateMap<Reaction, ReactionViewModel>();

	}
}
