using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Enum;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
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
    Task<string> AddMediaAsync(Guid postId, IFormFile file);
}
public class PostService : IPostService
{
	private readonly DataContext _dataContext;
	private readonly IMapper _mapper;
	private readonly ISortHelpers<Topic> _sortHelpers;
    private readonly ICloudinaryService _cloudinaryService;

    public PostService(DataContext dataContext, IMapper mapper, ISortHelpers<Topic> sortHelpers, ICloudinaryService cloudinaryService)
	{
		_dataContext = dataContext;
		_mapper = mapper;
		_sortHelpers = sortHelpers;
		_cloudinaryService = cloudinaryService;
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

				if (topic.PostTopic==null)
				{
					postViewModel.Topics = new List<PostTopic>();
				}
				else
				{
					postViewModel.Topics = topic.PostTopic.ToList();
				}
				


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

    public async Task<string> AddMediaAsync(Guid postId, IFormFile file)
    {
        var post = await _dataContext.Post
            .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);
        if (post == null)
            throw new AppException(ErrorMessage.PostNotFound);

        var contentType = file.ContentType.ToLower();
        MediaType mediaType;

        if (contentType.StartsWith("image/"))
        {
            mediaType = MediaType.Image;
        }
        else if (contentType.StartsWith("video/"))
        {
            mediaType = MediaType.Video;
        }
        else
        {
            throw new AppException(ErrorMessage.UnsupportedFile);
        }

        string path = mediaType == MediaType.Image ? $"{postId}/images" : $"{postId}/videos";
        string url = mediaType == MediaType.Image
            ? await _cloudinaryService.UploadImageAsync(file, path)
            : await _cloudinaryService.UploadVideoAsync(file, path);

        var media = new Media
        {
            Url = url,
            Type = mediaType,
            PostId = postId
        };

        await _dataContext.Media.AddAsync(media);
        await _dataContext.SaveChangesAsync();

        return url;
    }
}
