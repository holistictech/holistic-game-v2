using System;
using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton;
using UnityEngine;
using Utilities.Helpers;

namespace Spans.ComplexSpan
{
    public class ComplexSpan : SpanController
    {
        [SerializeField] private List<Question> numberQuestions;
        [SerializeField] private List<Question> clipQuestions;

        private CommonFields.ComplexModes _currentModeEnum;
        private IComplexSpanStrategy _currentMode;
        private bool _isMainSpanNeeded;
        private bool _isRecursiveAnswerStateNeeded;

        private Dictionary<CommonFields.ComplexModes, Tuple<List<Question>, List<Question>>> _modeQuestions;
        protected override void Start()
        {
            _modeQuestions = new Dictionary<CommonFields.ComplexModes, Tuple<List<Question>, List<Question>>>()
            {
                {
                    CommonFields.ComplexModes.SoundAndNumberChooser,
                    new Tuple<List<Question>, List<Question>>(clipQuestions, numberQuestions)
                },
                { CommonFields.ComplexModes.PerceptionRecognition, new Tuple<List<Question>, List<Question>>(spanQuestions.ToList(), clipQuestions) },
                { CommonFields.ComplexModes.BlockSpanAndNumbers, new Tuple<List<Question>, List<Question>>(numberQuestions, new List<Question>()) },
            };
            
            _currentMode = new BlockAndNumberSpanStrategy();
            _currentModeEnum = CommonFields.ComplexModes.BlockSpanAndNumbers;
            _currentMode.InjectController(this);
            var modeQuestions = GetModeQuestions();
            _currentMode.InjectModeQuestions(modeQuestions.Item1, modeQuestions.Item2);
            base.Start();
            currentRoundIndex = _currentMode.GetStartingRoundIndex();
        }
        
        public override List<Question> GetSpanObjects()
        {
            return _currentMode.GetCorrectQuestions(currentRoundIndex);
        }
        
        public override List<Question> GetChoices()
        {
            return _currentMode.GetModeChoices();
        }
        
        public override float GetRoundTime()
        {
            return currentRoundIndex * 3 + 2;
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
            else if (_isMainSpanNeeded || _isRecursiveAnswerStateNeeded)
            {
                stateContext.Transition(stateList[2]);
            }
            else
            {
                stateContext.Transition(stateList[1]);
            }
        }
        
        public override bool IsAnswerCorrect()
        {
            var result = _currentMode.CheckAnswer(currentGivenAnswers);
            if (_isMainSpanNeeded)
            {
                if (result)
                {
                    IncrementSuccessStreak();
                }
                else
                {
                    IncrementFailStreak();
                }
                
                SetMainSpanNeeded(false);
            }

            return result;
        }

        public override int GetStartingUnitIndex()
        {
            return _currentMode.GetUnitIndex();
        }

        public void SetMainSpanNeeded(bool toggle)
        {
            _isMainSpanNeeded = toggle;
        }

        public bool GetIsMainSpanNeeded()
        {
            return _isMainSpanNeeded;
        }

        public void SetRecursion(bool toggle)
        {
            _isRecursiveAnswerStateNeeded = toggle;
        }

        public bool GetRecursion()
        {
            return _isRecursiveAnswerStateNeeded;
        }

        public IComplexSpanStrategy GetCurrentStrategy()
        {
            return _currentMode;
        }
        
        private Tuple<List<Question>, List<Question>> GetModeQuestions()
        {
            return _modeQuestions[_currentModeEnum];
        }
    }
}
