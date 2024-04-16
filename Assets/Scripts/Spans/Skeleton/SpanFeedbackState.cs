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

        private readonly string[] _successFeedbacks = new string[]
        {
            "Harika!",
            "Başardın!",
            "Çok iyi!",
            "Aferin sana!",
            "Süper!",
            "Tam isabet!",
            "Mükemmel!",
            "Doğru cevap!",
            "İşte bu!",
            "Çok güzel!",
            "Bravo!",
            "Çok akıllıca!",
            "Ne kadar zekisin!",
            "Çok başarılı!",
            "Harikasın!",
        };
        private readonly string[] _failFeedbacks = new string[]
        {
            "Bir daha deneyelim!",
            "Çok yaklaştın, tekrar dene!",
            "Az kaldı, bir kez daha dene!",
            "Hiç sorun değil, birlikte doğru cevabı bulalım.",
            "Olur böyle, önemli olan denemek!",
            "Bu sefer olmadı ama doğru yoldasın!",
            "İyi bir denemeydi, şimdi düzeltebiliriz!",
            "Herkes hata yapabilir, önemli olan tekrar denemektir.",
            "Hatalardan öğreniyoruz, bir daha deneyelim",
            "Biraz daha çalışırsak yapabiliriz.",
            "Her zaman ilk seferde olmaz, tekrar deneyelim",
            "Doğru cevaba bir adım daha yaklaştın, devam et!",
            "Bu sefer olmadı ama bir dahaki sefere daha iyi olacak!",
        };
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
            feedbackLabel.sprite = correct ? correctSprite : wrongSprite;
            
            feedbackField.text = GetRandomFeedback(correct);

            feedbackLabel.transform.DOScale(1f, 1.5f).SetEase(Ease.OutBack).OnComplete(() =>
            {
                DOVirtual.DelayedCall(2.5f, () =>
                {
                    _spanController.SwitchState();
                });
            });
        }

        private string GetRandomFeedback(bool isCorrect)
        {
            var index = Random.Range(0, _successFeedbacks.Length);
            return isCorrect ? _successFeedbacks[index] : _failFeedbacks[index];
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
