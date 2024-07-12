using System;
using System.Collections.Generic;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton.AnswerStates;
using Spans.Skeleton.QuestionStates;
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
            _modeQuestions.AddRange(mainQuestions);
            _modeQuestions.AddRange(helperQuestions);
        }

        public void InjectQuestionState(ComplexQuestionState questionState)
        {
            _questionState = questionState;
            var blockGrid = _questionState.GetGridHelper();
            blockGrid.GetCorsiBlocks();
            blockGrid.AssignQuestions(_modeQuestions);
        }

        public void InjectAnswerState(ComplexAnswerState answerState)
        {
            _answerState = answerState;
        }

        private bool _blocksDisplayed;
        private int _currentQuestionIndex = 0;
        public void ShowQuestionStateQuestion(Questioner questioner)
        {
            if (_currentQuestionIndex >= _currentQuestions.Count)
            {
                if (_controller.GetIsMainSpanNeeded())
                {
                    _controller.GetSpanObjects();
                    _currentQuestionIndex = 0;
                }
                else
                {
                    _controller.SetMainSpanNeeded(true);
                    _questionState.SwitchNextState();
                    return;
                }
            }
            
            var blockGrid = _questionState.GetGridHelper();
            var count = 0;
            if (_blocksDisplayed)
            {
                count = 1;
                _blocksDisplayed = false;
            }
            else
            {
                count = 3;
                _blocksDisplayed = true;
            }
            
            questioner.PlayBlockSpanRoutine(_currentQuestions.GetRange(_currentQuestionIndex, count), this, blockGrid);
            _currentQuestionIndex += count;
            blockGrid.ResetCorsiBlocks();
            blockGrid.gameObject.SetActive(false);
        }

        public void HandleOnComplete()
        {
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
                for (int j = 0; j < 4; j++)
                {
                    var randomQuestion = _modeQuestions[UnityEngine.Random.Range(0, _modeQuestions.Count)];
                    while (_currentQuestions.Contains(randomQuestion))
                    {
                        randomQuestion = _modeQuestions[UnityEngine.Random.Range(0, _modeQuestions.Count)];
                    }
                
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
            if (_blocksDisplayed)
            {
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

        public int GetUnitIndex()
        {
            return 0;
        }
    }
}
