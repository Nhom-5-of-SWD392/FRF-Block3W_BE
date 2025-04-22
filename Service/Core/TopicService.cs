using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Utilities;

namespace Service.Core;

public interface ITopicService
{
	Task<PagingModel<TopicViewModel>> GetAll(TopicQueryModel query);
	Task<Guid> Create(string userId,TopicCreateModel model);
	Task<Guid> Update(string userId,Guid id,TopicUpdateModel model);
	Task<Guid> SoftDelete(Guid id);
	Task<PostTopicResponse?> GetPostsByTopicAsync(Guid topicId);
}
public class TopicService : ITopicService
{
	private readonly DataContext _dataContext;
	private readonly IMapper _mapper;
	private readonly ISortHelpers<Topic> _sortHelpers;

	public TopicService(DataContext dataContext, IMapper mapper, ISortHelpers<Topic> sortHelpers)
	{
		_dataContext = dataContext;
		_mapper = mapper;
		_sortHelpers = sortHelpers;
	}
    public async Task<Topic> GetById(Guid id)
    {
        try
        {
            var topic = await _dataContext.Topic
                .FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id);
            if (topic == null)
            {
                throw new AppException(ErrorMessage.TopicNotFound);
            }

            return topic;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new AppException(e.Message);
        }
    }

    public async Task<Guid> Create(string userId, TopicCreateModel model)
	{
		try
		{
			if (string.IsNullOrEmpty(userId))
			{
				throw new AppException(ErrorMessage.Unauthorize);
			}

			var data = _mapper.Map<TopicCreateModel, Topic>(model);

			data.CreatedBy = new Guid(userId);

			await _dataContext.Topic.AddAsync(data);

			await _dataContext.SaveChangesAsync();
		
			return data.Id;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new Exception(e.Message);
		}
	}

	public async Task<Guid> SoftDelete(Guid id)
	{
		try
		{
			var data = await GetById(id);
			if (data == null)
			{
				throw new AppException(ErrorMessage.TopicNotFound);
			}

			data.IsDeleted = true;

			_dataContext.Topic.Update(data);

			await _dataContext.SaveChangesAsync();

			return data.Id;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new Exception(e.Message);
		}
	}

	public async Task<PagingModel<TopicViewModel>> GetAll(TopicQueryModel query)
	{
		try
		{
			var queryTopic = _dataContext.Topic
				.Where(x => !x.IsDeleted);

            queryTopic = queryTopic.SearchByKeyword(t => t.Name, query.Search);

			var sortedData = _sortHelpers.ApplySort(queryTopic, query.OrderBy!);
			var data = await sortedData.ToPagedListAsync(query.PageIndex, query.PageSize);

			var topicView = data.Select(topic =>
			{
				var topicViewModel = _mapper.Map<Topic, TopicViewModel>(topic);
				return topicViewModel;
			}).ToList();

			var pagingData = new PagingModel<TopicViewModel>()
			{
				PageIndex = data.CurrentPage,
				PageSize = data.PageSize,
				TotalCount = data.TotalCount,
				TotalPages = data.TotalPages,
				pagingData = topicView
			};
			return pagingData;

		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			
			throw new Exception(e.Message);
		}
	}

	public async Task<Guid> Update(string userId,Guid id, TopicUpdateModel model)
	{
		try
		{
			if (string.IsNullOrEmpty(userId))
				throw new AppException(ErrorMessage.Unauthorize);

			var data = await GetById(id);

			if (data == null)
				throw new AppException(ErrorMessage.TopicNotFound);

			var updateData = _mapper.Map(model,data);

			updateData.UpdatedBy = new Guid(userId);

			_dataContext.Topic.Update(updateData);

			await _dataContext.SaveChangesAsync();

			return data.Id;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new Exception(e.Message);
		}
	}

    public async Task<PostTopicResponse?> GetPostsByTopicAsync(Guid topicId)
    {
		try
		{
            var topic = await _dataContext.Topic
				.Include(t => t.PostTopics!)
					.ThenInclude(pt => pt.Post!)
						.ThenInclude(p => p.PostBy)
				.FirstOrDefaultAsync(t => t.Id == topicId);

            if (topic == null)
                throw new AppException(ErrorMessage.TopicNotFound);

            var result = new PostTopicResponse
            {
                Id = topic.Id,
                Name = topic.Name,
                Posts = topic.PostTopics!.Select(pt => new PostResponse
                {
                    Id = pt.Post!.Id,
                    Title = pt.Post.Title,
                    Content = pt.Post.Content,
                    AuthorName = pt.Post.PostBy?.FirstName + pt.Post.PostBy?.LastName ?? "Thành viên ẩn danh"
                }).ToList()
            };

            return result;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new Exception(e.Message);
        }
    }

}
