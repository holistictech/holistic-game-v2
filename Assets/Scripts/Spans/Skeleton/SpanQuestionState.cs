using System;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Spans.CumulativeSpan;
using UI;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace Spans.Skeleton
{
    public class SpanQuestionState : MonoBehaviour, ISpanState
    {
        [SerializeField] protected HorizontalLayoutGroup unitParent;
        [SerializeField] protected UnitCircle unit;

        private List<UnitCircle> _spawnedUnitPool;
        private List<UnitCircle> _activeCircles;
        protected int currentQuestionIndex;
        protected SpanController spanController;
        protected Coroutine displayingQuestions;
        
        protected virtual void Start()
        {
            SpawnUnitCircles();
        }
        
        public virtual void Enter(SpanController controller)
        {
            if (spanController == null)
            {
                spanController = controller;
                EventBus.Instance.Register<RoundResetEvent>(OnRoundReset);
            }
            //EnableUIElements();
            
        }

        public virtual void ShowQuestion()
        {
        }

        public virtual void Exit()
        {
            DisableUIElements();
            if (displayingQuestions != null)
            {
                StopCoroutine(displayingQuestions);
            }
            ResetPreviousCircles();
        }

        public virtual void SwitchNextState()
        {
            spanController.SwitchState();
        }

        public virtual void TryShowStateTutorial()
        {
        }

        public virtual void EnableUIElements()
        {
            unitParent.gameObject.SetActive(true);
        }

        public virtual void DisableUIElements()
        {
        }
        
        public void SetCircleUI(int count)
        {
            for (int i = 0; i < count; i++)
            {
                var tempCircle = GetAvailableUnitCircle();
                tempCircle.EnableSelf();
                _activeCircles.Add(tempCircle);
            }
            
            spanController.SetActiveCircles(_activeCircles);
        }

        private void SpawnUnitCircles()
        {
            _spawnedUnitPool = new List<UnitCircle>();
            _activeCircles = new List<UnitCircle>();
            for (int i = 0; i < 25; i++)
            {
                var tempCircle = Instantiate(unit, unitParent.transform);
                _spawnedUnitPool.Add(tempCircle);
                tempCircle.DisableSelf();
            }
        }

        public void ActivateCircle(int index, float duration)
        {
            if (_activeCircles != null && _activeCircles.Count > index)
            {
                _activeCircles[index].ConfigureUI(duration);
            }
        }
        
        protected void ActivateCircle(int index, float duration, Color color)
        {
            if (_activeCircles != null && _activeCircles.Count > index)
            {
                _activeCircles[index].ConfigureUI(duration, color);
            }
        }
        
        protected void HighlightPreviousCircle()
        {
            var circle = _activeCircles[^2];
            if(circle != null)
                circle.ChangeColor(Color.green);
        }

        public void ResetPreviousCircles()
        {
            foreach (var circle in _activeCircles)
            {
                circle.ResetSelf();
            }
            
            _activeCircles.Clear();
        }

        private UnitCircle GetAvailableUnitCircle()
        {
            foreach (var circle in _spawnedUnitPool)
            {
                if (!circle.isActiveAndEnabled)
                {
                    return circle;
                }
            }

            throw new Exception("No available unit circle");
        }

        protected void RotateCircles(int endValue, Action onComplete)
        {
            Quaternion originalRotation = unitParent.transform.rotation;
            
            unitParent.transform.DORotate(new Vector3(originalRotation.eulerAngles.x, originalRotation.eulerAngles.y, endValue), .5f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    unitParent.transform.rotation = originalRotation;
                    onComplete?.Invoke();
                });
        }

        public virtual void OnDestroy()
        {
            EventBus.Instance.Unregister<RoundResetEvent>(OnRoundReset);
        }

        private void OnRoundReset(RoundResetEvent reset)
        {
            currentQuestionIndex = 0;
        }
    }
}
