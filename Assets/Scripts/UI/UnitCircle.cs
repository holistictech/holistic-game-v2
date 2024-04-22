using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class UnitCircle : MonoBehaviour
    {
        [SerializeField] private Image circleImage;
        [SerializeField] private Color activeColor;

        public void ConfigureUI()
        {
            circleImage.DOColor(activeColor, 1f).SetEase(Ease.Flash).OnComplete(ResetSelf);
        }

        public void EnableSelf()
        {
            gameObject.SetActive(true);
        }

        public void DisableSelf()
        {
            gameObject.SetActive(false);
        }

        public void ResetSelf()
        {
            circleImage.color = Color.white;
        }
    }
}
