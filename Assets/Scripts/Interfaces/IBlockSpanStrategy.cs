using System.Collections.Generic;
using Scriptables.QuestionSystem;
using UI.CorsiBlockTypes;
using UnityEngine;
using Utilities;

namespace Interfaces
{
    public interface IBlockSpanStrategy
    {
        public void HighlightBlock(AdaptableBlock targetBlock);
        public void SetBlockSelected(AdaptableBlock block, Question selection);
        public bool CheckAnswer(List<Question> displayed, List<Question> given);
        public List<Question> GetCorrectQuestions(List<Question> allQuestions, int count = -1);
    }
}
