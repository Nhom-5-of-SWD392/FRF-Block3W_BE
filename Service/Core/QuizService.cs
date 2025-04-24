using AutoMapper;
using Data.EFCore;
using Data.Entities;
using Data.Enum;
using Data.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
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
    Task<Guid> SubmitQuizAsync(string userId, SubmitQuizRequest request);
    Task<QuizResultView> GetQuizResultAsync(Guid quizResultId);
    Task<PagingModel<QuizResultView>> GetAllMyQuizResultsAsync(string userId, QuizResultQueryModel query, string role);
    Task<Guid> EvaluateInterviewAsync(string evaluatorId, EvaluateEssayRequest model);

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
        if (string.IsNullOrEmpty(userId))
            throw new AppException(ErrorMessage.Unauthorize);

        using var transaction = await _dataContext.Database.BeginTransactionAsync();

        try
        {
            var newQuiz = new Quiz
            {
                Name = model.Name,
                Description = model.Description,
                Type = model.Type,
                CreatedBy = new Guid(userId),
            };

            await _dataContext.Quiz.AddAsync(newQuiz);

            var allQuestions = new List<QuizQuestion>();
            var allAnswers = new List<QuizAnswer>();
            int totalMaxScore = 0;

            foreach (var questionModel in model.Questions)
            {
                if (newQuiz.Type == QuizType.Quiz && questionModel.Type != QuestionType.MultipleChoice)
                    throw new AppException(ErrorMessage.QuizTypeOnlyMulChoice);

                var newQuestion = new QuizQuestion
                {
                    QuestionText = questionModel.QuestionText,
                    Type = questionModel.Type,
                    QuizId = newQuiz.Id,
                    CreatedBy = new Guid(userId),
                };

                allQuestions.Add(newQuestion);

                int maxScore = 0;

                if (questionModel.Answers != null && questionModel.Answers.Any())
                {
                    foreach (var answerModel in questionModel.Answers)
                    {
                        var answer = new QuizAnswer
                        {
                            AnswerText = questionModel.Type == QuestionType.Essay ? null : answerModel.AnswerText ?? null,
                            Score = answerModel.Score,
                            QuizQuestion = newQuestion,
                            QuizId = newQuiz.Id
                        };

                        allAnswers.Add(answer);

                        maxScore = Math.Max(maxScore, answerModel.Score);
                    }

                    totalMaxScore += maxScore;
                }
                else if (questionModel.Type == QuestionType.MultipleChoice)
                {
                    throw new AppException(ErrorMessage.MulChoiceMustHaveAnswer);
                }
            }

            await _dataContext.QuizQuestion.AddRangeAsync(allQuestions);
            await _dataContext.QuizAnswer.AddRangeAsync(allAnswers);

            ValidateRangeScores(model.QuizRangeScore, totalMaxScore);

            var rangeScores = model.QuizRangeScore.Select(r => new QuizRangeScore
            {
                Id = Guid.NewGuid(),
                QuizId = newQuiz.Id,
                MinScore = r.MinScore,
                MaxScore = r.MaxScore,
                Result = r.Result,
                CreatedBy = new Guid(userId)
            });

            await _dataContext.QuizRangeScore.AddRangeAsync(rangeScores);

            await _dataContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return newQuiz.Id;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            await transaction.RollbackAsync();
            throw new AppException(e.Message);
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

    public async Task<Guid> SubmitQuizAsync(string userId, SubmitQuizRequest request)
    {
        if (string.IsNullOrEmpty(userId))
            throw new AppException(ErrorMessage.Unauthorize);

        var userGuid = Guid.Parse(userId);

        await using var transaction = await _dataContext.Database.BeginTransactionAsync();

        try
        {
            var quiz = await _dataContext.Quiz
                .Include(q => q.QuizQuestions)!
                    .ThenInclude(q => q.QuizAnswers)
                .Include(q => q.QuizRangeScores)
                .FirstOrDefaultAsync(q => q.Id == request.QuizId);

            if (quiz == null)
                throw new AppException(ErrorMessage.QuizNotExist);

            var quizResult = await _dataContext.QuizResult
                .Include(qr => qr.QuizDetails)
                .FirstOrDefaultAsync(qr => qr.QuizId == request.QuizId && qr.CreatedBy == userGuid);

            if (quizResult == null)
                throw new AppException(ErrorMessage.QuizResultNotFound);

            double totalScore = 0;
            var newDetails = new List<QuizDetail>();

            foreach (var answer in request.Answers)
            {
                var question = quiz.QuizQuestions!.FirstOrDefault(q => q.Id == answer.QuestionId);
                if (question == null) continue;

                var detail = new QuizDetail
                {
                    QuizQuestionId = question.Id,
                    QuizId = quiz.Id
                };

                switch (question.Type)
                {
                    case QuestionType.MultipleChoice when answer.AnswerId.HasValue:
                        var selectedAnswer = question.QuizAnswers?.FirstOrDefault(a => a.Id == answer.AnswerId.Value);
                        if (selectedAnswer != null)
                        {
                            detail.QuizAnswerId = selectedAnswer.Id;
                            detail.EvaluationScore = selectedAnswer.Score;
                            totalScore += selectedAnswer.Score;
                        }
                        break;

                    case QuestionType.Essay:
                        detail.EssayAnswerText = answer.EssayAnswer;
                        detail.EvaluationScore = 0;
                        break;
                }

                newDetails.Add(detail);
            }

            var isAutoEvaluated = quiz.Type == QuizType.Quiz;

            quizResult.QuizDetails = newDetails;
            quizResult.FinalScore = isAutoEvaluated ? totalScore : 0;
            quizResult.Status = isAutoEvaluated ? QuizResultStatus.Completed : QuizResultStatus.Pending;
            quizResult.Result = isAutoEvaluated
                ? GetRangeScore(quiz.QuizRangeScores!, totalScore)
                : "Đang chờ đánh giá...";
            quizResult.UpdatedAt = DateTime.UtcNow;
            quizResult.UpdatedBy = userGuid;

            _dataContext.QuizResult.Update(quizResult);
            await _dataContext.SaveChangesAsync();
            await transaction.CommitAsync();

            return quizResult.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Lỗi khi nộp bài quiz: {ex}");
            throw new AppException(ex.Message);
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
                        .ThenInclude(q => q.QuizAnswers)
                .Include(r => r.QuizDetails)!
                    .ThenInclude(d => d.QuizAnswer)
                .FirstOrDefaultAsync(r => r.Id == quizResultId);

            if (result == null)
                throw new AppException(ErrorMessage.QuizResultNotFound);

            var response = new QuizResultView
            {
                Id = result.Id,
                CreatedBy = result.CreatedBy,
                UpdatedBy = result.UpdatedBy,
                Quizz = new QuizzModel
                {
                    QuizId = result.Quiz?.Id ?? Guid.Empty,
                    QuizName = result.Quiz?.Name ?? "Unknown Quiz",
                    Type = result.Quiz?.Type ?? QuizType.Quiz
                },
                FinalScore = result.FinalScore,
                Result = result.Result,
                Status = result.Status,
                Details = result.QuizDetails!.Select(d =>
                {
                    var question = d.QuizQuestion;
                    var questionType = d.QuizQuestion?.Type ?? QuestionType.MultipleChoice;
                    int maxScore = 0;

                    if (questionType == QuestionType.MultipleChoice)
                    {
                        //maxScore = d.QuizAnswer?.Score ?? 0;
                        maxScore = question?.QuizAnswers?.Max(a => a.Score) ?? 0;
                    }
                    else if (questionType == QuestionType.Essay)
                    {
                        maxScore = d.QuizQuestion?.QuizAnswers?.FirstOrDefault()?.Score ?? 0;
                    }

                    return new QuizResultDetail
                    {
                        Question = new QuestionModel
                        {
                            Id = d.QuizQuestion?.Id ?? Guid.Empty,
                            QuestionText = d.QuizQuestion?.QuestionText ?? "",
                            QuestionType = questionType
                        },
                        MaxScoreForAnswer = maxScore,
                        SelectedAnswerText = d.QuizAnswer?.AnswerText,
                        EssayAnswerText = d.EssayAnswerText,
                        EvaluationScore = d.EvaluationScore
                    };
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
                queryResult = queryResult.Where(r => r.UpdatedBy == userGuid);
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
            throw new AppException(ex.Message);
        }
    }

    public async Task<Guid> EvaluateInterviewAsync(string evaluatorId, EvaluateEssayRequest model)
    {
        if (string.IsNullOrEmpty(evaluatorId))
            throw new AppException(ErrorMessage.Unauthorize);

        var evaluatorGuid = Guid.Parse(evaluatorId);

        await using var transaction = await _dataContext.Database.BeginTransactionAsync();

        try
        {
            var result = await _dataContext.QuizResult
                .Include(r => r.QuizDetails!)
                    .ThenInclude(q => q.QuizQuestion)
                        .ThenInclude(q => q.QuizAnswers)
                .Include(r => r.Quiz)
                    .ThenInclude(q => q.QuizRangeScores)
                .FirstOrDefaultAsync(r => r.Id == model.QuizResultId && !r.IsDeleted);

            if (result == null || result.Status != QuizResultStatus.Pending)
                throw new AppException(ErrorMessage.QuizResultNotFoundOrEvaluated);

            double finalScore = 0;

            foreach (var detail in result.QuizDetails!)
            {
                var question = detail.QuizQuestion;
                if (question == null) continue;

                if (question.Type == QuestionType.Essay)
                {
                    if (!model.EssayScores.TryGetValue(detail.QuizQuestionId, out var score))
                        throw new AppException(ErrorMessage.QuestionNotFound);

                    var maxScore = question.QuizAnswers?.Max(a => a.Score) ?? 0;

                    if (score > maxScore)
                        throw new AppException($"Điểm không thể vượt quá {maxScore} cho câu hỏi này.");

                    detail.EvaluationScore = score;

                    finalScore += score;
                }
                else
                {
                    finalScore += detail.EvaluationScore;
                }
            }

            result.FinalScore = finalScore;
            result.EvaluateById = evaluatorGuid;
            result.Status = QuizResultStatus.Completed;
            result.Result = GetRangeScore(result.Quiz!.QuizRangeScores!, finalScore);

            var user = await _dataContext.User.FirstOrDefaultAsync(u => u.Id == result.UpdatedBy && !u.IsDeleted);
            if (user == null)
                throw new AppException(ErrorMessage.UserNotFound);

            user.IsModerator = true;
            _dataContext.Update(user);

            await _dataContext.SaveChangesAsync();

            await transaction.CommitAsync();

            return result.Id;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            Console.WriteLine($"Lỗi khi chấm điểm bài phỏng vấn: {ex}");
            throw new AppException(ex.Message);
        }
    }


    //private method
    private string GetRangeScore(IEnumerable<QuizRangeScore> rangeScores, double score)
    {
        foreach (var range in rangeScores)
        {
            if (score >= range.MinScore && score <= range.MaxScore)
            {
                return range.Result;
            }
        }
        return "Không trong phạm vi điểm";
    }

    private void ValidateRangeScores(List<QuizRangeScoreAddToQuiz> ranges, int totalMaxScore)
    {
        if (ranges == null || !ranges.Any()) return;

        var sorted = ranges.OrderBy(r => r.MinScore).ToList();

        for (int i = 0; i < sorted.Count; i++)
        {
            var current = sorted[i];

            if (current.MinScore > current.MaxScore)
                throw new AppException(ErrorMessage.MinCantGreaterMax);

            if (current.MinScore < 0 || current.MaxScore > totalMaxScore)
                throw new AppException($"Điểm số của bài kiểm tra phải nằm trong khoảng từ 0 đến {totalMaxScore}.");

            if (i > 0 && current.MinScore <= sorted[i - 1].MaxScore)
                throw new AppException(ErrorMessage.NotOverlap);
        }
    }
}
