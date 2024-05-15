using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Helpers
{
    using UnityEngine;
    using UnityEngine.UI;
    using System;

    public class TimerHelper : MonoBehaviour
    {
        [SerializeField] private Image timerBg;
        [SerializeField] private Image timerFill;
        [SerializeField] private TextMeshProUGUI remainingTime;

        private Coroutine _timer;

        public void StartTimer(float maxTime, Action onComplete)
        {
            timerBg.gameObject.SetActive(true);
            _timer = StartCoroutine(PlayTimer(maxTime, onComplete));
        }

        public void EnableSelf()
        {
            gameObject.SetActive(true);
            timerBg.gameObject.SetActive(true);
        }

        private IEnumerator PlayTimer(float maxTime, Action onComplete)
        {
            Vector3 initialScale = new Vector3(1, 1, 1);
            float currentTime = maxTime;

            while (currentTime > 0f)
            {
                float t = (currentTime / maxTime);
                float targetScaleX = Mathf.Lerp(0f, initialScale.x, t);
                timerFill.transform.localScale = new Vector3(targetScaleX, initialScale.y, initialScale.z);
                
                int wholeSeconds = Mathf.RoundToInt(currentTime);
                remainingTime.text = $"{wholeSeconds}";

                currentTime -= Time.deltaTime;
                yield return null;
            }
            
            timerFill.transform.localScale = new Vector3(0f, initialScale.y, initialScale.z);
            onComplete?.Invoke();
        }

        public GameObject GetBackgroundForTutorial()
        {
            return timerBg.gameObject;
        }

        public void StopTimer()
        {
            if (_timer != null)
            {
                StopCoroutine(_timer);
            }
            timerBg.gameObject.SetActive(false);
        }
    }

}
