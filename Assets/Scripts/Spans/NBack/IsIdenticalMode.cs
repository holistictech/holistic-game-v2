using System.Collections.Generic;
using System.Linq;
using ETFXPEL;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton.QuestionStates;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Utilities.Helpers.CommonFields;

namespace Spans.NBack
{
    public class IsIdenticalMode : INBackStrategy
    {
        private int[] _buttonIndexes = new int[]{0, 1};
        private NBack _controller;
        private ButtonType _correctType;
        private List<ButtonType> _chosenTypes = new List<ButtonType>();
        private const NBackModes GameMode = NBackModes.IsIdentical;

        public IsIdenticalMode(NBack controller)
        {
            _controller = controller;
        }

        public void InjectQuestionState(NBackQuestionState questionState)
        {
            throw new System.NotImplementedException();
        }

        public void ShowQuestion()
        {
            throw new System.NotImplementedException();
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
            _correctType = ButtonType.Null;
            _chosenTypes.Clear();
            List<Question> roundQuestions = new List<Question>();
            var questionStack = _controller.GetCurrentStack();

            if (count == 1)
            {
                if (ShouldBeSame())
                {
                    var lastQuestion = questionStack.Peek();
                    roundQuestions.Add(lastQuestion);
                    _correctType = ButtonType.Identical;
                }
                else
                {
                    int randomIndex = Random.Range(0, questions.Count);
                    var randomQuestion = questions[randomIndex];
                    roundQuestions.Add(randomQuestion);
                    _correctType = ButtonType.NotIdentical;
                }
                
                questionStack = QueueUtils.AppendQueue(questionStack, new Queue<Question>(roundQuestions));
            }
            else
            {
                if (ShouldBeSame())
                {
                    int randomIndex = Random.Range(0, questions.Count);
                    var randomQuestion = questions[randomIndex];
                    roundQuestions.Add(randomQuestion);
                    roundQuestions.Add(randomQuestion);
                    _correctType = ButtonType.Identical;
                }
                else
                {
                    HashSet<int> selectedIndices = new HashSet<int>();

                    for (int i = 0; i < count; i++)
                    {
                        int randomIndex;

                        do
                        {
                            randomIndex = Random.Range(0, questions.Count);
                        } while (selectedIndices.Contains(randomIndex));

                        selectedIndices.Add(randomIndex);
                        var randomQuestion = questions[randomIndex];
                        roundQuestions.Add(randomQuestion);
                        _correctType = ButtonType.NotIdentical;
                    }
                }
                questionStack = new Queue<Question>(roundQuestions);
            }

            _controller.UpdateCurrentStack(questionStack);
            
            return questionStack.ToList();
        }

        public void SetCorrectType()
        {
            throw new System.NotImplementedException();
        }

        public bool IsSwitchable()
        {
            return true;
        }

        public int[] GetModeIndexes()
        {
            return _buttonIndexes;
        }

        public NBackModes GetModeEnum()
        {
            return GameMode;
        }

        private bool ShouldBeSame()
        {
            return Random.Range(0, 2) == 0;
        }
    }
}
