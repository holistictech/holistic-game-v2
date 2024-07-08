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
        
        private Tuple<List<Question>, List<Question>> GetModeQuestions()
        {
            return new Tuple<List<Question>, List<Question>>(clipQuestions, numberQuestions);
        }
    }
}
