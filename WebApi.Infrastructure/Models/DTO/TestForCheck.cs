﻿namespace WebApi.Infrastructure.Models.DTO;

public class TestForCheck
{
    public int UserId { get; set; }
    
    public List<UserAnswer> Answers { get; set; }
}

public class UserAnswer
{
    public int TaskId { get; set; }
    public string Answer { get; set; }
}