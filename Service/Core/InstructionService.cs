using AutoMapper;
using Data.EFCore;
using Service.Utilities;

namespace Service.Core;

public interface IInstructionService
{
    Task<string> DeleteInstructionAsync(Guid instructionId);
}

public class InstructionService : IInstructionService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public InstructionService(DataContext dataContext, IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public async Task<string> DeleteInstructionAsync(Guid instructionId)
    {
        var instruction = await _dataContext.Instruction.FindAsync(instructionId)
            ?? throw new AppException("Instruction not found");

        _dataContext.Instruction.Remove(instruction);

        await _dataContext.SaveChangesAsync();

        return "Deleted!";
    }
}
