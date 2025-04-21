using AutoMapper;
using Data.EFCore;
using Service.Utilities;

namespace Service.Core;

public interface IRangeScoreService
{
    Task<string> DeleteRangeScoreAsync(Guid id);
}

public class RangeScoreService : IRangeScoreService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public RangeScoreService(DataContext dataContext, IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public async Task<string> DeleteRangeScoreAsync(Guid id)
    {
        var rangeScore = await _dataContext.QuizRangeScore.FindAsync(id)
            ?? throw new AppException("Quiz range score not found");

        _dataContext.QuizRangeScore.Remove(rangeScore);

        await _dataContext.SaveChangesAsync();

        return "Deleted!";
    }
}
