using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;
using Utilities;

namespace Spans.NBack
{
    public class NBack : SpanController
    {
        private Stack<Question> _questionStack;
        private CommonFields.ButtonType _identicalShown;
        private INBackStrategy _currentStrategy;

        protected override void Start()
        {
            base.Start();
            _currentStrategy = new IsIdenticalMode(this);
            _questionStack = new Stack<Question>();
        }
        
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
            return _currentStrategy.GetQuestionByCount(GetAllAvailableSpanObjects().ToList(), count);
        }

        public override bool IsAnswerCorrect()
        {
            var isCorrect = _currentStrategy.CheckAnswer();
            if (isCorrect)
            {
                IncrementSuccessStreak();
            }
            else
            {
                IncrementFailStreak();
            }
            
            return isCorrect;
        }

        public Stack<Question> GetCurrentStack()
        {
            return _questionStack; 
        }

        public void UpdateCurrentStack(Stack<Question> updated)
        {
            _questionStack = updated;
        }

        public INBackStrategy GetStrategyClass()
        {
            return _currentStrategy;
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
