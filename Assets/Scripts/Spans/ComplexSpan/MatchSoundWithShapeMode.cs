using System;
using System.Collections.Generic;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton.AnswerStates;
using Spans.Skeleton.QuestionStates;
using UnityEngine;
using Utilities.Helpers;

namespace Spans.ComplexSpan
{
    public class MatchSoundWithShapeMode : IComplexSpanStrategy
    {
        private ComplexSpan _controller;
        private ComplexQuestionState _questionState;
        private ComplexAnswerState _answerState;

        private List<Question> _modeQuestions;
        private List<Question> _currentQuestions;
        public void InjectController(ComplexSpan controller)
        {
            _controller = controller;
        }

        public int GetStartingRoundIndex()
        {
            throw new NotImplementedException();
        }

        public void InjectModeQuestions(List<Question> mainQuestions, List<Question> helperQuestions)
        {
            throw new NotImplementedException();
        }

        public void InjectQuestionState(ComplexQuestionState questionState)
        {
            throw new NotImplementedException();
        }

        public void InjectAnswerState(ComplexAnswerState answerState)
        {
            throw new NotImplementedException();
        }

        public void ShowQuestionStateQuestion(Questioner questioner)
        {
            throw new NotImplementedException();
        }

        public void HandleOnComplete()
        {
            throw new NotImplementedException();
        }

        public void ShowAnswerStateQuestion(Questioner questioner, Action onComplete)
        {
            throw new NotImplementedException();
        }

        public int GetCircleCount()
        {
            throw new NotImplementedException();
        }

        public List<Question> GetCorrectQuestions(int iterations)
        {
            throw new NotImplementedException();
        }

        public List<Question> GetModeChoices()
        {
            throw new NotImplementedException();
        }

        public void AppendChoice(CommonFields.ButtonType type)
        {
            throw new NotImplementedException();
        }

        public bool CheckAnswer(List<Question> given)
        {
            throw new NotImplementedException();
        }

        public int GetUnitIndex()
        {
            throw new NotImplementedException();
        }
    }
}
