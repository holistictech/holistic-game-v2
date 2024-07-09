using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.ComplexSpan;
using Spans.Skeleton.QuestionStates;

namespace Interfaces
{
    public interface IComplexSpanStrategy
    {
        public void InjectController(ComplexSpan controller);
        public void InjectModeQuestions(List<Question> mainQuestions, List<Question> helperQuestions);
        public void EnableRequiredModeElements(ComplexQuestionState questionState);
        public int GetFixedQuestionCount();
        public List<Question> GetCorrectQuestions(int iterations);
        public bool IsAnsweringMainQuestions();
        public List<Question> GetModeChoices();
        public List<Question> GetCorrectMainQuestions();
        public bool CheckAnswer(List<Question> given);
    }
}
