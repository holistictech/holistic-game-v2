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
        
        [Header("Choose Class (7.4)")]
        [SerializeField] private List<Question> classQuestions;
        [SerializeField] private List<Question> choiceList; //Specific to class chooser mode.

        [Header("Complex Number Chooser (7.6)")]
        [SerializeField] private List<Question> complexClipQuestions;

        private CommonFields.ComplexModes _currentModeEnum;
        private IComplexSpanStrategy _currentMode;
        private bool _isMainSpanNeeded;
        private bool _isRecursiveAnswerStateNeeded;

        [SerializeField] private CommonFields.ComplexModes spanMode;

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
                { CommonFields.ComplexModes.ChooseClass, new Tuple<List<Question>, List<Question>>(classQuestions, choiceList) },
                { CommonFields.ComplexModes.NumberAndAnimalChooser, new Tuple<List<Question>, List<Question>>(complexClipQuestions, new List<Question>()) },
            };

            SetSpanMode();
            //_currentMode = new BlockAndNumberSpanStrategy();
            //_currentModeEnum = CommonFields.ComplexModes.BlockSpanAndNumbers;
            _currentMode.InjectController(this);
            var modeQuestions = GetModeQuestions();
            _currentMode.InjectModeQuestions(modeQuestions.Item1, modeQuestions.Item2);
            base.Start();
            currentRoundIndex = _currentMode.GetStartingRoundIndex();
        }

        private void SetSpanMode()
        {
            switch (spanMode)
            {
                case CommonFields.ComplexModes.SoundAndNumberChooser:
                    _currentMode = new ComplexSoundChooserMode();
                    _currentModeEnum = CommonFields.ComplexModes.SoundAndNumberChooser;
                    break;
                case CommonFields.ComplexModes.PerceptionRecognition:
                    _currentMode = new PerceptionRecognitionStrategy();
                    _currentModeEnum = CommonFields.ComplexModes.PerceptionRecognition;
                    break;
                case CommonFields.ComplexModes.BlockSpanAndNumbers:
                    _currentMode = new BlockAndNumberSpanStrategy();
                    _currentModeEnum = CommonFields.ComplexModes.BlockSpanAndNumbers;
                    break;
                case CommonFields.ComplexModes.ChooseClass:
                    _currentMode = new ChooseClassMode();
                    _currentModeEnum = CommonFields.ComplexModes.ChooseClass;
                    break;
                case CommonFields.ComplexModes.NumberAndAnimalChooser:
                    _currentMode = new ComplexNumberChooserMode();
                    _currentModeEnum = CommonFields.ComplexModes.NumberAndAnimalChooser;
                    break;
                    default:
                        Debug.LogError("No such span mode");
                    break;
            }
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
            else if (GetRecursion())
            {
                SwitchToAnswerState();
            }
            else
            {
                SwitchToQuestionState();
            }
        }

        public void SwitchToQuestionState()
        {
            stateContext.Transition(stateList[1]);
        }

        public void SwitchToAnswerState()
        {
            stateContext.Transition(stateList[2]);
        }

        public override bool IsEmptyRound()
        {
            return !GetIsMainSpanNeeded();
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
