using AutoMapper;
using Data.EFCore;
using Microsoft.EntityFrameworkCore;
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
        var rangeScore = await _dataContext.QuizRangeScore
            .FirstOrDefaultAsync(qrs => !qrs.IsDeleted && qrs.Id == id) 
            ?? throw new AppException(ErrorMessage.QuizRangeScoreNotFound);

        _dataContext.QuizRangeScore.Remove(rangeScore);

        await _dataContext.SaveChangesAsync();

        return "Đã xóa!";
    }
}
