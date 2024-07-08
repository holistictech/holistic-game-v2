using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.ComplexSpan;

namespace Interfaces
{
    public interface IComplexSpanStrategy
    {
        public void InjectController(ComplexSpan controller);
        public void InjectModeQuestions(List<Question> mainQuestions, List<Question> helperQuestions);
        public List<Question> GetCorrectQuestions(int iterations);
        public List<Question> GetModeChoices();
    }
}
