using System.Collections.Generic;
using System.Linq;
using Interfaces;
using Scriptables.QuestionSystem;
using Spans.Skeleton.QuestionStates;
using UI.Helpers;
using UnityEngine;
using UnityEngine.Rendering;
using Utilities.Helpers;
using static Utilities.Helpers.CommonFields;

namespace Spans.NBack
{
    public class DualNBackMode : INBackStrategy
    {
        private readonly int[] _buttonIndexes = new int[] { 5, 6 };
        private ButtonType _correctType;
        private List<ButtonType> _chosenTypes = new List<ButtonType>();
        private NBack _controller;
        private NBackQuestionState _questionState;
        private CorsiBlockUIHelper _blockUIHelper;
        private const NBackModes GameMode = NBackModes.DualNBack;
        private AudioClip _lastPlayedClip;
        private int _roundCount;
        public DualNBackMode(NBack controller)
        {
            _controller = controller;
            _roundCount = 0;
        }

        public void InjectQuestionState(NBackQuestionState questionState)
        {
            _questionState = questionState;
        }

        public void ShowQuestion()
        {
            GetBlockHelper();
            _blockUIHelper.GetCorsiBlocks();
            _blockUIHelper.AssignQuestions(GetModeQuestions());
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
            if (_correctType == ButtonType.SoundAndPosition)
            {
                return _chosenTypes.Contains(ButtonType.Sound) && _chosenTypes.Contains(ButtonType.Position);
            }
            
            return _chosenTypes.Count == 1 && _chosenTypes[0] == _correctType;
        }

        public bool IsEmptyRound()
        {
            return _correctType == ButtonType.Null;
        }

        public List<Question> GetQuestionByCount(List<Question> questions, int count)
        {
            if (_roundCount == 19)
            {
                _controller.EndSpan();
                return new List<Question>();
            }
            
            
            SetCorrectType();
            _chosenTypes.Clear();
            _roundCount++;
            var questionStack = _controller.GetCurrentStack();
            var alternativeQuestions = GetModeQuestions();
            List<Question> roundQuestions = new List<Question>();

            var firstQuestion = GetFirstQuestion(count, questionStack, alternativeQuestions);

            if (count == 1)
            {
                roundQuestions = GetRoundQuestionsForSingleCount(firstQuestion, alternativeQuestions);
                questionStack = QueueUtils.AppendQueue(questionStack, new Queue<Question>(roundQuestions));
            }
            else if (count == 2)
            {
                roundQuestions = GetRoundQuestionsForDoubleCount(firstQuestion, alternativeQuestions);
                questionStack = new Queue<Question>(roundQuestions);
            }

            _controller.UpdateCurrentStack(questionStack);
            
            Debug.Log($"Returned round question count {roundQuestions.Count}");
            return questionStack.ToList();
        }

        private Question GetFirstQuestion(int count, Queue<Question> questionStack, List<Question> alternativeQuestions)
        {
            if(questionStack.Count == 0 ||count == 2)
            {
                return alternativeQuestions[Random.Range(0, alternativeQuestions.Count)];
            }

            return questionStack.Peek();
        }

        private List<Question> GetRoundQuestionsForSingleCount(Question first, List<Question> alternativeQuestions)
        {
            List<Question> roundQuestions = new List<Question>();
            _lastPlayedClip = (AudioClip)first.GetQuestionItemByType(ButtonType.Sound);
            Debug.Log($"Chosen type {_correctType.ToString()}");
            switch (_correctType)
            {
                case ButtonType.Position:
                    var alternativeSound = ModifyQuestionClip(first);
                    roundQuestions.Add(alternativeSound);
                    break;
                case ButtonType.Sound:
                    var alternativePosition = CreateAlternativePositionQuestion(first, alternativeQuestions);
                    roundQuestions.Add(alternativePosition);
                    break;
                case ButtonType.SoundAndPosition:
                    roundQuestions.Add(first);
                    break;
                case ButtonType.Null:
                    var question = GetCompletelyRandomQuestion(first);
                    roundQuestions.Add(question);
                    break;
            }

            return roundQuestions;
        }

        private List<Question> GetRoundQuestionsForDoubleCount(Question first, List<Question> alternativeQuestions)
        {
            List<Question> roundQuestions = new List<Question>();
            roundQuestions.Add(first);
            _lastPlayedClip = (AudioClip)first.GetQuestionItemByType(ButtonType.Sound);

            switch (_correctType)
            {
                case ButtonType.Position:
                    var alternativeSound = ModifyQuestionClip(first);
                    roundQuestions.Add(alternativeSound);
                    break;
                case ButtonType.Sound:
                    var alternativePosition = CreateAlternativePositionQuestion(first, alternativeQuestions);
                    roundQuestions.Add(alternativePosition);
                    break;
                case ButtonType.SoundAndPosition:
                    roundQuestions.Add(first);
                    break;
                case ButtonType.Null:
                    var question = GetCompletelyRandomQuestion(first);
                    roundQuestions.Add(question);
                    break;
                default:
                    Debug.LogError("THERE IS A CRUICAL ERROR");
                    break;
            }

            return roundQuestions;
        }

        private DualNBackQuestion CreateAlternativePositionQuestion(Question first, List<Question> alternativeQuestions)
        {
            var alternativePosition = ScriptableObject.CreateInstance<DualNBackQuestion>();
            var randomQuestion = alternativeQuestions[Random.Range(0, alternativeQuestions.Count)];

            while ((int)randomQuestion.GetQuestionItem() == (int)first.GetQuestionItem())
            {
                randomQuestion = alternativeQuestions[Random.Range(0, alternativeQuestions.Count)];
            }

            alternativePosition.Value = (int)randomQuestion.GetQuestionItem();
            alternativePosition.QuestionClip = _lastPlayedClip;

            return alternativePosition;
        }


        private DualNBackQuestion ModifyQuestionClip(Question first)
        {
            var alternativeQuestions = GetModeQuestions();
            var alternativeSound = ScriptableObject.CreateInstance<DualNBackQuestion>();
            alternativeSound.Value = (int)first.GetQuestionItem();
            var randomClipQuestion = alternativeQuestions[Random.Range(0, alternativeQuestions.Count)];
            var clip = (AudioClip)randomClipQuestion.GetQuestionItemByType(ButtonType.Sound);
            while (clip == _lastPlayedClip)
            {
                randomClipQuestion = alternativeQuestions[Random.Range(0, alternativeQuestions.Count)];
                clip = (AudioClip)randomClipQuestion.GetQuestionItemByType(ButtonType.Sound);
            }

            alternativeSound.QuestionClip = clip;
            return alternativeSound;
        }

        private DualNBackQuestion GetCompletelyRandomQuestion(Question currentQuestion)
        {
            var questions = GetModeQuestions();
            var randomIndex = Random.Range(0, questions.Count);
            var randomQuestion = questions[randomIndex];
            while ((int)randomQuestion.GetQuestionItem() == (int)currentQuestion.GetQuestionItem())
            {
                randomIndex = Random.Range(0, questions.Count);
                randomQuestion = questions[randomIndex];
            }

            var newQuestion = (DualNBackQuestion)randomQuestion;
            return newQuestion;
        }

        private List<Question> GetModeQuestions()
        {
            return _controller.GetAlternativeImagesByType(GameMode);
        }

        private void GetBlockHelper()
        {
            _blockUIHelper = _questionState.GetBlockHelper();
        }

        public void SetCorrectType()
        {
            _correctType = (ButtonType)Random.Range((int)ButtonType.Position, (int)ButtonType.Null + 1);
        }

        public bool IsSwitchable()
        {
            return _correctType != ButtonType.SoundAndPosition;
        }

        public int[] GetModeIndexes()
        {
            return _buttonIndexes;
        }

        public NBackModes GetModeEnum()
        {
            return GameMode;
        }
    }
}