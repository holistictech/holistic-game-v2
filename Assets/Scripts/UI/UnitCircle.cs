using System;
using DG.Tweening;
using Spans.Skeleton.AnswerStates;
using UI.Helpers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitCircle : MonoBehaviour
    {
        [SerializeField] private Image circleImage;
        [SerializeField] private Color activeColor;

        private Color _assignedColor = Color.white;
        
        private Tween _jump;
        
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        public void ConfigureUI(float duration)
        {
            circleImage.DOColor(activeColor, duration).SetEase(Ease.Linear).OnComplete(ResetSelf);
        }
        
        public void ConfigureUI(float duration, Color color)
        {
            _assignedColor = color;
            circleImage.DOColor(_assignedColor, duration).SetEase(Ease.Linear).OnComplete(ResetSelf);
        }

        public void ChangeColor(Color color)
        {
            circleImage.color = color;
        }

        public void OnAnswerGiven()
        {
            circleImage.color = _assignedColor == Color.white ? activeColor : _assignedColor;
            ResetScale();
        }

        public void OnAnswerRevoked()
        {
            ResetSelf();
        }

        public void AnimateCircle()
        {
            _jump = circleImage.transform.DOScale(new Vector3(1f, 1f, 1f), .6f).SetEase(Ease.OutBack)
                .SetLoops(-1, LoopType.Yoyo);
            _jump.Play();
        }

        public void DisableCircle()
        {
            ResetSelf();
            DisableSelf();
        }

        public void EnableSelf()
        {
            ResetScale();
            gameObject.SetActive(true);
        }

        public void DisableSelf()
        {
            gameObject.SetActive(false);
        }

        public void ResetSelf()
        {
            ResetScale();
            circleImage.color = Color.white;
            _assignedColor = Color.white;
        }

        private void ResetScale()
        {
            _jump.Kill();
            transform.localScale = new Vector3(.8f, .8f, .8f);
        }

        private void AddListeners()
        {
            GridUIHelper.OnRoundFinished += DisableCircle;
        }

        private void RemoveListeners()
        {
            GridUIHelper.OnRoundFinished -= DisableCircle;
        }
    }
}
