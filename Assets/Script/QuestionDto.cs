using System;
using System.Collections.Generic;
using UnityEngine;

public class QuestionDto
{
    public class Question
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Image { get; set; }
        public int IndexNumber { get; set; }
        public List<QuestionOption> Options { get; set; }
    }

    public class QuestionOption
    {
        //public string OptionAlphabet { get; set; } //A or B or C or D
        public string Title { get; set; }
        public bool IsCorrectOption { get; set; }
    }
}

[Serializable]
public class QuestionOption
{
    public int index;

    public GameObject option;
}