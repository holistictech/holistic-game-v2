using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Helpers
{
    public class CurrencyTrailHelper : MonoBehaviour
    {
        [SerializeField] private Image trailObject;
        private List<GameObject> _trailObjects = new List<GameObject>();
        private Image _currentTrail;
        private RectTransform _currentTarget; 

        public void AnimateCurrencyIncrease(CurrencyUIHelper currency, int amount, Action onComplete)
        {
            _currentTarget = currency.GetTargetAnimationPos();
            SpawnTrailObject(currency.GetFieldSprite(), amount);
            MoveTrailObjects(() =>
            {
                onComplete?.Invoke();
            });
        }

        private void SpawnTrailObject(Sprite objectSprite, int amount)
        {
            float paddingRange = 70f; // Adjust this value for more or less padding

            for (int i = 0; i < amount; i++)
            {
                _currentTrail = Instantiate(trailObject, transform);
                _currentTrail.sprite = objectSprite;

                // Add random padding to position
                float randomOffsetX = UnityEngine.Random.Range(-paddingRange, paddingRange);
                float randomOffsetY = UnityEngine.Random.Range(-paddingRange, paddingRange);
        
                _currentTrail.transform.localPosition += new Vector3(randomOffsetX, randomOffsetY, 0);
        
                _trailObjects.Add(_currentTrail.gameObject);
            }
        }


        private void MoveTrailObjects(Action onComplete)
        {
            Sequence masterSequence = DOTween.Sequence();
            for (int i = 0; i < _trailObjects.Count; i++)
            {
                Sequence animSequence = DOTween.Sequence();
                animSequence.AppendInterval(0.03f * i);
                animSequence.Join(_trailObjects[i].transform.DOJump(_currentTarget.position, 6, 1, .8f)
                    .SetEase(Ease.OutQuad));
                animSequence.Join(_trailObjects[i].transform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.8f));
                animSequence.AppendCallback(() =>
                {
                    _currentTarget.transform.DOShakeScale(0.35f, 0.7f);
                });
                var i1 = i;
                animSequence.AppendCallback(() =>
                {
                    Destroy(_trailObjects[i1].gameObject);
                    if (i1 == _trailObjects.Count - 1)
                    {
                        onComplete?.Invoke();
                    }
                });
                masterSequence.Append(animSequence);
            }
        }
    }
}
