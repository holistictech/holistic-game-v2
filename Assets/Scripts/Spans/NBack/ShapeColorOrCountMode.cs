using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton.QuestionStates;
using UnityEngine;
using Utilities;
using static Utilities.Helpers.CommonFields;

namespace Spans.NBack
{
    public class ShapeColorOrCountMode : INBackStrategy
    {
        private int[] _buttonIndexes = {2, 3, 4};
        private NBack _controller;
        private ButtonType _correctType;
        private ButtonType _chosen;
        private const NBackModes GameMode = NBackModes.ColorShapeOrCount;

        public ShapeColorOrCountMode(NBack controller)
        {
            _controller = controller;
            ResetSpawnAmounts(_controller.GetAlternativeImagesByType(GameMode));
        }

        public void InjectQuestionState(NBackQuestionState questionState)
        {
            throw new System.NotImplementedException();
        }

        public void ShowQuestion()
        {
            throw new System.NotImplementedException();
        }

        public void SetChosenButtonType(ButtonType chosen)
        {
            _chosen = chosen;
        }

        public bool CheckAnswer()
        {
            return _correctType == _chosen;
        }

        public bool IsEmptyRound()
        {
            return false;
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
            var images = _controller.GetAlternativeImagesByType(GameMode);
            var questionStack = _controller.GetCurrentStack();
            Question first = isInitial ? images[Random.Range(0, images.Count)] : questionStack.Peek();
            var spawnAmount = first.SpawnAmount;
            switch (_correctType)
            {
                case ButtonType.Shape:
                    while (IsElementInQueue(questionStack, first))
                    {
                        first = GetRandomQuestion(images);
                    }

                    first.SpawnAmount = spawnAmount;
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
                        alternative.SpawnAmount = first.SpawnAmount;
                        alternative.ItemSprite = (Sprite)first.GetQuestionItemByType(_correctType);
                        alternative.AlternativeColorSprite = (Sprite)first.GetQuestionItem();
                        questionStack.Enqueue(alternative);
                    }
                    break;
                case ButtonType.Count:
                    var temp = first.SpawnAmount;
                    var random = Random.Range(1, 5);
                    while (random == temp)
                    {
                        random = Random.Range(1, 5);
                    }
                    NBackQuestion alternativeCount = ScriptableObject.CreateInstance<NBackQuestion>();
                    alternativeCount.SpawnAmount = random;
                    alternativeCount.ItemSprite = (Sprite)first.GetQuestionItem();
                    alternativeCount.AlternativeColorSprite = (Sprite)first.GetQuestionItemByType(_correctType);
                    questionStack.Enqueue(alternativeCount);
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
        
        public void SetCorrectType()
        {
            _correctType = (ButtonType)Random.Range((int)ButtonType.Shape, (int)ButtonType.Count+1);
        }

        private void ResetSpawnAmounts(List<Question> questions)
        {
            foreach (var element in questions)
            {
                element.SpawnAmount = 1;
            }
        }
        
        public int[] GetModeIndexes()
        {
            return _buttonIndexes;
        }
    }
}
