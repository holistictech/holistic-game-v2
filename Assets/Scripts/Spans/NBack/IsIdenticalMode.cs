using System.Collections.Generic;
using ETFXPEL;
using Interfaces;
using Scriptables.QuestionSystem;
using UnityEngine;
using UnityEngine.UI;
using Utilities;
using static Utilities.CommonFields;

namespace Spans.NBack
{
    public class IsIdenticalMode : INBackStrategy
    {
        private int _maxButtonIndex = (int)ButtonType.NotIdentical;
        private NBack _controller;
        private ButtonType _identicalShown;
        private ButtonType _chosen;

        public void InjectController(NBack controller)
        {
            _controller = controller;
        }

        public void SetChosenButtonType(ButtonType chosen)
        {
            _chosen = chosen;
        }

        public bool CheckAnswer()
        {
            return _identicalShown == _chosen;
        }

        public List<Question> GetQuestionByCount(List<Question> questions, int count)
        {
            List<Question> roundQuestions = new List<Question>();
            var questionStack = _controller.GetCurrentStack();

            if (count == 1)
            {
                if (ShouldBeSame())
                {
                    var lastQuestion = questionStack.Peek();
                    roundQuestions.Add(lastQuestion);
                    _identicalShown = ButtonType.Identical;
                }
                else
                {
                    int randomIndex = Random.Range(0, questions.Count);
                    var randomQuestion = questions[randomIndex];
                    roundQuestions.Add(randomQuestion);
                    _identicalShown = ButtonType.NotIdentical;
                }
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
                    _identicalShown = ButtonType.NotIdentical;

                    if (count == 2 && ShouldBeSame())
                    {
                        roundQuestions.Add(randomQuestion);
                        _identicalShown = ButtonType.Identical;
                        break;
                    }
                }
            }

            if (questionStack.Count == 0)
            {
                questionStack = new Stack<Question>(roundQuestions);
            }
            else
            {
                questionStack = StackUtils.AppendStacks(questionStack, new Stack<Question>(roundQuestions));
            }

            _controller.UpdateCurrentStack(questionStack);
            
            return roundQuestions;
        }

        public int GetModeIndex()
        {
            return _maxButtonIndex;
        }

        private bool ShouldBeSame()
        {
            return Random.Range(0, 2) == 0;
        }
    }
}
