using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Spans.Skeleton
{
    public class SpanFeedbackState : MonoBehaviour, ISpanState
    {
        [SerializeField] private ParticleSystem successEffect;
        [SerializeField] private ParticleSystem failEffect;
        [SerializeField] private Image feedbackLabel;
        [SerializeField] private TextMeshProUGUI feedbackField;
        [SerializeField] private Sprite wrongSprite;
        [SerializeField] private Sprite correctSprite;
        [SerializeField] private Slider progressBar;
        private SpanController _spanController;
        public void Enter(SpanController spanController)
        {
            if (_spanController == null)
            {
                _spanController = spanController;
            }

            EnableUIElements();
            PlayEffects();
        }

        private void PlayEffects()
        {
            if (_spanController.IsAnswerCorrect())
            {
                successEffect.Play();
                progressBar.maxValue = _spanController.GetRoundIndex();
                progressBar.value += progressBar.maxValue / 4;
                ConfigureFeedbackField(true);
            }
            else
            {
                failEffect.Play();
                progressBar.value = 0;
                ConfigureFeedbackField(false);
            }
        }

        private void ConfigureFeedbackField(bool correct)
        {
            if (correct)
            {
                feedbackField.text = "Dogru!";
                feedbackLabel.sprite = correctSprite;
            }
            else
            {
                feedbackField.text = "Yanlis";
                feedbackLabel.sprite = wrongSprite;
            }

            feedbackLabel.transform.DOScale(1f, 1.5f).SetEase(Ease.OutBounce).OnComplete(() =>
            {
                _spanController.SwitchState();
            });
        }

        public void Exit()
        {
            DisableUIElements();
        }

        public void SwitchNextState()
        {
            _spanController.SwitchState();
        }

        public void EnableUIElements()
        {
            successEffect.gameObject.SetActive(true);
            failEffect.gameObject.SetActive(true);
            progressBar.gameObject.SetActive(true);
            feedbackLabel.gameObject.SetActive(true);
            feedbackField.gameObject.SetActive(true);
        }

        public void DisableUIElements()
        {
            successEffect.gameObject.SetActive(false);
            failEffect.gameObject.SetActive(false);
            feedbackLabel.gameObject.SetActive(false);
            feedbackLabel.transform.localScale = new Vector3(0,0,0);
        }
    }
}
