using System.Collections.Generic;
using System.Linq;
using ETFXPEL;
using Interfaces;
using Scriptables.QuestionSystem;
using UnityEngine;
using Utilities;
using static Utilities.CommonFields;

namespace Spans.NBack
{
    public class ShapeOrColorMode : INBackStrategy
    {
        private int[] _buttonIndexes;
        private NBack _controller;
        private ButtonType _correctType;
        private ButtonType _chosen;
        
        public ShapeOrColorMode(NBack controller)
        {
            _controller = controller;
        }
        public void SetChosenButtonType(ButtonType chosen)
        {
            _chosen = chosen;
        }

        public bool CheckAnswer()
        {
            return _correctType == _chosen;
        }

        private bool _isInitial;
        public List<Question> GetQuestionByCount(List<Question> questions, int count)
        {
            _chosen = ButtonType.Null;
            SetCorrectType();
            List<Question> round = new List<Question>();
            if (count == 1)
            {
                round = GetAlternativeQuestionByType(false);
            }
            else
            {
                _isInitial = count == 2;
                for (int i = 0; i < count; i++)
                {
                    round = GetAlternativeQuestionByType(_isInitial);
                    _isInitial = false;
                }
            }
            return round;
        }

        private List<Question> GetAlternativeQuestionByType(bool isInitial)
        {
            var images = _controller.GetAlternativeImages();
            var questionStack = _controller.GetCurrentStack();
            Question first = isInitial ? images[Random.Range(0, images.Count)] : questionStack.Peek();
            switch (_correctType)
            {
                case ButtonType.Shape:
                    while (IsElementInQueue(questionStack, first))
                    {
                        first = GetRandomQuestion(images);
                    }
                    questionStack.Enqueue(first);
                    break;
                case ButtonType.Color:
                    if (isInitial)
                    {
                        questionStack.Enqueue(first);
                    }
                    else
                    {
                        NBackQuestion alternative = ScriptableObject.CreateInstance<NBackQuestion>();
                        alternative.ItemSprite = (Sprite)first.GetQuestionItemByType(_correctType);
                        questionStack.Enqueue(alternative);
                    }
                    break;
            }
            _controller.UpdateCurrentStack(questionStack);
            return questionStack.ToList();
        }

        private Question GetRandomQuestion(List<Question> questions)
        {
            return questions[Random.Range(0, questions.Count)];
        }

        private bool IsElementInQueue(Queue<Question> queue, Question element)
        {
            return queue.Contains(element);
        }

        private void SetCorrectType()
        {
            _correctType = (ButtonType)Random.Range((int)ButtonType.Shape, (int)ButtonType.Color+1);
        }

        public int[] GetModeIndexes()
        {
            return _buttonIndexes;
        }
    }
}
