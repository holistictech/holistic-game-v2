using System.Collections.Generic;
using Interfaces;
using Scriptables.QuestionSystem;
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

        private bool _answeringHelper = true;
        
        public void InjectController(ComplexSpan controller)
        {
            _controller = controller;
        }

        public void InjectModeQuestions(List<Question> mainQuestions, List<Question> helperQuestions)
        {
            _clipQuestions = mainQuestions;
            _numberQuestions = helperQuestions;
        }

        public List<Question> GetCorrectQuestions(int iterations)
        {
            List<Question> corrects = new List<Question>();
            for (int i = 0; i < iterations; i++)
            {
                corrects.Add(GetRandomClipQuestion());
                corrects.AddRange(GetNumberQuestions());
            }
            
            return corrects;
        }

        public List<Question> GetModeChoices()
        {
            List<Question> choices = new List<Question>();
            if (_answeringHelper)
            {
                choices.AddRange(_correctNumberQuestions);
            }
            else
            {
                choices.AddRange(_correctClipQuestions);
                var iterations = _correctClipQuestions.Count;
                for (int i = 0; i < iterations; i++)
                {
                    choices.Add(GetRandomClipQuestion());
                }
            }
            
            return choices;
        }

        private Question GetRandomClipQuestion()
        {
            var question = _clipQuestions[Random.Range(0, _clipQuestions.Count)];
            while (_correctClipQuestions.Contains(question))
            {
                question = _clipQuestions[Random.Range(0, _clipQuestions.Count)];
            }
            _correctClipQuestions.Add(question);
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
                _correctNumberQuestions.Add(question);
                numbers.Add(question);
            }
            
            return numbers;
        }
    }
}
