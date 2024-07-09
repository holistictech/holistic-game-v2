using System.Collections.Generic;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton.QuestionStates;
using UnityEngine;

namespace Spans.ComplexSpan
{
    public class ComplexSoundChooserMode : IComplexSpanStrategy
    {
        private ComplexSpan _controller;
        private List<Question> _clipQuestions;
        private List<Question> _numberQuestions;

        private List<Question> _correctClipQuestions = new List<Question>();
        private List<Question> _correctNumberQuestions = new List<Question>();
        private Dictionary<int, List<Question>> _numberSpans = new Dictionary<int, List<Question>>();
        
        private int _iterations = 0;
        private int _iterationCount = 0;
        private bool _isAnsweringClip;
        
        public void InjectController(ComplexSpan controller)
        {
            _controller = controller;
        }

        public void InjectModeQuestions(List<Question> mainQuestions, List<Question> helperQuestions)
        {
            _clipQuestions = mainQuestions;
            _numberQuestions = helperQuestions;
        }

        public void EnableRequiredModeElements(ComplexQuestionState questionState)
        {
            questionState.GetQuestionField().gameObject.SetActive(true);
        }

        public int GetFixedQuestionCount()
        {
            return 3;
        }

        public List<Question> GetCorrectQuestions(int iterations)
        {
            _iterations = iterations;
            List<Question> corrects = new List<Question>();
            for (int i = 0; i < iterations; i++)
            {
                var question = GetRandomQuestion(_clipQuestions, _correctClipQuestions);
                _correctClipQuestions.Add(question);
                corrects.Add(question);
                corrects.AddRange(GetNumberQuestions());
            }
            
            return corrects;
        }

        public bool IsAnsweringMainQuestions()
        {
            return _isAnsweringClip;
        }

        private int _roundCounter = 0;
        private bool _answerStateToggle = true;
        public List<Question> GetModeChoices()
        {
            List<Question> choices = new List<Question>();
            if (_answerStateToggle)
            {
                choices.AddRange(_correctClipQuestions);
                var iterations = _correctClipQuestions.Count;
                for (int i = 0; i < iterations; i++)
                {
                    choices.Add(GetRandomQuestion(_clipQuestions, _correctClipQuestions));
                }

                _answerStateToggle = false;
                _isAnsweringClip = true;
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
                    _answerStateToggle = true;
                    _isAnsweringClip = false;
                }
            }
            
            return choices;
        }

        public bool CheckAnswer(List<Question> given)
        {
            List<Question> displayed = _controller.GetIsMainSpanNeeded() ? _correctClipQuestions : _controller.GetCurrentDisplayedQuestions();

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

        public List<Question> GetCorrectMainQuestions()
        {
            return _correctClipQuestions;
        }
    }
}
