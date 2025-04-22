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

namespace Service.Core;

public interface IReactionService
{
	Task<Guid> CreateReaction(ReactionCreateModel model, string userId);
	Task<ReactionNumberViewModel> GetReactionByCommentId(Guid id);
	
}
public class ReactionService : IReactionService
{
	private readonly DataContext _dataContext;
	private readonly IMapper _mapper;
	//private readonly ICommentService _commentService;
	private readonly IUserService _userService;
	public ReactionService(DataContext dataContext, IMapper mapper)
	{
		_dataContext = dataContext;
		_mapper = mapper;
	}

	public async Task<Guid> CreateReaction(ReactionCreateModel model, string userId)
	{
		using (var transaction = _dataContext.Database.BeginTransaction())
		{
			try
			{
				var reaction = _mapper.Map<ReactionCreateModel, Reaction>(model);

				reaction.CreatedBy = new Guid(userId);
				reaction.UserId = new Guid(userId);
				reaction.CommentId = model.CommentId;

				await _dataContext.Reaction.AddAsync(reaction);

	
				await _dataContext.SaveChangesAsync();
				await transaction.CommitAsync();

				return reaction.Id;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				await transaction.RollbackAsync();
				throw new Exception(e.Message);
			}
		}
	}

	public async Task<ReactionViewModel> GetById(Guid id)
	{
		try
		{
			try
			{
				var reaction = await _dataContext.Reaction
					.FirstOrDefaultAsync(t => !t.IsDeleted && t.Id == id);
				if (reaction == null)
				{
					throw new AppException(ErrorMessage.TopicNotFound);
				}
				var view = _mapper.Map<Reaction, ReactionViewModel>(reaction);
				if(reaction.User != null)
				{
					view.UserName = reaction.User.UserName;
				}
				else
				{
					view.UserName = "Unknown";
				}
				return view;
			}
			catch (Exception e)
			{
				Console.WriteLine(e);
				throw new AppException(e.Message);
			}
		}
		catch (Exception e)
		{

			Console.WriteLine(e);
			throw new Exception(e.Message);
		}
	}

	public async Task<ReactionNumberViewModel> GetReactionByCommentId(Guid id)
	{
		try
		{
			var reactionCounts = await _dataContext.Reaction
			.Where(r => r.CommentId == id)
			.GroupBy(r => r.ReactionType)
				.Select(g => new
				 {
					ReactionType = g.Key,
					Count = g.Count()
				})
			.ToListAsync();

			// Create the view model with the comment id
			var viewModel = new ReactionNumberViewModel
			{
				CommentId = id,
				Love = 0,
				Happy = 0,
				Bad = 0,
				Sad = 0,
				Like = 0				
			};

			// Populate the counts for each reaction type
			foreach (var reactionCount in reactionCounts)
			{
				switch (reactionCount.ReactionType)
				{
					case ReactionType.Love:
						viewModel.Love = reactionCount.Count;
						break;
					case ReactionType.Happy:
						viewModel.Happy = reactionCount.Count;
						break;
					case ReactionType.Bad:
						viewModel.Bad = reactionCount.Count;
						break;
					case ReactionType.Sad:
						viewModel.Sad = reactionCount.Count;
						break;
					case ReactionType.Like:
						viewModel.Like = reactionCount.Count;
						break;
					
				}
			}

			return viewModel;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new Exception(e.Message);
		}
	}


	
}


