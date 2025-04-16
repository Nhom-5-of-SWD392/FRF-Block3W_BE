using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Enum;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Service.Utilities;
using System.Linq.Dynamic.Core;

namespace Service.Core;

public interface IQuizService
{
    Task<PagingModel<QuizViewModel>> GetAll(QuizQueryModel query);
    Task<Guid> CreateFullQuizAsync(string userId, CreateQuizRequest model);
    Task<QuizDetailResponse> GetQuizDetailAsync(Guid quizId);
}

public class QuizService : IQuizService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly ISortHelpers<Quiz> _sortHelpers;

    public QuizService (DataContext dataContext, IMapper mapper, ISortHelpers<Quiz> sortHelpers)
    {
        _dataContext = dataContext;
        _mapper = mapper;
        _sortHelpers = sortHelpers; 
    }

    public async Task<PagingModel<QuizViewModel>> GetAll(QuizQueryModel query)
    {
        try
        {
            var queryQuiz = _dataContext.Quiz
                .Include(q => q.QuizQuestions)
                .Where(q => !q.IsDeleted);

            SearchByKeyWord(ref queryQuiz, query.Search);

            var sortData = _sortHelpers.ApplySort(queryQuiz, query.OrderBy!);

            var data = await sortData.ToPagedListAsync(query.PageIndex, query.PageSize);

            var quizView = data.Select(quiz =>
            {
                var quizViewModel = _mapper.Map<Quiz, QuizViewModel>(quiz);

                quizViewModel.TotalQuestion = quiz.QuizQuestions.Count();

                return quizViewModel;
            }).ToList();    

            var pagingData = new PagingModel<QuizViewModel>()
            {
                PageIndex = data.CurrentPage,
                PageSize = data.PageSize,
                TotalCount = data.TotalCount,
                TotalPages = data.TotalPages,
                pagingData = quizView
            };

            return pagingData;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new AppException(e.Message);
        }
    }

    public async Task<Guid> CreateFullQuizAsync(string userId, CreateQuizRequest model)
    {
        using (var transaction = await _dataContext.Database.BeginTransactionAsync())
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new AppException(ErrorMessage.Unauthorize);
                }

                var newQuiz = new QuizCreateModel
                {
                    Name = model.Name,
                    Description = model.Description,
                    Type = model.Type,
                };
                var quizData = _mapper.Map<QuizCreateModel, Quiz>(newQuiz);

                quizData.CreatedBy = new Guid(userId);

                await _dataContext.Quiz.AddAsync(quizData);

                foreach (var questionModel in model.Questions)
                {
                    if (newQuiz.Type == QuizType.Quiz && questionModel.Type != QuestionType.MultipleChoice)
                        throw new AppException(ErrorMessage.QuizTypeOnlyMulChoice);

                    var newQuestion = new QuizQuestionCreateModel
                    {
                        QuestionText = questionModel.QuestionText,
                        Type = questionModel.Type,
                        QuizId = quizData.Id,
                    };
                    var quizQuestionData = _mapper.Map<QuizQuestionCreateModel, QuizQuestion>(newQuestion);
                    await _dataContext.QuizQuestion.AddAsync(quizQuestionData);

                    // Nếu không phải Essay thì phải có đáp án
                    if (quizQuestionData.Type == QuestionType.MultipleChoice)
                    {
                        if (questionModel.Answers == null || !questionModel.Answers.Any())
                            throw new AppException(ErrorMessage.MulChoiceMustHaveAnswer);

                        foreach (var answerModel in questionModel.Answers)
                        {
                            var newAnswer = new QuizAnswerCreateModel
                            {
                                AnswerText = answerModel.AnswerText,
                                Score = answerModel.Score,
                                QuizQuestionId = quizQuestionData.Id,
                                QuizId = quizData.Id
                            };
                            var quizAnswerData = _mapper.Map<QuizAnswerCreateModel, QuizAnswer>(newAnswer);
                            await _dataContext.QuizAnswer.AddAsync(quizAnswerData);
                        }
                    }
                }

                await _dataContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return quizData.Id;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await transaction.RollbackAsync();
                throw new AppException(e.Message);
            }

        }
    }

    public async Task<QuizDetailResponse> GetQuizDetailAsync(Guid quizId)
    {
        var quiz = await _dataContext.Quiz
            .Include(q => q.QuizQuestions)
                .ThenInclude(q => q.QuizAnswers)
            .FirstOrDefaultAsync(q => q.Id == quizId && !q.IsDeleted);

        if (quiz == null)
            throw new AppException(ErrorMessage.QuizNotExist);

        var response = new QuizDetailResponse
        {
            Id = quiz.Id,
            Name = quiz.Name,
            Description = quiz.Description,
            Type = quiz.Type,
            Questions = quiz.QuizQuestions.Select(q => new QuizQuestionResponse
            {
                Id = q.Id,
                QuestionText = q.QuestionText,
                Type = q.Type,
                Answers = q.QuizAnswers?.Select(a => new QuizAnswerResponse
                {
                    Id = a.Id,
                    AnswerText = a.AnswerText,
                    Score = a.Score
                }).ToList()
            }).ToList()
        };

        return response;
    }


    //private method
    private void SearchByKeyWord(ref IQueryable<Quiz> quiz, string keyword)
    {
        if (!quiz.Any() || string.IsNullOrWhiteSpace(keyword))
            return;
        quiz = quiz.Where(o => o.Name.ToLower().Contains(keyword.Trim().ToLower()));
    }
}
