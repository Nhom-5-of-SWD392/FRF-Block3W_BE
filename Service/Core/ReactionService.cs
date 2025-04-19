using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Core;

public interface IReactionService
{
	Task<Guid> CreateReaction(ReactionCreateModel model, string userId);
	Task<ReactionViewModel> GetById(Guid id);
}
public class ReactionService : IReactionService
{
	private readonly DataContext _dataContext;
	private readonly IMapper _mapper;

	public ReactionService(DataContext dataContext, IMapper mapper)
	{
		_dataContext = dataContext;
		_mapper = mapper;
	}

	public async Task<Guid> CreateReaction(ReactionCreateModel model, string userId)
	{
		try
		{
			var reaction = _mapper.Map<ReactionCreateModel,Reaction>(model);

			reaction.CreatedBy = new Guid(userId);
			reaction.UserId = new Guid(userId);

			await _dataContext.Reaction.AddAsync(reaction);

			await _dataContext.SaveChangesAsync();

			return  reaction.Id;
		}
		catch (Exception e)
		{
			Console.WriteLine(e);
			throw new Exception(e.Message);
		}
	}

	public Task<ReactionViewModel> GetById(Guid id)
	{
		throw new NotImplementedException();
	}
}
