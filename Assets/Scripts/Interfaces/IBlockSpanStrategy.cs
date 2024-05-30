using System.Collections.Generic;
using Scriptables.QuestionSystem;
using UI.CorsiBlockTypes;
using UnityEngine;

namespace Interfaces
{
    public interface IBlockSpanStrategy
    {
        public void HighlightBlock(AdaptableBlock targetBlock);
        public void CheckAnswer();
        public List<Question> GetCorrectQuestions(List<Question> allQuestions);
    }
}
