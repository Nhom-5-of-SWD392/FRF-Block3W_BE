using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Enum;
using Data.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Service.Utilities;
using System.Linq.Dynamic.Core;

namespace Service.Core;

public interface IQuizService
{
    Task<PagingModel<QuizViewModel>> GetAll(QuizQueryModel query);
    Task<Guid> CreateFullQuizAsync(string userId, CreateQuizRequest model);
    Task<QuizDetailResponse> GetQuizDetailAsync(Guid quizId);
    Task<Guid> AddQuizRangeScore(string userId, Guid quizId, List<QuizRangeScoreCreateModel> models);
    Task<Guid> SoftDelete(Guid id);
    Task<Guid> HardDelete(Guid id);
    Task<string> SubmitQuizAsync(string userId, SubmitQuizRequest request);
    Task<QuizResultView> GetQuizResultAsync(Guid quizResultId);
    Task<PagingModel<QuizResultView>> GetAllMyQuizResultsAsync(string userId, QuizResultQueryModel query, string role);

}

public class QuizService : IQuizService
{
    private readonly DataContext _dataContext;
    private readonly IMapper _mapper;
    private readonly ISortHelpers<Quiz> _sortQuizHelpers;
    private readonly ISortHelpers<QuizResult> _sortQuizResultHelpers;

    public QuizService (DataContext dataContext, IMapper mapper, ISortHelpers<Quiz> sortQuizHelpers, ISortHelpers<QuizResult> sortQuizResultHelpers)
    {
        _dataContext = dataContext;
        _mapper = mapper;
        _sortQuizHelpers = sortQuizHelpers; 
        _sortQuizResultHelpers = sortQuizResultHelpers;
    }

    public async Task<PagingModel<QuizViewModel>> GetAll(QuizQueryModel query)
    {
        try
        {
            var queryQuiz = _dataContext.Quiz
                .Include(q => q.QuizQuestions)
                .Where(q => !q.IsDeleted);

            queryQuiz = queryQuiz.SearchByKeyword(q => q.Name, query.Search);

            var sortData = _sortQuizHelpers.ApplySort(queryQuiz, query.OrderBy!);

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

    public async Task<Quiz> GetById(Guid id)
    {
        try
        {
            var quiz = await _dataContext.Quiz
                .FirstOrDefaultAsync(q => !q.IsDeleted && q.Id == id);
            if (quiz == null)
            {
                throw new AppException(ErrorMessage.QuizNotExist);
            }

            return quiz;
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
                            var newAnswer = new QuizAnswerModel
                            {
                                AnswerText = answerModel.AnswerText,
                                Score = answerModel.Score,
                                QuizQuestionId = quizQuestionData.Id,
                                QuizId = quizData.Id
                            };
                            var quizAnswerData = _mapper.Map<QuizAnswerModel, QuizAnswer>(newAnswer);
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
        try
        {
            var quiz = await _dataContext.Quiz
            .Include(q => q.QuizQuestions)
                .ThenInclude(q => q.QuizAnswers)
            .Include(q => q.QuizRangeScores)
            .FirstOrDefaultAsync(q => q.Id == quizId && !q.IsDeleted);

            if (quiz == null)
                throw new AppException(ErrorMessage.QuizNotExist);

            var response = new QuizDetailResponse
            {
                Id = quiz.Id,
                Name = quiz.Name,
                Description = quiz.Description,
                Type = quiz.Type,
                QuizRangeScore = quiz.QuizRangeScores.Select(q => new QuizRangeScoreResponse
                {
                    Id = q.Id,
                    MinScore = q.MinScore,
                    MaxScore = q.MaxScore,
                    Result = q.Result
                }).ToList(),
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
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new AppException(e.Message);
        }
    }

    public async Task<Guid> AddQuizRangeScore(string userId, Guid quizId, List<QuizRangeScoreCreateModel> models)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException(ErrorMessage.Unauthorize);
            }

            var existQuiz = await GetById(quizId);

            foreach (var model in models)
            {
                if (model.MinScore > model.MaxScore)
                    throw new AppException(ErrorMessage.MinCantGreaterMax);

                var range = new QuizRangeScore
                {
                    MinScore = model.MinScore,
                    MaxScore = model.MaxScore,
                    Result = model.Result,
                    QuizId = existQuiz.Id
                };

                await _dataContext.QuizRangeScore.AddAsync(range);

                range.CreatedBy = new Guid(userId);
            }

            await _dataContext.SaveChangesAsync();

            return existQuiz.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new AppException(e.Message);
        }
    }

    public async Task<Guid> SoftDelete(Guid id)
    {
        var data = await GetById(id);
        if (data == null)
        {
            throw new AppException(ErrorMessage.QuizNotExist);
        }

        data.IsDeleted = true;

        await _dataContext.SaveChangesAsync();

        return data.Id;
    }

    public async Task<Guid> HardDelete(Guid id)
    {
        try
        {
            var data = await GetById(id);
            if (data == null)
            {
                throw new AppException(ErrorMessage.QuizNotExist);
            }

            _dataContext.Quiz.Remove(data);

            await _dataContext.SaveChangesAsync();

            return data.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new AppException(e.Message);
        }
    }

    public async Task<string> SubmitQuizAsync(string userId, SubmitQuizRequest request)
    {
        using (var transaction = await _dataContext.Database.BeginTransactionAsync())
        {
            try
            {
                if (string.IsNullOrEmpty(userId))
                {
                    throw new AppException(ErrorMessage.Unauthorize);
                }

                var quiz = await _dataContext.Quiz
                    .Include(q => q.QuizQuestions)!
                        .ThenInclude(q => q.QuizAnswers)
                    .Include(q => q.QuizRangeScores)
                    .FirstOrDefaultAsync(q => q.Id == request.QuizId);

                if (quiz == null)
                    throw new AppException(ErrorMessage.QuizNotExist);

                double totalScore = 0;
                var quizDetails = new List<QuizDetail>();

                foreach (var answer in request.Answers)
                {
                    var question = quiz.QuizQuestions!.FirstOrDefault(q => q.Id == answer.QuestionId);
                    if (question == null) continue;

                    var detail = new QuizDetail
                    {
                        QuizQuestionId = answer.QuestionId,
                        QuizId = quiz.Id
                    };

                    if (question.Type == QuestionType.MultipleChoice && answer.AnswerId != null)
                    {
                        detail.QuizAnswerId = answer.AnswerId.Value;
                        var selectedAnswer = question.QuizAnswers!.FirstOrDefault(a => a.Id == answer.AnswerId);
                        if (selectedAnswer != null)
                        {
                            detail.EvaluationScore = selectedAnswer.Score;
                            totalScore += selectedAnswer.Score;
                        }
                    }
                    else if (question.Type == QuestionType.Essay)
                    {
                        detail.EssayAnswerText = answer.EssayAnswer;
                        detail.EvaluationScore = 0;
                    }

                    quizDetails.Add(detail);
                }

                var result = new QuizResult
                {
                    Id = Guid.NewGuid(),
                    QuizId = quiz.Id,
                    QuizMadeById = new Guid(userId),
                    FinalScore = totalScore,
                    Status = quiz.Type == QuizType.Quiz ? QuizResultStatus.Completed : QuizResultStatus.Pending,
                    Result = quiz.Type == QuizType.Quiz ? GetRangeScore(quiz.QuizRangeScores!, totalScore) : RangeScoreResult.None,
                    CreatedBy = new Guid(userId),
                    QuizDetails = quizDetails
                };

                await _dataContext.QuizResult.AddAsync(result);
                await _dataContext.SaveChangesAsync();

                await transaction.CommitAsync();

                return "Submit successfuly!";
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                await transaction.RollbackAsync();
                throw new AppException(e.Message);
            }
        }
    }

    public async Task<QuizResultView> GetQuizResultAsync(Guid quizResultId)
    {
        try
        {
            var result = await _dataContext.QuizResult
            .Include(r => r.Quiz)
            .Include(r => r.QuizDetails)!
                .ThenInclude(d => d.QuizQuestion)
            .Include(r => r.QuizDetails)!
                .ThenInclude(d => d.QuizAnswer)
            .FirstOrDefaultAsync(r => r.Id == quizResultId);

            if (result == null)
                throw new AppException(ErrorMessage.QuizResultNotFound);

            var response = new QuizResultView
            {
                Id = result.Id,
                Quizz = new QuizzModel
                {
                    QuizId = result.Quiz?.Id ?? Guid.Empty,
                    QuizName = result.Quiz?.Name ?? "Unknown Quiz",
                    Type = result.Quiz?.Type ?? QuizType.Quiz
                },
                FinalScore = result.FinalScore,
                Result = result.Result,
                Status = result.Status,
                Details = result.QuizDetails!.Select(d => new QuizResultDetail
                {
                    Question = new QuestionModel
                    {
                        Id = d.QuizQuestion?.Id ?? Guid.Empty,
                        QuestionText = d.QuizQuestion?.QuestionText ?? "",
                        QuestionType = d.QuizQuestion?.Type ?? QuestionType.MultipleChoice
                    },
                    SelectedAnswerText = d.QuizAnswer?.AnswerText,
                    EssayAnswerText = d.EssayAnswerText,
                    EvaluationScore = d.EvaluationScore
                }).ToList()
            };

            return response;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw new AppException(e.Message);
        }
    }

    public async Task<PagingModel<QuizResultView>> GetAllMyQuizResultsAsync(string userId, QuizResultQueryModel query, string role)
    {
        try
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new AppException(ErrorMessage.Unauthorize);
            }

            var queryResult = _dataContext.QuizResult
                .Include(r => r.Quiz)
                .Include(r => r.QuizDetails)
                .Where(r => !r.IsDeleted);

            if (role != UserRole.Administrator.ToString() && !string.IsNullOrEmpty(userId))
            {
                var userGuid = new Guid(userId);
                queryResult = queryResult.Where(r => r.QuizMadeById == userGuid);
            }

            queryResult = queryResult.SearchByKeyword(r => r.Quiz!.Name, query.Search);

            var sorted = _sortQuizResultHelpers.ApplySort(queryResult, query.OrderBy!);

            var pagedData = await sorted.ToPagedListAsync(query.PageIndex, query.PageSize);

            var quizViews = pagedData.Select(result => new QuizResultView
            {
                Id = result.Id,
                Quizz = new QuizzModel
                {
                    QuizId = result.QuizId,
                    QuizName = result.Quiz?.Name ?? "Unknown Quiz",
                    Type = result.Quiz?.Type ?? QuizType.Quiz
                },
                FinalScore = result.FinalScore,
                Result = result.Result,
                Status = result.Status,
                CreatedAt = result.CreatedAt,
                CreatedBy = result.CreatedBy,
            }).ToList();

            return new PagingModel<QuizResultView>
            {
                PageIndex = pagedData.CurrentPage,
                PageSize = pagedData.PageSize,
                TotalCount = pagedData.TotalCount,
                TotalPages = pagedData.TotalPages,
                pagingData = quizViews
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex);
            throw new AppException("Failed to fetch quiz results: " + ex.Message);
        }
    }


    private RangeScoreResult GetRangeScore(IEnumerable<QuizRangeScore> scores, double score)
    {
        foreach (var range in scores)
        {
            if (score >= range.MinScore && score <= range.MaxScore)
            {
                return range.Result;
            }
        }
        return RangeScoreResult.Bad;
    }


    //private method


}
