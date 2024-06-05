using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton.QuestionStates;
using UI.Helpers;
using UnityEngine;
using Utilities;
using static Utilities.CommonFields;

namespace Spans.NBack
{
    public class NBackMode : INBackStrategy
    {
        private int[] _buttonIndexes = new int[]{0, 1};
        private ButtonType _correctType;
        private ButtonType _chosen;
        private NBack _controller;
        private NBackQuestionState _questionState;
        private CorsiBlockUIHelper _blockUIHelper;
        private int _currentQuestionIndex = 0;
        private const NBackModes GameMode = NBackModes.NBack;
        
        public NBackMode(NBack controller)
        {
            _controller = controller;
        }
        
        public void InjectQuestionState(NBackQuestionState questionState)
        {
            _questionState = questionState;
        }

        public void ShowQuestion(List<Question> questions)
        {
            GetBlockHelper();
            _currentQuestionIndex = 0;
            _blockUIHelper.AssignQuestions(GetQuestionByCount(new List<Question>(), 9));
        }
        
        private IEnumerator IterateQuestions(List<Question> spanQuestions)
        {
            for (int i = 0; i < spanQuestions.Count; i++)
            {
                if (_currentQuestionIndex >= spanQuestions.Count)
                {
                    break;
                }
                _questionState.EnableCircle(_currentQuestionIndex);
                _blockUIHelper.HighlightTargetBlock(spanQuestions[_currentQuestionIndex]);
                _currentQuestionIndex++;
                yield return new WaitForSeconds(2f);
            }
            
            DOVirtual.DelayedCall(1f, _questionState.SwitchNextState);
        }

        public void SetChosenButtonType(ButtonType chosen)
        {
            _chosen = chosen;
        }

        public bool CheckAnswer()
        {
            return _correctType == _chosen;
        }

        public List<Question> GetQuestionByCount(List<Question> questions, int count)
        {
            questions = _controller.GetAlternativeImagesByType(GameMode);
            List<Question> roundQuestions = new List<Question>();
            var iterations = count;
            for (int i = 0; i < iterations; i++)
            {
                var randomQuestionIndex = Random.Range(0, questions.Count);
                var randomQuestion = questions[randomQuestionIndex];
                while (roundQuestions.Contains(randomQuestion))
                {
                    randomQuestionIndex = Random.Range(0, questions.Count);
                    randomQuestion = questions[randomQuestionIndex];
                }
                
                roundQuestions.Add(randomQuestion);
            }
            
            return roundQuestions;
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
