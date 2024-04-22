using System;
using System.Collections.Generic;
using DG.Tweening;
using Scriptables;
using Scriptables.QuestionSystem;
using Scriptables.Tutorial;
using Tutorial;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Spans.Skeleton
{
    public abstract class SpanController : MonoBehaviour
    {
        [SerializeField] private StateHolder states;
        [SerializeField] private TutorialManager tutorialManager;
        private List<ISpanState> _stateList = new List<ISpanState>();
        protected SpanStateContext stateContext;
        protected int currentRoundIndex = CommonFields.DEFAULT_ROUND_INDEX;
        protected int fetchedQuestionCount = 9;

        protected int currentSuccessStreak;
        protected int currentFailStreak;
        private bool _tutorialActive;
        protected List<Question> currentSpanQuestions;
        protected List<Question> currentDisplayedQuestions;
        protected List<string> currentDetectedAnswers;
        protected List<Question> currentGivenAnswers;
        private const int _neededStreakCount = 4;

        protected virtual void Start()
        {
            stateContext = new SpanStateContext(this);
            InstantiateGameStates();
            _tutorialActive = PlayerSaveManager.GetPlayerAttribute(states.TutorialKey, 0) == 0;
            stateContext.Transition(_stateList[0]);
        }

        public void SwitchState()
        {
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

        public virtual List<Question> GetSpanObjects()
        {
            return new List<Question>();
        }

        public virtual List<Question> GetChoices()
        {
            return new List<Question>();
        }

        public virtual bool IsAnswerCorrect()
        {
            //@todo: change this with appropriate to game rules. 
            //maybe a comparator to check correctness percentage.
            for (int i = 0; i < currentDisplayedQuestions.Count; i++)
            {
                if (!currentDisplayedQuestions[i].CorrectAnswer.Equals(currentDetectedAnswers[i], StringComparison.OrdinalIgnoreCase))
                {
                    IncrementFailStreak();
                    return false;
                }
            }

            IncrementSuccessStreak();
            return true;
        }

        public void SetCurrentQuestions(List<Question> questions)
        {
            currentDisplayedQuestions = questions;
        }

        public List<Question> GetCurrentQuestions()
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
            return currentRoundIndex;
        }

        public void IncrementRoundIndex()
        {
            currentRoundIndex++;
        }

        public void DecrementRoundIndex()
        {
            if(currentRoundIndex > CommonFields.DEFAULT_ROUND_INDEX)
                currentRoundIndex--;
            else
            {
                currentRoundIndex = CommonFields.DEFAULT_ROUND_INDEX;
            }
        }

        public void IncrementSuccessStreak()
        {
            currentSuccessStreak++;
            currentFailStreak = 0;
            if (currentSuccessStreak == _neededStreakCount)
            {
                IncrementRoundIndex();
                currentSuccessStreak = 0;
            }
        }

        public void IncrementFailStreak()
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

        public bool GetTutorialStatus()
        {
            return _tutorialActive;
        }

        public void SetTutorialCompleted()
        {
            _tutorialActive = false;
            PlayerSaveManager.SavePlayerAttribute(1, states.TutorialKey);
        }

        public void TriggerStateTutorial(Dictionary<GameObject, TutorialStep> tutorials, Action onComplete)
        {
            tutorialManager.ActivateStateTutorial(tutorials, () =>
            {
                onComplete?.Invoke();
            });
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