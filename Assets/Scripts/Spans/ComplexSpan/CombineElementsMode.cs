using System;
using System.Collections.Generic;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton.AnswerStates;
using Spans.Skeleton.QuestionStates;
using UnityEngine;
using Utilities.Helpers;
using Random = UnityEngine.Random;

namespace Spans.ComplexSpan
{
    public class CombineElementsMode : IComplexSpanStrategy
    {
        private ComplexSpan _controller;
        private ComplexQuestionState _questionState;
        private ComplexAnswerState _answerState;
        private List<Question> _modeQuestions = new List<Question>();

        private ComplexShapeQuestion _currentQuestion;
        
        public void InjectController(ComplexSpan controller)
        {
            _controller = controller;
        }

        public int GetStartingRoundIndex()
        {
            return 3;
        }

        public void InjectModeQuestions(List<Question> mainQuestions, List<Question> helperQuestions)
        {
            _modeQuestions.AddRange(mainQuestions);
            _modeQuestions.AddRange(helperQuestions);
        }

        public void InjectQuestionState(ComplexQuestionState questionState)
        {
            _questionState = questionState;
        }

        public void InjectAnswerState(ComplexAnswerState answerState)
        {
            _answerState = answerState;
        }

        public void ShowQuestionStateQuestion(Questioner questioner)
        {
            questioner.InjectQuestionState(_questionState);
            questioner.PlayComplexShapeCoroutine(new List<ComplexShapeQuestion>(){_currentQuestion}, this, _questionState);
        }

        public void HandleOnComplete()
        {
            _controller.SwitchState();
        }

        public void ShowAnswerStateQuestion(Questioner questioner, Action onComplete)
        {
            _answerState.ConfigureUnitCircles();
            _answerState.EnableButtons();
            _answerState.SetChoiceUI();
            onComplete?.Invoke();
        }

        public int GetCircleCount()
        {
            return 3;
        }

        public List<Question> GetCorrectQuestions(int iterations)
        {
            List<Question> variation = new List<Question>();
            var tempQuestion = _modeQuestions[Random.Range(0, _modeQuestions.Count)];
            _currentQuestion = (ComplexShapeQuestion)tempQuestion;
            variation.Add(tempQuestion);

            return variation;
        }

        public List<Question> GetModeChoices()
        {
            List<Question> choices = new List<Question>();
            choices.Add(_currentQuestion);
            for (int i = 0; i < 3; i++)
            {
                var tempQuestion = _modeQuestions[Random.Range(0, _modeQuestions.Count)];
                while (choices.Contains(tempQuestion))
                {
                    tempQuestion = _modeQuestions[Random.Range(0, _modeQuestions.Count)];
                }
                
                choices.Add(tempQuestion);
            }

            return choices;
        }

        public void AppendChoice(CommonFields.ButtonType type)
        {
            Debug.Log("Not related with this game mode");
        }

        public bool CheckAnswer(List<Question> given)
        {
            var answer = given[0];
            return _currentQuestion.IsEqual(answer);
        }

        public int GetUnitIndex()
        {
            return 0;
        }
    }
}
