using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Utilities;
using System.Linq.Dynamic.Core;

namespace Service.Core;

public interface IFavoriteService
{
	Task<PagingModel<FavoriteViewModel>> GetFavoriteListByUser(FavoriteQueryModel model, string userId);
}
public class FavoriteService : IFavoriteService
{
	private readonly DataContext _dataContext;
	private readonly IMapper _mapper;
	private readonly IPostService _postService;
	private readonly IUserService _userService;

	public FavoriteService(DataContext dataContext, IMapper mapper, IPostService postService, IUserService userService)
	{
		_dataContext = dataContext;
		_mapper = mapper;
		_postService = postService;
		_userService = userService;
	}

	public async Task<Favorite> GetById(Guid id)
	{
		try
		{
			var favorite = await _dataContext.Favorite
				.FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id);

			if (favorite == null)
			{
				throw new AppException(ErrorMessage.FavoriteNotFound);
			}

			return favorite;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new AppException(e.Message);
		}
	}

    public async Task<PagingModel<FavoriteViewModel>> GetFavoriteListByUser(FavoriteQueryModel query, string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
                throw new AppException(ErrorMessage.Unauthorize);

            var userGuid = new Guid(userId);

            var queryable = _dataContext.Favorite
                .Where(f => !f.IsDeleted && f.CreatedBy == userGuid)
                .Include(f => f.Post)!
                    .ThenInclude(p => p.PostBy!)!
                .Include(f => f.Post)!
                    .ThenInclude(p => p.PostIngredients)!
                        .ThenInclude(pt => pt.Ingredient)
                .Include(f => f.Post)!
                    .ThenInclude(p => p.PostTopic!)!
                        .ThenInclude(pt => pt.Topic)
                .Include(f => f.Post)!
                    .ThenInclude(p => p.Medias)
                .Where(f => f.Post != null && !f.Post.IsDeleted);

            if (!string.IsNullOrEmpty(query.Search))
            {
                queryable = queryable.Where(f => f.Post!.Title.Contains(query.Search));
            }

            var data = await queryable.ToPagedListAsync(query.PageIndex, query.PageSize);


            var favoriteViewModels = data.Select(favorite => new FavoriteViewModel
            {
                Id = favorite.Id,
                UpdatedBy = favorite.CreatedBy,
                CreatedBy = favorite.UpdatedBy,
                Posts = new List<PostViewFavoriteModel>
                {
                    new PostViewFavoriteModel
                    {
                        Id = favorite.Post!.Id,
                        Title = favorite.Post.Title,
                        Content = favorite.Post.Content,
                        Status = favorite.Post.Status,
                        PostById = favorite.Post.PostById,
                        CreatedBy = favorite.Post.CreatedBy,
                        UpdatedBy = favorite.Post.UpdatedBy,
                        PostBy = favorite.Post.PostBy != null
                            ? $"{favorite.Post.PostBy.FirstName} {favorite.Post.PostBy.LastName}"
                            : "Thành viên ẩn danh",
                        AuthorImage = favorite.Post.PostBy!.AvatarUrl,
                        Topics = favorite.Post.PostTopic!.Select(pt => new TopicViewModel
                        {
                            Id = pt.Topic!.Id,
                            Name = pt.Topic.Name,
                            CreatedBy = pt.Topic!.CreatedBy,
                            UpdatedBy = pt.Topic!.UpdatedBy,
                        }).ToList(),
                        Medias = favorite.Post.Medias?.Select(m => new MediaViewModel
                        {
                            Url = m.Url,
                            Type = m.Type
                        }).ToList() ?? new List<MediaViewModel>(),
                        Ingredients = favorite.Post.PostIngredients?
                            .Select(pi => pi.Ingredient!.Name)
                            .Where(name => !string.IsNullOrEmpty(name))
                            .ToList() ?? new List<string>()
                    }
                }
            }).ToList();

            return new PagingModel<FavoriteViewModel>
            {
                PageIndex = data.CurrentPage,
                PageSize = data.PageSize,
                TotalCount = data.TotalCount,
                TotalPages = data.TotalPages,
                pagingData = favoriteViewModels
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }
}
