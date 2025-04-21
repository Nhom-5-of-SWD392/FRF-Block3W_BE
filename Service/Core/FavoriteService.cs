using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Enum;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Service.Core;

public interface IFavoriteService
{
	Task<Guid> CreateFavorite(FavoriteCreateModel model, string userId);
	Task<Favorite> GetById(Guid id);
	Task<PagingModel<FavoriteViewModel>> GetFavoriteListByUser(FavoriteQueryModel model, string userId, string role);

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

	public async Task<Guid> CreateFavorite(FavoriteCreateModel model, string userId)
	{
		using (var transaction = _dataContext.Database.BeginTransaction())
		{
			try
			{
				if (string.IsNullOrEmpty(userId))
				{
					throw new Exception(ErrorMessage.Unauthorize);
				}
					
				//Create favorite entity
				var favorite = _mapper.Map<FavoriteCreateModel, Favorite>(model);

				favorite.CreatedBy = new Guid(userId);
				favorite.UserId = new Guid(userId);
				favorite.PostId = new Guid(model.PostId.ToString());


				await _dataContext.Favorite.AddAsync(favorite);

				//Add the favorite to the user
				var user = await _userService.GetById(new Guid(userId));

				if (user == null)
				{
					throw new Exception(ErrorMessage.UserNotFound);
				}

				if(user.Favorites == null)
				{
					user.Favorites = new List<Favorite>();
				}

				user.Favorites.Add(favorite);

				_dataContext.User.Update(user);

				//Add the post to the favorite
				var post = await _postService.GetById(model.PostId);
				if (post == null)
				{
					throw new Exception(ErrorMessage.PostNotFound);
				}
				//If list of post is null, create new
				if (post.Favorites == null)
				{
					post.Favorites = new List<Favorite>();
				}
				post.Favorites.Add(favorite);

				_dataContext.Post.Update(post);

				await _dataContext.SaveChangesAsync();
				await transaction.CommitAsync();

				return favorite.Id;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				await transaction.RollbackAsync();
				throw new Exception(e.Message);
			}
		}
	}

	public async Task<Favorite> GetById(Guid id)
	{
		try
		{
			var favorite = await _dataContext.Favorite
				.FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id);

			if (favorite == null)
			{
				throw new AppException(ErrorMessage.TopicNotFound);
			}

			return favorite;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new AppException(e.Message);
		}
	}

	

	public async Task<PagingModel<FavoriteViewModel>> GetFavoriteListByUser(FavoriteQueryModel query, string userId, string role)
	{
		try
		{
			if (string.IsNullOrEmpty(userId) && string.IsNullOrEmpty(role))
			{
				throw new AppException(ErrorMessage.Unauthorize);
			}

			var queryable = _dataContext.Favorite
				.Where(p => !p.IsDeleted && p.CreatedBy == new Guid(userId))
				.Include(p => p.Post)
				.AsQueryable();
			queryable = queryable.SearchByKeyword(p => p.Post.Title, query.Search);

			var data = await queryable.ToPagedListAsync(query.PageIndex, query.PageSize);


			//Convert Favorite to FavoriteViewModel using var data variable
			var favoriteView = _mapper.Map<List<Favorite>, List<FavoriteViewModel>>(data.ToList());

			var pagingData = new PagingModel<FavoriteViewModel>()
			{
				PageIndex = data.CurrentPage,
				PageSize = data.PageSize,
				TotalCount = data.TotalCount,
				TotalPages = data.TotalPages,
				pagingData = favoriteView
			};
			return pagingData;

		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new Exception(e.Message);
		}
	}
}
