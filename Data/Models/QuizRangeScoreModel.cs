﻿using Data.Enum;

namespace Data.Models;

public class QuizRangeScoreCreateModel
{
    public int MinScore { get; set; } = 0;
    public int MaxScore { get; set; } = 0;
    public string Result { get; set; } = string.Empty;
}


