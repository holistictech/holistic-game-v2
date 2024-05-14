using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Helpers
{
    public class CurrencyTrailHelper : MonoBehaviour
    {
        [SerializeField] private Image trailObject;
        private Image _currentTrail;
        private RectTransform _currentTarget; 

        public void AnimateTrail(EnergyUIHelper energy, Action onComplete)
        {
            _currentTarget = energy.GetTargetAnimationPos();
            SpawnTrailObject(energy.GetFieldSprite());
            MoveTrailObject(() =>
            {
                onComplete?.Invoke();
            });
        }

        private void SpawnTrailObject(Sprite objectSprite)
        {
            _currentTrail = Instantiate(trailObject, transform);
            _currentTrail.sprite = objectSprite;
        }

        private void MoveTrailObject(Action onComplete)
        {
            Sequence animSequence = DOTween.Sequence();
            animSequence.AppendInterval(0.1f * .5f);
            animSequence.Append(_currentTrail.transform.DOScale(new Vector3(1.5f, 1.5f,1.5f), .7f).SetLoops(2, LoopType.Yoyo));
            animSequence.Join(_currentTrail.transform.DOJump(_currentTarget.position, 20, 1, 1.5f).SetEase(Ease.OutQuad));
            animSequence.AppendCallback(() =>
            {
                Destroy(_currentTrail.gameObject);
                onComplete?.Invoke();
            });
            
        }
    }
}
