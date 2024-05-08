using System;
using System.Collections.Generic;
using DG.Tweening;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Spans.Skeleton
{
    public class SpanQuestionState : MonoBehaviour, ISpanState
    {
        [SerializeField] protected HorizontalLayoutGroup unitParent;
        [SerializeField] protected UnitCircle unit;

        private List<UnitCircle> _spawnedUnitPool;
        private List<UnitCircle> _activeCircles;
        private SpanController _spanController;
        protected Coroutine displayingQuestions;
        
        private void Start()
        {
            SpawnUnitCircles();
        }
        
        public virtual void Enter(SpanController spanController)
        {
            if (_spanController == null)
            {
                _spanController = spanController;
            }
            EnableUIElements();
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
        }

        public virtual void SwitchNextState()
        {
            _spanController.SwitchState();
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
            
            _spanController.SetActiveCircles(_activeCircles);
        }

        public void SpawnUnitCircles()
        {
            _spawnedUnitPool = new List<UnitCircle>();
            _activeCircles = new List<UnitCircle>();
            for (int i = 0; i < 9; i++)
            {
                var tempCircle = Instantiate(unit, unitParent.transform);
                _spawnedUnitPool.Add(tempCircle);
                tempCircle.DisableSelf();
            }
        }

        public void ActivateCircle(int index)
        {
            if (_activeCircles != null && _activeCircles.Count > index)
            {
                _activeCircles[index].ConfigureUI();
            }
        }
        
        public void ResetPreviousCircles()
        {
            foreach (var circle in _activeCircles)
            {
                circle.ResetSelf();
            }
            _activeCircles.Clear();
        }

        public UnitCircle GetAvailableUnitCircle()
        {
            foreach (var circle in _spawnedUnitPool)
            {
                if (!circle.isActiveAndEnabled)
                {
                    return circle;
                }
            }

            throw new Exception("No available unit circle");
            return null;
        }
        
        public void RotateCircles(Action onComplete)
        {
            unitParent.transform.DORotate(new Vector3(0, 0, -180), .5f).SetEase(Ease.Linear).OnComplete(() =>
            {
                unitParent.transform.rotation = Quaternion.Euler(0, 0, 0);
                onComplete?.Invoke();
            });
        }
    }
}
