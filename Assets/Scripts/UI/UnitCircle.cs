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

        private int _circleIndex = -1;

        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        public void ConfigureUI(int index)
        {
            _circleIndex = index;
            circleImage.DOColor(activeColor, 1f).SetEase(Ease.Flash).OnComplete(ResetSelf);
        }

        private void HandleOnAnswerGiven(int index)
        {
            if (index == _circleIndex)
            {
                circleImage.color = activeColor;
            }else if (index == -_circleIndex)
            {
                ResetSelf();
            }else if (index == 0)
            {
                ResetSelf();
                DisableSelf();
            }
        }

        public void EnableSelf()
        {
            gameObject.SetActive(true);
        }

        public void DisableSelf()
        {
            _circleIndex = -1;
            gameObject.SetActive(false);
        }

        public void ResetSelf()
        {
            circleImage.color = Color.white;
        }

        private void AddListeners()
        {
            GridUIHelper.OnChoiceSelected += HandleOnAnswerGiven;
        }

        private void RemoveListeners()
        {
            GridUIHelper.OnChoiceSelected -= HandleOnAnswerGiven;
        }
    }
}
