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
                int currentIndex = i;
                Transform trailTransform = _trailObjects[currentIndex].transform;

                Sequence animSequence = DOTween.Sequence();
                animSequence.AppendInterval(0.1f);
                animSequence.Append(trailTransform.DOJump(_currentTarget.position, 6, 1, 0.6f).SetEase(Ease.OutQuad));
                animSequence.Join(trailTransform.DOScale(new Vector3(0.5f, 0.5f, 0.5f), 0.4f));
                animSequence.AppendCallback(() =>
                {
                    _currentTarget.transform.DOShakeScale(0.2f, 0.5f);
                });

                masterSequence.Append(animSequence);
            }

            masterSequence.OnComplete(() =>
            {
                for (int i = _trailObjects.Count - 1; i >= 0; i--)
                {
                    Destroy(_trailObjects[i].gameObject);
                }
                
                _trailObjects.Clear();
                onComplete?.Invoke();
            });
        }
    }
}
