using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Enum;
using Data.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Service.Utilities;

namespace Service.Core;

public interface IPostService
{
	Task<PagingModel<PostViewModel>> GetAllPostByUser(PostQueryModel model, string userId, string role);
    Task<Guid> CreateFullPost(string userId, PostInputModel model);
	Task<Guid> SoftDelete(string userId,Guid id);
	Task<Guid> HardDelete(string userId,Guid id);
    Task<PagingModel<PostViewModel>> GetAllApprovedPostsAsync(PostApproveQueryModel query);
	Task<string> AddMediaAsync(Guid postId, List<IFormFile> file);
    Task<PostDetailResponse> GetPostDetailAsync(Guid postId);
    Task<string> AddInstructionToPostAsync(Guid postId, InstructionRequestModel instruction);
    Task UpdateInstructionFromPostAsync(string userId, Guid postId, InstructionUpdateModel instruction);
    Task<string> AddIngredientToPostAsync(Guid postId, List<IngredientDetailModel> ingredients);
    Task<Guid> AddCommentAsync(string userId, Guid postId, CommentCreateModel model);
    Task<IEnumerable<CommentResponseModel>> GetCommentsByPostIdAsync(Guid postId);
    Task<Guid> ApproveOrRejectPostAsync(string userId, Guid postId, ConfirmPost model);
    Task<string> AddPostToFavoriteList(Guid postId, string userId);
    Task<Guid> RemovePostFromFavoriteList(Guid postId, string userId);
    Task<Guid> UpdatePostAsync(string userId, Guid id, PostUpdateModel model);
}
public class PostService : IPostService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;   
    private readonly IUserService _userService;
	private readonly ICloudinaryService _cloudinaryService;
    private readonly IFilterHelper<Post>_filterPostHelper;

    public PostService(DataContext dataContext, IMapper mapper, ICloudinaryService cloudinaryService, IFilterHelper<Post> filterPostHelper, IUserService userService)
    {
        _dataContext = dataContext;
        _mapper = mapper;
        _filterPostHelper = filterPostHelper;
		_cloudinaryService = cloudinaryService;
		_userService = userService;

	}

    public async Task<Guid> CreateFullPost(string userId, PostInputModel model)
    {
        using (var transaction = await _dataContext.Database.BeginTransactionAsync())
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new AppException(ErrorMessage.Unauthorize);
                }

                var userGuid = new Guid(userId);

                var newPost = new PostCreateModel
                {
                    Title = model.Title,
                    Content = model.Content,
                    PostById = userGuid,
                };

                var postData = _mapper.Map<PostCreateModel, Post>(newPost);

                postData.CreatedBy = userGuid;

                await _dataContext.Post.AddAsync(postData);

                if (model.Topics != null && model.Topics.Count > 0)
                {
                    var existingTopicIds = await _dataContext.Topic
                        .Where(t => model.Topics.Select(mt => mt).Contains(t.Id))
                        .Select(t => t.Id)
                        .ToListAsync();

                    foreach (var topicId in existingTopicIds)
                    {
                        var postTopic = new PostTopic
                        {
                            PostId = postData.Id,
                            TopicId = topicId,
                            CreatedBy = userGuid
                        };
                        await _dataContext.PostTopic.AddAsync(postTopic);
                    }

                    if (existingTopicIds.Count != model.Topics.Count)
                    {
                        throw new AppException(ErrorMessage.TopicNotExist);
                    }
                }

                if (model.Medias != null && model.Medias.Count > 0)
                {
                    foreach (var file in model.Medias!)
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

    public async Task<PagingModel<PostViewModel>> GetAllApprovedPostsAsync(PostApproveQueryModel query)
    {
        try
        {
            var queryable = _dataContext.Post
                .Where(p => !p.IsDeleted && p.Status == PostStatus.Approved)
                .Include(p => p.PostTopic)!
                    .ThenInclude(pt => pt.Topic)
                .Include(p => p.Medias)
                .Include(p => p.PostBy)
                .AsQueryable();

            //queryable = queryable.SearchByManyKeyword(query.Search,
            //[
            //    p => p.Title,
            //    p => p.PostBy!.LastName,
            //]);

            queryable = queryable.SearchIncludingTopics(query.Search);

            var filters = new Dictionary<string, string>();

            if (query.TopicId.HasValue)
                filters.Add("TopicId", query.TopicId.ToString());

            queryable = _filterPostHelper.ApplyFilterPost(queryable, filters);

            var data = await queryable.ToPagedListAsync(query.PageIndex, query.PageSize);

            var postView = data.Select(post =>
            {
                var postViewModel = new PostViewModel
                {
                    Id = post.Id,
                    Title = post.Title,
                    Content = post.Content,
                    Status = post.Status,
                    PostBy = post.PostBy!.FirstName + " " + post.PostBy!.LastName,
                    CreatedBy = post.CreatedBy,
                    UpdatedBy = post.UpdatedBy,
                    ConfirmBy = post.ComfirmById,
                    Topics = post.PostTopic?.Select(pt => new TopicViewModel
                    {
                        Id = pt.TopicId,
                        Name = pt.Topic?.Name
                    }).ToList() ?? new(),

                    Medias = post.Medias?
                    .Where(m => m.Type == MediaType.Image)
                    .Select(m => new MediaViewModel
                    {
                        Url = m.Url,
                        Type = m.Type
                    }).ToList() ?? new()
                };

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

    public async Task<PagingModel<PostViewModel>> GetAllPostByUser(PostQueryModel query, string userId, string role)
    {
        try
        {
            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(role))
                throw new AppException(ErrorMessage.Unauthorize);

            IQueryable<Post> queryable;

            var userGuid = new Guid(userId);

            var isAdmin = role == UserRole.Administrator.ToString();

            var user = await _dataContext.User
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.Id == userGuid);

            if (isAdmin || user!.IsModerator == true)
            {
                queryable = _dataContext.Post
                    .Where(p => !p.IsDeleted)
                    .Include(p => p.PostTopic)!
                        .ThenInclude(pt => pt.Topic)
                    .Include(p => p.Medias)
                    .Include(p => p.PostIngredients)!
						.ThenInclude(pi => pi.Ingredient)
					.Include(p => p.PostBy);
            }
            else
            {
                queryable = _dataContext.Post
                    .Where(p => !p.IsDeleted && p.CreatedBy == userGuid)
                    .Include(p => p.PostTopic)!
                        .ThenInclude(pt => pt.Topic)
						.Include(p => p.PostIngredients)!
						.ThenInclude(pi => pi.Ingredient)
					.Include(p => p.Medias);
            }

            queryable = queryable.SearchByTitleOrIngredient(query.Search);

			var filters = new Dictionary<string, string>();

            if (query.Status.HasValue)
                filters.Add("Status", query.Status.ToString());

            queryable = _filterPostHelper.ApplyFilterPost(queryable, filters);

            var data = await queryable.ToPagedListAsync(query.PageIndex, query.PageSize);

            var postView = data.Select(post => new PostViewModel
            {
                Id = post.Id,
                Title = post.Title,
                Content = post.Content,
                Status = post.Status,
                PostBy = post.PostBy!.FirstName + " " + post.PostBy.LastName,
                ConfirmBy = post.ComfirmById,
                CreatedBy = post.CreatedBy,
                UpdatedBy = post.UpdatedBy,
                Topics = post.PostTopic?.Select(pt => new TopicViewModel
                {
                    Id = pt.TopicId,
                    Name = pt.Topic?.Name
                }).ToList() ?? new(),

                Medias = post.Medias?
                .Where(m => m.Type == MediaType.Image)
                .Select(m => new MediaViewModel
                {
                    Url = m.Url,
                    Type = m.Type
                }).ToList() ?? new()

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
				if (string.IsNullOrEmpty(userId))
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
			if (string.IsNullOrEmpty(userId))
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
                Reason = post.Reason,
                PostByName = post.PostBy?.FirstName + " " + post.PostBy!.LastName,
                AuthorImage = post.PostBy!.AvatarUrl,
                Ingredients = post.PostIngredients?.Select(pi => new IngredientDetail
                {
                    Id = pi.Id,
                    Name = pi.Ingredient!.Name,
                    Quantity = pi.Quantity,
                    Unit = pi.Unit
                }).ToList() ?? new(),
                Topics = post.PostTopic?.Select(pt => pt.Topic!.Name).ToList() ?? new(),
                MediaUrls = post.Medias?.Select(m => new MediaResponse
                {
                    Id = m.Id,
                    Url = m.Url,
                    Type = m.Type
                }).ToList() ?? new(),

                Instructions = post.Instructions?.OrderBy(i => i.CreatedAt)
                .Select(i => new InstructionResponse
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

    public async Task UpdateInstructionFromPostAsync(string userId, Guid postId, InstructionUpdateModel instruction)
    {
        try
        {
            var userGuid = Guid.Parse(userId);

            var post = await _dataContext.Post
                .Include(p => p.Instructions)
                .FirstOrDefaultAsync(p => p.Id == postId && p.PostById == userGuid);

            if (post == null)
                throw new Exception(ErrorMessage.PostNotMatchWithUser);

            var existingInstruction = post.Instructions!
                .FirstOrDefault(i => i.Id == instruction.Id);

            if (existingInstruction == null)
                throw new Exception(ErrorMessage.InstructionNotFound);

            if (instruction.Image != null)
            {
                var path = $"{post.Title}/instruction-images";
                var imageUrl = await _cloudinaryService.UploadImageAsync(instruction.Image, path);
                existingInstruction.ImageUrl = imageUrl;
            }

            if (!string.IsNullOrWhiteSpace(instruction.Content))
            {
                existingInstruction.Content = instruction.Content;
            }

            existingInstruction.UpdatedAt = DateTime.UtcNow;

            existingInstruction.UpdatedBy = userGuid;

            _dataContext.Instruction.Update(existingInstruction);

            post.Status = PostStatus.EditedPendingApproval;

            await _dataContext.SaveChangesAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
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

    public async Task<Guid> AddCommentAsync(string userId, Guid postId, CommentCreateModel model)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException(ErrorMessage.Unauthorize);
            }

            var user = await _dataContext.User
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.Id == new Guid(userId));
            if (user == null || user.Role != UserRole.Member && user.Role != UserRole.Administrator)
                throw new AppException(ErrorMessage.OnlyMemberCanComment);

            var post = await GetById(postId);
            if (post == null)
                throw new AppException(ErrorMessage.PostNotFound);

            if (model.ParentCommentId != null)
            {
                var parent = await _dataContext.Comment
                    .FirstOrDefaultAsync(c => !c.IsDeleted && c.Id == model.ParentCommentId);
                if (parent == null)
                    throw new AppException(ErrorMessage.ParentCommentNotFound);
            }

            var comment = new Comment
            {
                Content = model.Content,
                PostId = post.Id,
                UserId = user.Id,
                ParentCommentId = model.ParentCommentId,
                CreatedBy = user.Id
            };

            await _dataContext.Comment.AddAsync(comment);

            await _dataContext.SaveChangesAsync();

            return comment.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task<IEnumerable<CommentResponseModel>> GetCommentsByPostIdAsync(Guid postId)
    {
        try
        {
            var comments = await _dataContext.Comment
                .Where(c => !c.IsDeleted && c.Post!.Id == postId && c.ParentCommentId == null)
                .Include(c => c.User)
                .Include(c => c.Reactions)
                .ToListAsync();

            return comments.Select(c => new CommentResponseModel(c));
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task<Guid> ApproveOrRejectPostAsync(string userId, Guid postId, ConfirmPost model)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
                throw new AppException(ErrorMessage.Unauthorize);

            var userGuid = Guid.Parse(userId);

            var user = await _dataContext.User
                .FirstOrDefaultAsync(u => !u.IsDeleted && u.Id == userGuid);
            if (user == null)
                throw new AppException(ErrorMessage.UserNotFound);

            if (user.Role == UserRole.Member && !user.IsModerator)
                throw new AppException(ErrorMessage.IsModerator);

            var post = await _dataContext.Post
                .FirstOrDefaultAsync(p => !p.IsDeleted && p.Id == postId);
            if (post == null)
                throw new AppException(ErrorMessage.PostNotFound);

            if (post.Status != PostStatus.Pending)
                throw new AppException(ErrorMessage.PostAlreadyConfirm);

            post.Status = model.IsApproved ? PostStatus.Approved : PostStatus.Rejected;
            post.Reason = model.Reason;
            post.ComfirmById = userGuid;
            post.UpdatedAt = DateTime.UtcNow;
            post.UpdatedBy = userGuid;

            _dataContext.Post.Update(post);

            await _dataContext.SaveChangesAsync();

            return post.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task<Guid> UpdatePostAsync(string userId, Guid id, PostUpdateModel model)
    {
        try
        {
            var userGuid = Guid.Parse(userId);

            var post = await _dataContext.Post
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted && p.PostById == userGuid);

            if (post == null)
                throw new Exception(ErrorMessage.PostNotFound);

            var mapper = _mapper.Map(model, post);

            mapper.Status = PostStatus.EditedPendingApproval;

            _dataContext.Post.Update(mapper);

            await _dataContext.SaveChangesAsync();

            return post.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task<string> AddPostToFavoriteList(Guid postId, string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
                throw new Exception(ErrorMessage.Unauthorize);

            var userGuid = new Guid(userId);

            var post = await _dataContext.Post
            .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);

            if (post == null)
                throw new AppException(ErrorMessage.PostNotFound);

            var existingFavorite = await _dataContext.Favorite
                .FirstOrDefaultAsync(f => f.UserId == userGuid && f.PostId == postId);

            if (existingFavorite == null)
            {
                var favorite = new Favorite
                {
                    CreatedBy = userGuid,
                    UserId = userGuid,
                    PostId = postId,
                };

                await _dataContext.Favorite.AddAsync(favorite);
            }

            existingFavorite!.UpdatedAt = DateTime.Now;
            existingFavorite!.UpdatedBy = userGuid;

            await _dataContext.SaveChangesAsync();

            return "Đã lưu vào danh sách yêu thích!";
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

    public async Task<Guid> RemovePostFromFavoriteList(Guid postId, string userId)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
                throw new AppException(ErrorMessage.Unauthorize);

            var userGuid = new Guid(userId);

            var post = await _dataContext.Post
                .FirstOrDefaultAsync(p => p.Id == postId && !p.IsDeleted);

            if (post == null)
                throw new AppException(ErrorMessage.PostNotFound);

            var favorite = await _dataContext.Favorite
                .FirstOrDefaultAsync(f => f.PostId == postId && f.UserId == userGuid);

            if (favorite == null)
                throw new AppException(ErrorMessage.NotFoundPostFromFavoriteList);

            _dataContext.Favorite.Remove(favorite);

            await _dataContext.SaveChangesAsync();

            return favorite.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }
}