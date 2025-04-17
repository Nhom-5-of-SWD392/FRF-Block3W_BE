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
	Task<PostViewModel> GetPostById(string id);
	Task<PostDetailModel> GetPostDetailById(string id);
	Task<PagingModel<PostViewModel>> GetAllPostByUserId(PostQueryModel model, string userId);
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

				// Create Post object first
				var postData = _mapper.Map<PostCreateModel, Post>(model);
				postData.CreatedBy = new Guid(userId);

				await _dataContext.Post.AddAsync(postData);



				if (model.Ingredients != null && model.Ingredients.Count > 0)
				{
					foreach (var item in model.Ingredients)
					{
						//Create Ingredient first before assign to PostIngredient
						var ingredient = new Ingredient
						{
							Name = item.Name,

							CreatedBy = new Guid(userId)
						};
						await _dataContext.Ingredient.AddAsync(ingredient);

					
						//Map with Post Ingredient, assume that Post Ingredient contains IngredientId
						//Asign this to Post
						var PostIngredient = new PostIngredient
						{
							PostId = postData.Id,
							IngredientId = ingredient.Id,
							Quantity = item.Quantity,
							Unit = item.Unit,
						};
						PostIngredient.CreatedBy = new Guid(userId);
						await _dataContext.PostIngredient.AddAsync(PostIngredient);
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

	public async Task<PagingModel<PostViewModel>> GetAllPostByUserId(PostQueryModel query,string userId)
	{
		try
		{
			if (String.IsNullOrEmpty(userId))
			{
				throw new AppException(ErrorMessage.Unauthorize);
			}

			var queryable = _dataContext.Post
				.Where(x => !x.IsDeleted && x.CreatedBy == new Guid(userId)).AsQueryable();

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

	

	public Task<PostViewModel> GetPostById(string id)
	{
		throw new NotImplementedException();
	}

	

	public Task<PostDetailModel> GetPostDetailById(string id)
	{
		throw new NotImplementedException();
	}

	
}
