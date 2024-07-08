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
                corrects.Add(GetRandomQuestion(_clipQuestions, _correctClipQuestions));
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
                    _isAnsweringClip = false;
                    _answerStateToggle = true;
                }
            }
            
            return choices;
        }

        public bool CheckAnswer(List<Question> given)
        {
            List<Question> displayed = _controller.GetCurrentDisplayedQuestions();
            /*if (_isInitial)
            {
                displayed = _correctClipQuestions;
                _isInitial = false;
                _roundCounter = 0;
            }
            else
            {
                if (_roundCounter < _iterations)
                {
                    displayed = _numberSpans[_roundCounter];
                    _roundCounter++;
                }
            }*/

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
            var question = reference[Random.Range(0, reference.Count)];
            while (corrects.Contains(question))
            {
                question = reference[Random.Range(0, reference.Count)];
            }
            corrects.Add(question);
            return question;
        }

        private List<Question> GetNumberQuestions()
        {
            List<Question> numbers = new List<Question>();
            for (int i = 0; i < 2; i++)
            {
                var question = _numberQuestions[Random.Range(0, _numberQuestions.Count)];
                while (_correctClipQuestions.Contains(question))
                {
                    question = _numberQuestions[Random.Range(0, _numberQuestions.Count)];
                }
                if(_numberSpans.ContainsKey(_iterationCount))
                {
                    _numberSpans[_iterationCount].Add(question);
                }
                else
                {
                    _numberSpans.Add(_iterationCount, new List<Question>{question});
                }
                
                numbers.Add(question);
            }
            _iterationCount++;
            return numbers;
        }
    }
}
