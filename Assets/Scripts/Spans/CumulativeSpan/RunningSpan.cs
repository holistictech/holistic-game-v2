using System.Collections.Generic;
using Scriptables.QuestionSystem;

namespace Spans.CumulativeSpan
{
    public class RunningSpan : CumulativeImageChooser
    {
        private const int max_fetch_count = 5;

        public override List<Question> GetSpanObjects()
        {
            List<Question> roundQuestions = new List<Question>();
            for (int i = 0; i < max_fetch_count; i++)
            {
                roundQuestions.Add(GetUnusedQuestion());
            }

            currentSpanQuestions.AddRange(roundQuestions);
            return currentSpanQuestions;
        }
    }
}
