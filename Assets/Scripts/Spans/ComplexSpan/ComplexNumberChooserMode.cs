using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton.AnswerStates;
using Spans.Skeleton.QuestionStates;
using UnityEngine;
using Utilities.Helpers;
using Random = UnityEngine.Random;

namespace Spans.ComplexSpan
{
    public class ComplexNumberChooserMode : IComplexSpanStrategy
    {
        private ComplexSpan _controller;
        private List<Question> _modeQuestions = new List<Question>();
        private ComplexQuestionState _questionState;
        private ComplexAnswerState _answerState;

        private List<Question> _currentQuestions = new List<Question>();
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
        }

        public void InjectAnswerState(ComplexAnswerState answerState)
        {
            _answerState = answerState;
        }

        public void ShowQuestionStateQuestion(Questioner questioner)
        {
            questioner.PlayCoroutine(_currentQuestions, this, _questionState);
        }

        public void HandleOnComplete()
        {
            _controller.SwitchState();
        }

        private bool _haveShownClips = false;
        public void ShowAnswerStateQuestion(Questioner questioner, Action onComplete)
        {
            _controller.SetMainSpanNeeded(true);
            if (!_haveShownClips)
            {
                ConfigureQuestions(false);
                _answerState.TriggerHintHelper("Sırasıyla hangi hayvanları duymuştun?", () =>
                {
                    _haveShownClips = true;
                    onComplete?.Invoke();
                });
            }
            else
            {
                ConfigureQuestions(true);
                _answerState.TriggerHintHelper("Sırasıyla hangi sayıları görmüştün?", () =>
                {
                    _haveShownClips = true;
                    onComplete?.Invoke();
                });
            }
        }

        private void ConfigureQuestions(bool ruleToggle)
        {
            foreach (var element in _currentQuestions)
            {
                element.IsAnswerStringMUST = ruleToggle;
            }
        }

        public int GetCircleCount()
        {
            return _currentQuestions.Count;
        }

        private int tempIndex = 0;
        public List<Question> GetCorrectQuestions(int iterations)
        {
            _currentQuestions.Clear();
            _currentQuestions = GetRandomQuestions(iterations);
            /*switch (tempIndex)
            {
                case 0:
                    var index = Random.Range(0, _modeQuestions.Count);
                    var question = _modeQuestions[index];
                    _currentQuestions.Add(question);
                    _currentQuestions.Add(question);
                    _currentQuestions.Add(question);
                    tempIndex++;
                    break;
                case 1:
                    index = Random.Range(0, _modeQuestions.Count);
                    question = _modeQuestions[index];
                    var next = _modeQuestions[^1];
                    _currentQuestions.Add(question);
                    _currentQuestions.Add(question);
                    _currentQuestions.Add(next);
                    tempIndex++;
                    break;
                default:
                    _currentQuestions.AddRange(GetRandomQuestions(iterations));
                    break;
            }*/

            return _currentQuestions;
        }

        private List<Question> GetRandomQuestions(int count)
        {
            List<Question> selected = new List<Question>();
            for (int i = 0; i < count; i++)
            {
                var index = Random.Range(0, _modeQuestions.Count);
                var question = _modeQuestions[index];
                while (selected.Contains(question))
                {
                    index = Random.Range(0, _modeQuestions.Count);
                    question = _modeQuestions[index];
                }

                selected.Add(question);
            }
            
            return selected;
        }
        
        public List<Question> GetModeChoices()
        {
            List<Question> choices = new List<Question>(_currentQuestions);
            for (int i = 0; i < _currentQuestions.Count; i++)
            {
                var index = Random.Range(0, _modeQuestions.Count);
                var question = _modeQuestions[index];
                while (choices.Contains(question))
                {
                    index = Random.Range(0, _modeQuestions.Count);
                    question = _modeQuestions[index];
                }

                choices.Add(question);
            }

            return choices;
        }

        public void AppendChoice(CommonFields.ButtonType type)
        {
            Debug.Log("Not related with this mode. Exist for the sake of strategy pattern");
        }

        public bool CheckAnswer(List<Question> given)
        {
            if (given.Count != _currentQuestions.Count)
            {
                return false;
            }
            
            if (_haveShownClips)
            {
                for (int i = 0; i < given.Count; i++)
                {
                    if (!_currentQuestions[i].IsEqual(given[i]))
                    {
                        return false;
                    }
                }

                return true;
            }
            else
            {
                for (int i = 0; i < given.Count; i++)
                {
                    if (_currentQuestions[i].GetQuestionItemByType(CommonFields.ButtonType.Count) != given[i].GetQuestionItemByType(CommonFields.ButtonType.Count))
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
