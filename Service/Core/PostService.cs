using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Models;
using Service.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Core;

public interface IPostService
{
	Task<PagingModel<PostViewModel>> GetAllPostByUser(PostQueryModel model, string userId);
	Task<Guid> CreateFullPost(string userId, PostCreateModel model);
	
}
public class PostService : IPostService
{
	private readonly DataContext _dataContext;
	private readonly IMapper _mapper;
	private readonly ISortHelpers<Topic> _sortHelpers;

	public PostService(DataContext dataContext, IMapper mapper, ISortHelpers<Topic> sortHelpers)
	{
		_dataContext = dataContext;
		_mapper = mapper;
		_sortHelpers = sortHelpers;
	}

	public async Task<Guid> CreateFullPost(string userId, PostCreateModel model)
	{
		using(var transaction = await _dataContext.Database.BeginTransactionAsync())
		{
			try
			{
				if (string.IsNullOrEmpty(userId))
				{
					throw new AppException(ErrorMessage.Unauthorize);
				}

				var postData = _mapper.Map<PostCreateModel, Post>(model);

				postData.CreatedBy = new Guid(userId);

				postData.PostById= new Guid(userId);

				await _dataContext.Post.AddAsync(postData);

				if (model.Ingredients != null && model.Ingredients.Count > 0)
				{
					foreach (var item in model.Ingredients)
					{
						var ingredient = new Ingredient
						{
							Name = item.Name,
							CreatedBy = new Guid(userId)
						};
						await _dataContext.Ingredient.AddAsync(ingredient);

						var postIngredient = new PostIngredient
						{
							PostId = postData.Id,
							IngredientId = ingredient.Id,
							Quantity = item.Quantity,
							Unit = item.Unit,
						};
                        postIngredient.CreatedBy = new Guid(userId);

						await _dataContext.PostIngredient.AddAsync(postIngredient);
					}
				}

				await _dataContext.SaveChangesAsync();

				await transaction.CommitAsync();

				return postData.Id;

			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				await transaction.RollbackAsync();
				throw new Exception(e.Message);
			}
		}
	}

	public async Task<PagingModel<PostViewModel>> GetAllPostByUser(PostQueryModel query, string userId)
	{
		try
		{
			if (String.IsNullOrEmpty(userId))
			{
				throw new AppException(ErrorMessage.Unauthorize);
			}

			var queryable = _dataContext.Post
				.Where(p => !p.IsDeleted && p.CreatedBy == new Guid(userId))
				.AsQueryable();

			queryable = queryable.SearchByKeyword(p => p.Title, query.Search);

			var data = await queryable.ToPagedListAsync(query.PageIndex, query.PageSize);

			var postView = data.Select(topic =>
			{
				var postViewModel = _mapper.Map<Post, PostViewModel>(topic);

				postViewModel.Topics = topic.PostTopic.ToList();

				return postViewModel;
			}).ToList();

			var pagingData = new PagingModel<PostViewModel>()
			{
				PageIndex = data.CurrentPage,
				PageSize = data.PageSize,
				TotalCount = data.TotalCount,
				TotalPages = data.TotalPages,
				pagingData = postView
			};

			return pagingData;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new Exception(e.Message);
		}
	}

	

	public Task<PostViewModel> GetById(Guid id)
	{
		throw new NotImplementedException();
	}
}
