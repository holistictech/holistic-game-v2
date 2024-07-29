using System;
using System.Collections.Generic;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton.AnswerStates;
using Spans.Skeleton.QuestionStates;
using UI.Helpers;
using UnityEngine;
using Utilities.Helpers;
using Random = System.Random;

namespace Spans.ComplexSpan
{
    public class BlockAndNumberSpanStrategy : IComplexSpanStrategy
    {
        private ComplexSpan _controller;
        private ComplexQuestionState _questionState;
        private ComplexAnswerState _answerState;
        private List<Question> _modeQuestions = new List<Question>();
        private List<Question> _currentQuestions = new List<Question>();
        private List<Question> _currentNumberQuestions = new List<Question>();
        private CorsiBlockUIHelper _blockGrid;
        
        public void InjectController(ComplexSpan controller)
        {
            _controller = controller;
        }

        public int GetStartingRoundIndex()
        {
            return 2;
        }

        public void InjectModeQuestions(List<Question> mainQuestions, List<Question> helperQuestions)
        {
            _modeQuestions.AddRange(mainQuestions);
            _modeQuestions.AddRange(helperQuestions);
        }

        public void InjectQuestionState(ComplexQuestionState questionState)
        {
            _questionState = questionState;
            _blockGrid = _questionState.GetGridHelper();
            _blockGrid.GetCorsiBlocks();
            _blockGrid.AssignQuestions(_modeQuestions);
        }

        public void InjectAnswerState(ComplexAnswerState answerState)
        {
            _answerState = answerState;
            _answerState.EnableButtons();
            var circles = _controller.GetActiveCircles();
            _blockGrid.SetActiveCircles(circles);
            _blockGrid.ConfigureInput(true);
        }

        private bool _blocksDisplayed;
        private int _currentQuestionIndex = 0;
        private bool _shouldPass = true;
        public void ShowQuestionStateQuestion(Questioner questioner)
        {
            if (_currentQuestionIndex >= _currentQuestions.Count)
            {
                if (_shouldPass)
                {
                    _questionState.SwitchNextState();
                    _controller.SetMainSpanNeeded(true);
                    _shouldPass = false;
                    return;

                }

                if (_shouldPass) return;
                _controller.GetSpanObjects();
                _currentQuestionIndex = 0;
                ShowQuestions(questioner);
            }
            else
            {
                ShowQuestions(questioner);
            }
        }

        private void ShowQuestions(Questioner questioner)
        {
            _blockGrid = _questionState.GetGridHelper();
            questioner.InjectQuestionState(_questionState);
            var count = 0;
            if (_blocksDisplayed)
            {
                count = 1;
                _blocksDisplayed = false;
                questioner.PlayCoroutine(_currentQuestions.GetRange(_currentQuestionIndex, 1), this, _questionState);
            }
            else
            {
                count = 3;
                _blocksDisplayed = true;
                questioner.PlayBlockSpanRoutine(_currentQuestions.GetRange(_currentQuestionIndex, count), this, _blockGrid);
            }
            
            Debug.Log($"Showed {count} questions in block display turn {_blocksDisplayed}");
            
            _currentQuestionIndex += count;
        }

        public void HandleOnComplete()
        {
            _blockGrid.ResetCorsiBlocks();
            _blockGrid.gameObject.SetActive(false);
            _questionState.SwitchNextState();
        }

        public void ShowAnswerStateQuestion(Questioner questioner, Action onComplete)
        {
            if (_controller.GetIsMainSpanNeeded())
            {
                _answerState.SetChoiceUI();
                _answerState.TriggerHintHelper("Sırasıyla hangi sayıları görmüştün?", () =>
                {
                    onComplete?.Invoke();
                });
            }
            else
            {
                if (_blocksDisplayed)
                {
                    var blockGrid = _questionState.GetGridHelper();
                    blockGrid.gameObject.SetActive(true);
                    onComplete?.Invoke();
                }
                else
                {
                    _answerState.SwitchNextState();
                }
            }
        }

        public int GetCircleCount()
        {
            return _blocksDisplayed ? 1 : 3;
        }

        public List<Question> GetCorrectQuestions(int iterations)
        {
            for (int i = 0; i < iterations; i++)
            {
                var iterationQuestions = new List<Question>();
                for (int j = 0; j < 4; j++)
                {
                    var randomQuestion = _modeQuestions[UnityEngine.Random.Range(0, _modeQuestions.Count)];
                    while (iterationQuestions.Contains(randomQuestion))
                    {
                        randomQuestion = _modeQuestions[UnityEngine.Random.Range(0, _modeQuestions.Count)];
                    }
                    iterationQuestions.Add(randomQuestion);
                    _currentQuestions.Add(randomQuestion);
                }
                
                _currentNumberQuestions.Add(_currentQuestions[^1]);
            }
            return _currentQuestions;
        }

        public List<Question> GetModeChoices()
        {
            var choices = new List<Question>(_currentNumberQuestions);
            var iterations = _currentNumberQuestions.Count;
            for (int i = 0; i < iterations; i++)
            {
                var randomQuestion = _modeQuestions[UnityEngine.Random.Range(0, _modeQuestions.Count)];
                while (choices.Contains(randomQuestion))
                {
                    randomQuestion = _modeQuestions[UnityEngine.Random.Range(0, _modeQuestions.Count)];
                }
                
                choices.Add(randomQuestion);
            }
            
            return choices;
        }

        public void AppendChoice(CommonFields.ButtonType type)
        {
            throw new NotImplementedException();
        }

        public bool CheckAnswer(List<Question> given)
        {
            _blockGrid.gameObject.SetActive(false);
            if (_blocksDisplayed)
            {
                given = _blockGrid.GetGivenAnswers();
                var spanQuestions = _currentQuestions.GetRange(_currentQuestionIndex - 3, 3);
                if (spanQuestions.Count != given.Count) return false;
                for (int i = 0; i < spanQuestions.Count; i++)
                {
                    if (!spanQuestions[i].IsEqual(given[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                if (_currentNumberQuestions.Count != given.Count) return false;
                for (int i = 0; i < _currentNumberQuestions.Count; i++)
                {
                    if (!_currentNumberQuestions[i].IsEqual(given[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool ShouldSwitchDirectly()
        {
            return _currentQuestionIndex < _currentQuestions.Count;
        }

        public int GetUnitIndex()
        {
            return 0;
        }
    }
}
