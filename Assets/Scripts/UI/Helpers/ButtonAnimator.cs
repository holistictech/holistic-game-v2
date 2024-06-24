using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
using Utilities;

namespace UI.Helpers
{
    public class ButtonAnimator : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private AudioClip clickClip;
        private readonly float _amount = 0.08f;
        
        private void OnEnable()
        {
            AddListeners();
        }

        private void OnDisable()
        {
            RemoveListeners();
        }

        private void OnValidate()
        {
            button = GetComponent<Button>();
        }

        private void OnButtonClick()
        {
            Sequence buttonSequence = DOTween.Sequence();
            if(clickClip != null)
                AudioManager.Instance.PlayAudioClip(clickClip);
            buttonSequence.Append(button.transform.DOPunchScale(new Vector3(-_amount, -_amount, -_amount), 0.15f).SetEase(Ease.OutQuad).OnComplete(
                () =>
                {
                    button.transform.localScale = new Vector3(1, 1, 1);
                }));
        }

        private void AddListeners()
        {
            button.onClick.AddListener(OnButtonClick);
        }

        private void RemoveListeners()
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}
