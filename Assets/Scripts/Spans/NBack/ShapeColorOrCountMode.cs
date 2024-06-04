using System.Collections.Generic;
using Interfaces;
using Scriptables.QuestionSystem;
using UnityEngine;
using Utilities;

namespace Spans.NBack
{
    public class ShapeColorOrCountMode : INBackStrategy
    {
        public void SetChosenButtonType(CommonFields.ButtonType chosen)
        {
            throw new System.NotImplementedException();
        }

        public bool CheckAnswer()
        {
            throw new System.NotImplementedException();
        }

        public List<Question> GetQuestionByCount(List<Question> questions, int count)
        {
            throw new System.NotImplementedException();
        }

        public int[] GetModeIndexes()
        {
            throw new System.NotImplementedException();
        }
    }
}
