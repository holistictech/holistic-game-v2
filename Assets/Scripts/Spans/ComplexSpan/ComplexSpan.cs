using System;
using System.Collections.Generic;
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

        protected override void Start()
        {
            _currentMode = new ComplexSoundChooserMode();
            _currentModeEnum = CommonFields.ComplexModes.SoundAndNumberChooser;
            _currentMode.InjectController(this);
            var modeQuestions = GetModeQuestions();
            _currentMode.InjectModeQuestions(modeQuestions.Item1, modeQuestions.Item2);
            base.Start();
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
            else
            {
                //stateContext.Transition(_currentMode.IsAnsweringMainQuestions() ? stateList[2] : stateList[1]); // to turn back to question state.
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

        public void SetMainSpanNeeded(bool toggle)
        {
            _isMainSpanNeeded = toggle;
        }

        public bool GetIsMainSpanNeeded()
        {
            return _isMainSpanNeeded;
        }

        public IComplexSpanStrategy GetCurrentStrategy()
        {
            return _currentMode;
        }
        
        private Tuple<List<Question>, List<Question>> GetModeQuestions()
        {
            return new Tuple<List<Question>, List<Question>>(clipQuestions, numberQuestions);
        }
    }
}
