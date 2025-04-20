using AutoMapper;
using Data.EFCore;
using Data.Entities;
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


				await _dataContext.Reaction.AddAsync(reaction);

				//Add the reaction to the user

				//Add the reaction to the comment

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
}
