using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestionDto
{
    public class Question
    {
        public long Id;
        public string Title { get; set; }// text, or image or image and text
        public int IndexNumber { get; set; }
        public List<QuestionOption> Options { get; set; }
    }

    public class QuestionOption
    {
        public string OptionAlphabet { get; set; } //A or B or C or D
        public string OptionTitle { get; set; }
        public bool IsCorrectOption { get; set; }
    }
}
