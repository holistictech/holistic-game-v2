using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Utilities.Helpers
{
    public class LoadingScreen : MonoBehaviour
    {
        [SerializeField] private GameObject loadingPanel;
        [SerializeField] private Slider loadingSlider;
        [SerializeField] private TextMeshProUGUI loadingField;

        private Coroutine _loading;
        private float _lerpDuration = 5f;

        public void ConfigureScreen(bool hasFinished)
        {
            Debug.Log("load triggered with: " + hasFinished);
            if (!hasFinished)
            {
                EnablePanel();
            }
            else
            {
                DisablePanel();
            }
        }

        private void EnablePanel()
        {
            if (_loading != null)
            {
                StopCoroutine(_loading);
            }
            
            loadingPanel.SetActive(true);
            _loading = StartCoroutine(LerpSlider());
        }

        private IEnumerator LerpSlider()
        {
            float startValue = loadingSlider.value;
            float elapsedTime = 0f;
            float targetValue = 1f; 

            while (elapsedTime < _lerpDuration)
            {
                elapsedTime += Time.deltaTime;
                loadingSlider.value = Mathf.Lerp(startValue, targetValue, elapsedTime / _lerpDuration);
                loadingField.text = $"Yükleniyor...{(int)(loadingSlider.value / targetValue * 100)}%";
                yield return new WaitForEndOfFrame();
            }

            loadingSlider.value = targetValue;
        }

        private void DisablePanel()
        {
            loadingSlider.value = 1f;
            if (_loading != null)
            {
                StopCoroutine(_loading);
            }
            
            loadingPanel.SetActive(false);
        }
    }
}
