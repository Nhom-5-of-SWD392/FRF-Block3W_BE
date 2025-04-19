using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Enum;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Service.Utilities;

namespace Service.Core;

public interface IPostService
{
	Task<PagingModel<PostViewModel>> GetAllPostByUser(PostQueryModel model, string userId);
	Task<Guid> CreateFullPost(string userId, PostCreateModel model);
	Task<Post> GetById (Guid id);
	Task<Guid> SoftDelete(string userId,Guid id);
	Task<Guid> HardDelete(string userId,Guid id);
    Task<PagingModel<PostViewModel>> GetAllApprovedPostsAsync(PostQueryModel query);
    Task<string> AddMediaAsync(Guid postId, List<IFormFile> file);
    Task<PostDetailResponse> GetPostDetailAsync(Guid postId);
    Task<string> AddInstructionToPostAsync(Guid postId, InstructionRequestModel instruction);
    Task<string> AddIngredientToPostAsync(Guid postId, List<IngredientDetailModel> ingredients);
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
        using (var transaction = await _dataContext.Database.BeginTransactionAsync())
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new AppException(ErrorMessage.Unauthorize);
                }

                var postData = _mapper.Map<PostCreateModel, Post>(model);

                postData.CreatedBy = new Guid(userId);

                postData.PostById = new Guid(userId);

                await _dataContext.Post.AddAsync(postData);

                if (model.Topics != null && model.Topics.Count > 0)
                {
                    var existingTopicIds = await _dataContext.Topic
                        .Where(t => model.Topics.Select(mt => mt.Id).Contains(t.Id))
                        .Select(t => t.Id)
                        .ToListAsync();

                    foreach (var topicId in existingTopicIds)
                    {
                        var postTopic = new PostTopic
                        {
                            PostId = postData.Id,
                            TopicId = topicId,
                            CreatedBy = new Guid(userId)
                        };
                        await _dataContext.PostTopic.AddAsync(postTopic);
                    }

                    if (existingTopicIds.Count != model.Topics.Count)
                    {
                        throw new AppException(ErrorMessage.TopicNotExist);
                    }
                }

                if (model.Media != null && model.Media.Count > 0)
                {
                    foreach (var file in model.Media!)
                    {
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

                        string path = mediaType == MediaType.Image ? $"{postData.Id}/images" : $"{postData.Id}/videos";

                        string url = mediaType == MediaType.Image
                            ? await _cloudinaryService.UploadImageAsync(file, path)
                            : await _cloudinaryService.UploadVideoAsync(file, path);

                        var media = new Media
                        {
                            Url = url,
                            Type = mediaType,
                            PostId = postData.Id,
                            CreatedBy = postData.CreatedBy,
                        };

                        await _dataContext.Media.AddAsync(media);
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

    public async Task<PagingModel<PostViewModel>> GetAllApprovedPostsAsync(PostQueryModel query)
    {
        try
        {
            var queryable = _dataContext.Post
                .Where(p => !p.IsDeleted && p.Status == PostStatus.Approved)
                .Include(p => p.PostTopic)
                .AsQueryable();

            queryable = queryable.SearchByKeyword(p => p.Title, query.Search);

            var data = await queryable.ToPagedListAsync(query.PageIndex, query.PageSize);

            var postView = data.Select(post =>
            {
                var postViewModel = _mapper.Map<Post, PostViewModel>(post);

                postViewModel.Topics = post.PostTopic?.ToList() ?? new List<PostTopic>();

                return postViewModel;
            }).ToList();

            return new PagingModel<PostViewModel>
            {
                PageIndex = data.CurrentPage,
                PageSize = data.PageSize,
                TotalCount = data.TotalCount,
                TotalPages = data.TotalPages,
                pagingData = postView
            };
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception("An error occurred while fetching approved posts.");
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

                if (topic.PostTopic == null)
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

    public async Task<Post> GetById(Guid id)	
	{
		try
		{
			var post = await _dataContext.Post
				.FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id);

			if (post == null)
			{
				throw new AppException(ErrorMessage.PostNotFound);
			}

			return post;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new AppException(e.Message);
		}
	}

	public async Task<Guid> HardDelete(string userId, Guid id)
	{
		using (var transaction = await _dataContext.Database.BeginTransactionAsync())
		{
			try
			{
				if (String.IsNullOrEmpty(userId))
				{
					throw new AppException(ErrorMessage.Unauthorize);
				}

				var data = await GetById(id);

				if (data == null)
				{
					throw new AppException(ErrorMessage.PostNotFound);
				}

				if (data.CreatedBy != new Guid(userId))
				{
					throw new AppException(ErrorMessage.PostNotMatchWithUser);
				}

				_dataContext.Post.Remove(data);

				await _dataContext.SaveChangesAsync();

				await transaction.CommitAsync();
				return id;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				await transaction.RollbackAsync();
				throw new Exception(e.Message);
			}
		}
	}

	public async Task<Guid> SoftDelete(string userId,Guid id)
	{
		try
		{
			if (String.IsNullOrEmpty(userId))
			{
				throw new AppException(ErrorMessage.Unauthorize);
			}
			var data = await GetById(id);
			if (data == null)
			{
				throw new AppException(ErrorMessage.TopicNotFound);
			}
			if (data.CreatedBy != new Guid(userId))
			{
				throw new AppException(ErrorMessage.PostNotMatchWithUser);
			}

			data.IsDeleted = true;

			_dataContext.Post.Update(data);

			await _dataContext.SaveChangesAsync();

			return data.Id;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new Exception(e.Message);
		}
	}

    public async Task<PostDetailResponse> GetPostDetailAsync(Guid postId)
    {
        try
        {
            var post = await _dataContext.Post
            .Include(p => p.PostBy)!
            .Include(p => p.PostIngredients)!
                .ThenInclude(pi => pi.Ingredient)!
            .Include(p => p.PostTopic)!
                .ThenInclude(pt => pt.Topic)
            .Include(p => p.Medias)
            .Include(p => p.Instructions)
            .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);

            if (post == null)
                throw new AppException(ErrorMessage.PostNotFound);

            var response = new PostDetailResponse
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Status = post.Status.ToString(),
                PostByName = post.PostBy?.FirstName + " " + post.PostBy?.LastName ?? "Unknown",

                Ingredients = post.PostIngredients?.Select(pi => new IngredientDetail
                {
                    Id = pi.Id,
                    Name = pi.Ingredient?.Name ?? "Unknown",
                    Quantity = pi.Quantity,
                    Unit = pi.Unit
                }).ToList() ?? new(),

                Topics = post.PostTopic?.Select(pt => pt.Topic?.Name ?? "Unnamed Topic").ToList() ?? new(),

                MediaUrls = post.Medias?.Select(m => new MediaResponse
                {
                    Id = m.Id,
                    Url = m.Url,
                    Type = m.Type
                }).ToList() ?? new(),

                Instructions = post.Instructions?.OrderBy(i => i.CreatedAt).Select(i => new InstructionResponse
                {
                    Id = i.Id,
                    Content = i.Content,
                    ImageUrl = i.ImageUrl
                }).ToList() ?? new()
            };

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task<string> AddMediaAsync(Guid postId, List<IFormFile> files)
    {
        try
        {
            var post = await _dataContext.Post
            .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);
            if (post == null)
                throw new AppException(ErrorMessage.PostNotFound);

            foreach (var file in files)
            {
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
                    PostId = postId,
                    CreatedBy = post.CreatedBy,
                };

                await _dataContext.Media.AddAsync(media);
            }

            await _dataContext.SaveChangesAsync();

            return "Uploaded!";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task<string> AddInstructionToPostAsync(Guid postId, InstructionRequestModel instruction)
    {
        try
        {
            var post = await GetById(postId);

            string? imageUrl = null;
            string path = $"{postId}/instructions";

            if (instruction.Image != null)
            {
                if (!instruction.Image.ContentType.StartsWith("image/"))
                {
                    throw new AppException(ErrorMessage.OnlyAllowImage);
                }

                imageUrl = await _cloudinaryService.UploadImageAsync(instruction.Image, path);
            }

            var newInstruction = new Instruction
            {
                PostId = postId,
                Content = instruction.Content,
                ImageUrl = imageUrl,
                CreatedBy = post.CreatedBy
            };

            await _dataContext.Instruction.AddAsync(newInstruction);

            await _dataContext.SaveChangesAsync();

            return "Instruction Added!";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task<string> AddIngredientToPostAsync(Guid postId, List<IngredientDetailModel> ingredients)
    {
        using (var transaction = await _dataContext.Database.BeginTransactionAsync())
        {
            try
            {
                var post = await GetById(postId);

                foreach (var item in ingredients)
                {
                    var newIngredient = new Ingredient
                    {
                        Name = item.Name,
                        CreatedBy = post.CreatedBy
                    };

                    await _dataContext.Ingredient.AddAsync(newIngredient);

                    var postIngredient = new PostIngredient
                    {
                        PostId = postId,
                        IngredientId = newIngredient.Id,
                        Quantity = item.Quantity,
                        Unit = item.Unit,
                        CreatedBy = post.CreatedBy
                    };
                    await _dataContext.PostIngredient.AddAsync(postIngredient);
                }

                await _dataContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return "Ingredients added successfully!";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw new Exception(e.Message);
            }
        }
    }
}