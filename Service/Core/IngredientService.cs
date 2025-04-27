using AutoMapper;
using Data.EFCore;
using Microsoft.EntityFrameworkCore;
using Service.Utilities;

namespace Service.Core;

public interface IIngredientService
{
    Task<string> DeleteIngredientAsync(Guid IngredientId);
}

public class IngredientService : IIngredientService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;

    public IngredientService(DataContext dataContext, IMapper mapper)
    {
        _dataContext = dataContext;
        _mapper = mapper;
    }

    public async Task<string> DeleteIngredientAsync(Guid ingredientId)
    {
        var ingredient = await _dataContext.Ingredient
            .FirstOrDefaultAsync(i => !i.IsDeleted && i.Id == ingredientId) 
            ?? throw new AppException(ErrorMessage.IngredientNotFound);

        _dataContext.Ingredient.Remove(ingredient);

        await _dataContext.SaveChangesAsync();

        return "Đã xóa!";
    }
}
