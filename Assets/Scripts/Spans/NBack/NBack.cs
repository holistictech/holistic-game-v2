using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;
using Utilities;
using Utilities.Helpers;

namespace Spans.NBack
{
    public class NBack : SpanController
    {
        [SerializeField] private List<Question> alternativeObjectImages;
        [SerializeField] private List<Question> nBackQuestions;
        [SerializeField] private List<Question> dualNBackQuestions;
        [SerializeField] private CommonFields.NBackModes _testMode; 
            
        private Dictionary<CommonFields.NBackModes, List<Question>> _modeQuestionDictionary;
        private Queue<Question> _questionStack;
        private CommonFields.ButtonType _identicalShown;
        private INBackStrategy _currentStrategy;
        private bool _isInitial;
        private bool _isCorrect;

        protected override void Start()
        {
            base.Start();
            _isInitial = true;
            _questionStack = new Queue<Question>();
            _modeQuestionDictionary =
                new Dictionary<CommonFields.NBackModes, List<Question>>()
                {
                    { CommonFields.NBackModes.IsIdentical, GetAllAvailableSpanObjects().ToList() },
                    { CommonFields.NBackModes.ColorOrShape, alternativeObjectImages },
                    { CommonFields.NBackModes.ColorShapeOrCount, alternativeObjectImages },
                    { CommonFields.NBackModes.NBack, nBackQuestions },
                    { CommonFields.NBackModes.DualNBack, dualNBackQuestions },
                };

            switch (_testMode)
            {
                case CommonFields.NBackModes.IsIdentical:
                    _currentStrategy = new IsIdenticalMode(this);
                    break;
                case CommonFields.NBackModes.ColorOrShape:
                    _currentStrategy = new ShapeOrColorMode(this);
                    break;
                case CommonFields.NBackModes.ColorShapeOrCount:
                    _currentStrategy = new ShapeColorOrCountMode(this);
                    break;
                case CommonFields.NBackModes.NBack:
                    _currentStrategy = new NBackMode(this);
                    break;
                case CommonFields.NBackModes.DualNBack:
                    _currentStrategy = new DualNBackMode(this);
                    break;
            }
        }
        
        protected override void StartTimer()
        {
            timerHelper.InjectSpanController(this, 150);
        }

        public override List<Question> GetSpanObjects()
        {
            if (_questionStack.Count == 0)
            {
                return GetQuestionByCount(2);
            }
            else
            {
                _questionStack.Dequeue();
                return GetQuestionByCount(1);
            }
        }

        private List<Question> GetQuestionByCount(int count)
        {
            return _currentStrategy.GetQuestionByCount(GetAllAvailableSpanObjects().ToList(), count);
        }

        public override int GetRoundTime()
        {
            return 3;
        }

        private void TryUpdateStrategy()
        {
            //@todo: update mechanism
            
        }
        
        public override void SwitchState()
        {
            if (isSpanFinished)
            {
                Debug.Log("this is finished");
                stateContext.Transition(stateList[^1]);
                return;
            }
            
            var index = stateList.IndexOf(stateContext.CurrentState);
            if (index < stateList.Count - 2)
            {
                ISpanState nextState = stateList[index+1];
                stateContext.Transition(nextState);
            }
            else
            {
                stateContext.Transition(stateList[1]); // to turn back to question state for NBack scenarios.
            }
        }

        public override bool IsAnswerCorrect()
        {
            _isCorrect = _currentStrategy.CheckAnswer();
            if (_isCorrect)
            {
                IncrementSuccessStreak();
            }
            else
            {
                IncrementFailStreak();
            }

            return _isCorrect;
        }

        public bool IsEmptyRound()
        {
            return _currentStrategy.IsEmptyRound();
        }

        public override int GetRoundIndex()
        {
            if (_isInitial)
            {
                _isInitial = false;
                return 2;
            }
            else
            {
                return 1;
            }
        }

        public List<Question> GetAlternativeImagesByType(CommonFields.NBackModes mode)
        {
            var list = _modeQuestionDictionary[mode];
            return list ?? GetAllAvailableSpanObjects().ToList();
        }

        public Queue<Question> GetCurrentStack()
        {
            return _questionStack; 
        }

        public void UpdateCurrentStack(Queue<Question> updated)
        {
            _questionStack = updated;
        }

        public INBackStrategy GetStrategyClass()
        {
            return _currentStrategy;
        }

        public bool GetCorrectStatus()
        {
            return _isCorrect;
        }
    }

    public abstract class QueueUtils
    {
        public static Queue<T> AppendQueue<T>(Queue<T> stack1, Queue<T> stack2)
        {
            Queue<T> tempStack = new Queue<T>();

            // Pop elements from stack2 and push them onto tempStack
            while (stack2.Count > 0)
            {
                tempStack.Enqueue(stack2.Dequeue());
            }

            // Pop elements from tempStack (which reverses it back to original order)
            // and push them onto stack1
            while (tempStack.Count > 0)
            {
                stack1.Enqueue(tempStack.Dequeue());
            }

            return stack1;
        }
    }
}
