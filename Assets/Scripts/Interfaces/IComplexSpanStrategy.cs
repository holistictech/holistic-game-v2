using System;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using Spans.ComplexSpan;
using Spans.Skeleton.AnswerStates;
using Spans.Skeleton.QuestionStates;
using Utilities.Helpers;

namespace Interfaces
{
    public interface IComplexSpanStrategy
    {
        public void InjectController(ComplexSpan controller);
        public int GetStartingRoundIndex();
        public void InjectModeQuestions(List<Question> mainQuestions, List<Question> helperQuestions);
        public void EnableRequiredModeElements(ComplexQuestionState questionState);
        public void InjectAnswerState(ComplexAnswerState answerState);
        public void ShowQuestionStateQuestion(Questioner questioner);
        public void HandleOnComplete();
        public void ShowAnswerStateQuestion(Questioner questioner, Action onComplete);
        public int GetCircleCount();
        public List<Question> GetCorrectQuestions(int iterations);
        public List<Question> GetModeChoices();
        public void AppendChoice(CommonFields.ButtonType type);
        public bool CheckAnswer(List<Question> given);
        public int GetUnitIndex();
    }
}
