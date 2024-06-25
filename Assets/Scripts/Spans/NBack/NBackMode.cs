using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton.QuestionStates;
using UI.Helpers;
using UnityEngine;
using Utilities;
using static Utilities.Helpers.CommonFields;

namespace Spans.NBack
{
    public class NBackMode : INBackStrategy
    {
        private int[] _buttonIndexes = new int[]{0, 1};
        private ButtonType _correctType;
        private List<ButtonType> _chosenTypes = new List<ButtonType>();
        private NBack _controller;
        private NBackQuestionState _questionState;
        private CorsiBlockUIHelper _blockUIHelper;
        private const NBackModes GameMode = NBackModes.NBack;
        private int _roundCount = 0;
        
        public NBackMode(NBack controller)
        {
            _controller = controller;
            _roundCount = 0;
        }
        
        public void InjectQuestionState(NBackQuestionState questionState)
        {
            _questionState = questionState;
        }

        public void ShowQuestion()
        {
            GetBlockHelper();
            _blockUIHelper.GetCorsiBlocks();
            _blockUIHelper.AssignQuestions(GetModeQuestions());
        }

        public void AppendChosenButtonType(ButtonType chosen)
        {
            if (!_chosenTypes.Contains(chosen))
            {
                _chosenTypes.Add(chosen);
            }
        }

        public bool CheckAnswer()
        {
            return _chosenTypes.Count == 1 && _chosenTypes.Contains(_correctType);
        }

        public bool IsEmptyRound()
        {
            return false;
        }

        public List<Question> GetQuestionByCount(List<Question> questions, int count)
        {
            if (_roundCount == 19)
            {
                _controller.EndSpan();
                return new List<Question>();
            }
            
            _roundCount++;
            _chosenTypes.Clear();
            SetCorrectType();
            var questionStack = _controller.GetCurrentStack();
            questions = _controller.GetAlternativeImagesByType(GameMode);
            List<Question> roundQuestions = new List<Question>();
            if (count == 1)
            {
                var first = questionStack.Peek();
                switch (_correctType)
                {
                    case ButtonType.Identical:
                        roundQuestions.Add(first);
                        break;
                    case ButtonType.NotIdentical:
                        Question randomQuestion = first;
                        while (questionStack.Contains(randomQuestion))
                        {
                            int randomIndex = Random.Range(0, questions.Count); 
                            randomQuestion = questions[randomIndex];
                        }
                        roundQuestions.Add(randomQuestion);
                        break;
                }

                questionStack = QueueUtils.AppendQueue(questionStack, new Queue<Question>(roundQuestions));
            }
            else
            {
                switch (_correctType)
                {
                    case ButtonType.Identical:
                        int randomIndex = Random.Range(0, questions.Count);
                        var randomQuestion = questions[randomIndex];
                        roundQuestions.Add(randomQuestion);
                        roundQuestions.Add(randomQuestion);
                        break;
                    case ButtonType.NotIdentical:
                        HashSet<int> selectedIndices = new HashSet<int>();

                        for (int i = 0; i < count; i++)
                        {
                            do
                            {
                                randomIndex = Random.Range(0, questions.Count);
                            } while (selectedIndices.Contains(randomIndex));

                            selectedIndices.Add(randomIndex);
                            randomQuestion = questions[randomIndex];
                            roundQuestions.Add(randomQuestion); 
                        } 
                        break;
                }
                questionStack = new Queue<Question>(roundQuestions);
            }
            
            _controller.UpdateCurrentStack(questionStack);
            return roundQuestions;
        }

        public void SetCorrectType()
        {
            _correctType = (ButtonType)Random.Range((int)ButtonType.Identical, (int)ButtonType.NotIdentical+1);
        }

        public bool IsSwitchable()
        {
            return true;
        }

        private List<Question> GetModeQuestions()
        {
            return _controller.GetAlternativeImagesByType(GameMode);
        }

        private void GetBlockHelper()
        {
            _blockUIHelper = _questionState.GetBlockHelper();
        }

        public int[] GetModeIndexes()
        {
            return _buttonIndexes;
        }
    }
}
