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
    public class ComplexSoundChooserMode : IComplexSpanStrategy
    {
        private ComplexSpan _controller;
        private List<Question> _clipQuestions;
        private List<Question> _numberQuestions;
        private ComplexQuestionState _questionState;
        private ComplexAnswerState _answerState;
        
        private List<Question> _correctClipQuestions = new List<Question>();
        private List<Question> _correctNumberQuestions = new List<Question>();
        private List<Question> _currentQuestions = new List<Question>();
        private Dictionary<int, List<Question>> _numberSpans = new Dictionary<int, List<Question>>();
        
        private int _iterations = 0;
        private int _iterationCount = 0;
        
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
            _clipQuestions = mainQuestions;
            _numberQuestions = helperQuestions;
        }

        public void InjectQuestionState(ComplexQuestionState questionState)
        {
            _questionState = questionState;
        }

        public void InjectAnswerState(ComplexAnswerState answerState)
        {
            _answerState = answerState;
            answerState.EnableGridField();
        }

        private int _currentQuestionIndex;
        private bool _hasMainPlayed;
        private List<Question> _questionsToBeDisplayed;
        public void ShowQuestionStateQuestion(Questioner questioner)
        {
            if (_currentQuestionIndex >= _currentQuestions.Count)
            {
                if (_controller.GetIsMainSpanNeeded() || _hasMainPlayed)
                {
                    _controller.GetSpanObjects();
                    _currentQuestionIndex = 0;
                    _hasMainPlayed = false;
                }
                else
                {
                    _controller.SetMainSpanNeeded(true);
                    _hasMainPlayed = true;
                    _questionState.SwitchNextState();
                }
            }

            _questionsToBeDisplayed = new List<Question> { };

            if (_playClipToggle)
            {
                _questionsToBeDisplayed.Add(_currentQuestions[_currentQuestionIndex]);
            }
            else
            {
                _questionsToBeDisplayed.AddRange(_numberSpans[_roundCounter]);
            }

            _currentQuestionIndex += _questionsToBeDisplayed.Count;
            questioner.PlayCoroutine(_questionsToBeDisplayed, this);
        }

        public void HandleOnComplete()
        {
            _questionState.SwitchNextState();
        }

        public void ShowAnswerStateQuestion(Questioner questioner, Action onComplete)
        {
            _answerState.SetChoiceUI();
            if(_controller.GetIsMainSpanNeeded())
            {
                _answerState.TriggerHintHelper("Sırasıyla hangi hayvanları duymuştun?", () =>
                {
                    onComplete?.Invoke();
                });
            }
            else
            {
                onComplete?.Invoke();
            }
        }

        public int GetCircleCount()
        {
            return _playClipToggle && _roundCounter < _iterations ? 1 : 2;
        }

        public List<Question> GetCorrectQuestions(int iterations)
        {
            _iterations = iterations;
            ResetLogicFields();
            List<Question> corrects = new List<Question>();
            for (int i = 0; i < iterations; i++)
            {
                var question = GetRandomQuestion(_clipQuestions, _correctClipQuestions);
                _correctClipQuestions.Add(question);
                corrects.Add(question);
                corrects.AddRange(GetNumberQuestions());
            }

            _currentQuestions = corrects;
            return corrects;
        }

        private void ResetLogicFields()
        {
            _correctClipQuestions.Clear();
            _numberSpans.Clear();
            _roundCounter = 0;
            _iterationCount = 0;
            _playClipToggle = true;
        }

        private int _roundCounter = 0;
        private bool _playClipToggle = true;
        public List<Question> GetModeChoices()
        {
            List<Question> choices = new List<Question>();
            if (_playClipToggle)
            {
                choices.AddRange(_correctClipQuestions);
                var iterations = _correctClipQuestions.Count;
                for (int i = 0; i < iterations; i++)
                {
                    choices.Add(GetRandomQuestion(_clipQuestions, _correctClipQuestions));
                }

                _playClipToggle = false;
            }
            else
            {
                if (_roundCounter < _iterations)
                {
                    choices.AddRange(_numberSpans[_roundCounter]);
                    for (int i = 0; i < 2; i++)
                    {
                        var question = GetRandomQuestion(_numberQuestions, _numberSpans[_roundCounter]);
                        choices.Add(question);
                    }
                    _roundCounter++;
                    _playClipToggle = true;
                }
            }
            
            choices.Shuffle();
            return choices;
        }

        public void AppendChoice(CommonFields.ButtonType type)
        {
            throw new System.NotImplementedException();
        }

        public bool CheckAnswer(List<Question> given)
        {
            List<Question> displayed = _controller.GetIsMainSpanNeeded() ? _correctClipQuestions : _questionsToBeDisplayed;

            if (displayed.Count != given.Count) return false;
            for (int i = 0; i < displayed.Count; i++)
            {
                var correct = displayed[i];
                var answer = given[i];
                if (!correct.IsEqual(answer))
                    return false;
            }
            
            return true;
        }

        private Question GetRandomQuestion(List<Question> reference, List<Question> corrects)
        {
            int maxAttempts = reference.Count * 2;
            int attempts = 0;

            Question question = reference[Random.Range(0, reference.Count)];
            while (corrects.Contains(question) && attempts < maxAttempts)
            {
                question = reference[Random.Range(0, reference.Count)];
                attempts++;
            }

            if (attempts >= maxAttempts)
            {
                Debug.LogWarning("GetRandomQuestion: Maximum attempts reached, returning last picked question.");
            }

            return question;
        }

        private List<Question> GetNumberQuestions()
        {
            List<Question> numbers = new List<Question>();
            HashSet<Question> addedQuestions = new HashSet<Question>();
            int maxAttempts = _numberQuestions.Count * 2;

            for (int i = 0; i < 2; i++)
            {
                int attempts = 0;
                Question question = _numberQuestions[Random.Range(0, _numberQuestions.Count)];
                while ((_numberSpans.ContainsKey(_iterationCount) && _numberSpans[_iterationCount].Contains(question) || addedQuestions.Contains(question)) && attempts < maxAttempts)
                {
                    question = _numberQuestions[Random.Range(0, _numberQuestions.Count)];
                    attempts++;
                }

                if (attempts >= maxAttempts)
                {
                    Debug.LogWarning("GetNumberQuestions: Maximum attempts reached, adding last picked question.");
                }

                if (_numberSpans.ContainsKey(_iterationCount))
                {
                    _numberSpans[_iterationCount].Add(question);
                }
                else
                {
                    _numberSpans.Add(_iterationCount, new List<Question> { question });
                }

                addedQuestions.Add(question);
                numbers.Add(question);
            }

            _iterationCount++;
            return numbers;
        }

        public int GetUnitIndex()
        {
            return 0;
        }
    }
}
