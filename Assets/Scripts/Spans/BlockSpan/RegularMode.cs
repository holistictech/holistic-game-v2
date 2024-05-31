using System.Collections.Generic;
using Interfaces;
using Scriptables.QuestionSystem;
using UI.CorsiBlockTypes;

namespace Spans.BlockSpan
{
    public class RegularMode : IBlockSpanStrategy
    {
        public void HighlightBlock(AdaptableBlock targetBlock)
        {
            throw new System.NotImplementedException();
        }

        public void SetBlockSelected(AdaptableBlock block, Question selection)
        {
            throw new System.NotImplementedException();
        }

        public void CheckAnswer()
        {
            throw new System.NotImplementedException();
        }

        public List<Question> GetCorrectQuestions(List<Question> allQuestions)
        {
            throw new System.NotImplementedException();
        }
    }
}
