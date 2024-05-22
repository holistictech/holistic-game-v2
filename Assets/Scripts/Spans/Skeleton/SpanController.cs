using System;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables;
using Scriptables.QuestionSystem;
using Scriptables.Tutorial;
using TMPro;
using Tutorial;
using UI;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Spans.Skeleton
{
    public abstract class SpanController : MonoBehaviour
    {
        [SerializeField] private Question[] spanQuestions;
        [SerializeField] protected bool isBackwards;
        [SerializeField] protected bool isCumulative;
        [SerializeField] private StateHolder states;
        [SerializeField] private TutorialManager tutorialManager;
        [SerializeField] private TextMeshProUGUI _spanNameField;
        [SerializeField] private RoundTimerHelper timerHelper;
        [SerializeField] private bool dummyTutorialBool;
        protected List<UnitCircle> activeUnitCircles = new List<UnitCircle>();
        private List<ISpanState> _stateList = new List<ISpanState>();
        protected SpanStateContext stateContext;
        protected int currentRoundIndex = CommonFields.DEFAULT_ROUND_INDEX;
        protected int fetchedQuestionCount = 9;

        protected int currentSuccessStreak;
        protected int currentFailStreak;
        private bool _tutorialActive;
        protected List<Question> currentSpanQuestions = new List<Question>();
        protected List<Question> currentDisplayedQuestions = new List<Question>();
        protected List<string> currentDetectedAnswers;
        protected List<Question> currentGivenAnswers = new List<Question>();
        private const int _neededStreakCount = 4;
        private bool _hasLeveledUp;
        private bool _isSpanFinished;
        
        protected SpanEventBus spanEventBus;
        private GameObject _helperObject;
        private List<GameObject> _tutorialHelpers = new List<GameObject>();

        public static event Action OnSpanFinished;
        private void SetSpanField()
        {
            _spanNameField.text = $"{gameObject.name}";
        }

        private void StartTimer()
        {
            timerHelper.InjectSpanController(this);
        }

        public void SetSpanCompleted()
        {
            _isSpanFinished = true;
        }

        protected virtual void Start()
        {
            currentRoundIndex = PlayerSaveManager.GetPlayerAttribute(gameObject.name, 2);
            StartTimer();
            stateContext = new SpanStateContext(this);
            spanEventBus = new SpanEventBus();
            SetSpanField();
            ResetQuestionStatus();
            InstantiateGameStates();
            if (dummyTutorialBool)
            {
                _tutorialActive = PlayerSaveManager.GetPlayerAttribute(states.TutorialKey, 0) == 0;
            }
            else
            {
                _tutorialActive = false;
            }
            stateContext.Transition(_stateList[0]);
        }

        public void SwitchState()
        {
            if (_isSpanFinished)
            {
                Debug.Log("this is finished");
                stateContext.Transition(_stateList[^1]);
                return;
            }
            
            var index = _stateList.IndexOf(stateContext.CurrentState);
            if (index < _stateList.Count - 2)
            {
                ISpanState nextState = _stateList[index+1];
                stateContext.Transition(nextState);
            }
            else
            {
                stateContext.Transition(_stateList[0]); // to turn back to initial state.
            }
        }

        public void EndSpan()
        {
            gameObject.SetActive(false);
            OnSpanFinished?.Invoke();
        }

        public virtual List<Question> GetSpanObjects()
        {
            return new List<Question>();
        }

        public virtual List<Question> GetChoices()
        {
            return new List<Question>();
        }

        protected Question[] GetAllAvailableSpanObjects()
        {
            return spanQuestions;
        }

        public virtual bool IsAnswerCorrect()
        {
            //@todo: change this with appropriate to game rules. 
            //maybe a comparator to check correctness percentage.
            for (int i = 0; i < currentDisplayedQuestions.Count; i++)
            {
                if (!currentDisplayedQuestions[i].CorrectAnswerString.Equals(currentDetectedAnswers[i], StringComparison.OrdinalIgnoreCase))
                {
                    IncrementFailStreak();
                    return false;
                }
            }

            IncrementSuccessStreak();
            return true;
        }
        

        public virtual void SetCurrentDisplayedQuestions(List<Question> questions)
        {
            currentDisplayedQuestions = questions;
        }

        public virtual List<Question> GetCurrentSpanQuestions()
        {
            return currentSpanQuestions;
        }

        public List<Question> GetCurrentDisplayedQuestions()
        {
            return currentDisplayedQuestions;
        }

        public void SetDetectedAnswers(List<string> detected)
        {
            currentDetectedAnswers = detected;
        }

        public void SetSelectedAnswers(List<Question> given)
        {
            currentGivenAnswers = given;
        }

        public virtual int GetRoundTime()
        {
            return 10;
        }

        public void ResetRoundIndex()
        {
            currentRoundIndex = CommonFields.DEFAULT_ROUND_INDEX;
        }

        public int GetRoundIndex()
        {
            return _hasLeveledUp ? currentRoundIndex - 1 : currentRoundIndex;
        }

        public void ResetLevelChangedStatus()
        {
            _hasLeveledUp = false;
        }

        private void IncrementRoundIndex()
        {
            _hasLeveledUp = true;
            currentRoundIndex++;
        }

        private void DecrementRoundIndex()
        {
            if(currentRoundIndex > CommonFields.DEFAULT_ROUND_INDEX)
                currentRoundIndex--;
            else
            {
                currentRoundIndex = CommonFields.DEFAULT_ROUND_INDEX;
            }
        }

        protected virtual void IncrementSuccessStreak()
        {
            StatisticsHelper.IncrementTrueCount();
            currentSuccessStreak++;
            currentFailStreak = 0;
            if (currentSuccessStreak == _neededStreakCount)
            {
                IncrementRoundIndex();
                currentSuccessStreak = 0;
            }
            else
            {
                _hasLeveledUp = false;
            }
        }

        protected virtual void IncrementFailStreak()
        {
            currentFailStreak++;
            currentSuccessStreak = 0;
            if (currentFailStreak == _neededStreakCount)
            {
                DecrementRoundIndex();
                currentFailStreak = 0;
            }
        }

        private void InstantiateGameStates()
        {
            foreach (var state in states.StatePrefabs)
            {
                var temp = Instantiate(state, transform);
                temp.transform.SetSiblingIndex(0);
                _stateList.Add(temp.GetComponent<ISpanState>());
            }
        }

        public void ExitSpan()
        {
            PlayerSaveManager.SavePlayerAttribute(currentRoundIndex, gameObject.name);
            EndSpan();
        }

        public void SetActiveCircles(List<UnitCircle> circles)
        {
            activeUnitCircles = new List<UnitCircle>(circles);
        }

        public List<UnitCircle> GetActiveCircles()
        {
            return activeUnitCircles;
        }

        public void SetHelperObject(GameObject helper)
        {
            if(_helperObject == null)
                _helperObject = helper;
        }

        public SpanEventBus GetEventBus()
        {
            return spanEventBus;
        }

        public GameObject GetHelperObject()
        {
            return _helperObject;
        }

        public bool GetTutorialStatus()
        {
            return _tutorialActive;
        }

        public void SetTutorialHelperObjects(List<GameObject> tutorialObjects)
        {
            _tutorialHelpers = tutorialObjects;
        }

        public List<GameObject> GetTutorialHelperObjects()
        {
            return _tutorialHelpers;
        }

        public void SetTutorialCompleted()
        {
            _tutorialActive = false;
            tutorialManager.DisablePanel();
            PlayerSaveManager.SavePlayerAttribute(1, states.TutorialKey);
        }

        public void SetHelperTutorialCompleted()
        {
            foreach (var item in _tutorialHelpers)
            {
                item.gameObject.SetActive(false);
            }
            PlayerSaveManager.SavePlayerAttribute(1, $"Helper{states.TutorialKey}");
        }

        public bool IsHelperTutorialNeeded()
        {
            return PlayerSaveManager.GetPlayerAttribute($"Helper{states.TutorialKey}", 0) == 0;
        }

        public void TriggerStateTutorial(Dictionary<GameObject, TutorialStep> tutorials, bool needHand, Action onComplete)
        {
            tutorialManager.ActivateStateTutorial(tutorials, needHand, () =>
            {
                onComplete?.Invoke();
            });
        }

        public void TriggerFinalTutorialField(string text, AudioClip clip)
        {
            tutorialManager.SetFinalTutorialField(text, clip);
        }
        
        public void HighlightTarget(RectTransform target, RectTransform parent, bool animNeeded, float offset = 0)
        {
            tutorialManager.HighlightTutorialObject(target, parent, offset, animNeeded);
        }

        public void ClearTutorialHighlights()
        {
            tutorialManager.ClearHighlights();
        }

        protected void ResetQuestionStatus()
        {
            foreach (var question in spanQuestions)
            {
                question.SetHasSelected(false);
            }
        }

        public bool GetBackwardStatus()
        {
            return isBackwards;
        }

        public bool GetCumulativeStatus()
        {
            return isCumulative;
        }
    }
}

public static class ListExtensions
{
    private static readonly System.Random rng = new System.Random();

    public static void Shuffle<T>(this IList<T> list)
    {
        int n = list.Count;
        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            (list[k], list[n]) = (list[n], list[k]);
        }
    }
}