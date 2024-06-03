using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;

namespace Spans.NBack
{
    public class NBack : SpanController
    {
        private Stack<Question> _questionStack = new Stack<Question>();
        private bool _identicalShown;
        private INBackStrategy _currentStrategy;

        public override List<Question> GetSpanObjects()
        {
            if (_questionStack.Count == 0)
            {
                return GetQuestionByCount(2);
            }
            else
            {
                _questionStack.Pop();
                return GetQuestionByCount(1);
            }
        }

        private List<Question> GetQuestionByCount(int count)
        {
            List<Question> roundQuestions = new List<Question>();
            var allQuestions = GetAllAvailableSpanObjects();

            if (count == 1)
            {
                if (ShouldBeSame() && _questionStack.Count > 0)
                {
                    var lastQuestion = _questionStack.Peek();
                    roundQuestions.Add(lastQuestion);
                    _identicalShown = true;
                }
                else
                {
                    int randomIndex = Random.Range(0, allQuestions.Length);
                    var randomQuestion = allQuestions[randomIndex];
                    roundQuestions.Add(randomQuestion);
                    _identicalShown = false;
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
                        randomIndex = Random.Range(0, allQuestions.Length);
                    } while (selectedIndices.Contains(randomIndex));

                    selectedIndices.Add(randomIndex);
                    var randomQuestion = allQuestions[randomIndex];
                    roundQuestions.Add(randomQuestion);
                    _identicalShown = false;

                    if (count == 2 && ShouldBeSame())
                    {
                        roundQuestions.Add(randomQuestion);
                        _identicalShown = true;
                        break;
                    }
                }
            }

            if (_questionStack.Count == 0)
            {
                _questionStack = new Stack<Question>(roundQuestions);
            }
            else
            {
                _questionStack = StackUtils.AppendStacks(_questionStack, new Stack<Question>(roundQuestions));
            }

            return roundQuestions;
        }

        public override bool IsAnswerCorrect()
        {
            return false;
        }


        private bool ShouldBeSame()
        {
            return Random.Range(0, 2) == 0;
        }
    }

    public abstract class StackUtils
    {
        public static Stack<T> AppendStacks<T>(Stack<T> stack1, Stack<T> stack2)
        {
            Stack<T> tempStack = new Stack<T>();

            // Pop elements from stack2 and push them onto tempStack
            while (stack2.Count > 0)
            {
                tempStack.Push(stack2.Pop());
            }

            // Pop elements from tempStack (which reverses it back to original order)
            // and push them onto stack1
            while (tempStack.Count > 0)
            {
                stack1.Push(tempStack.Pop());
            }

            return stack1;
        }
    }
}
