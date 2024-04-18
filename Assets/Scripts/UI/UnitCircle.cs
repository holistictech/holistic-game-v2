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
            circleImage.color = activeColor;
            EnableSelf();
        }

        public void EnableSelf()
        {
            gameObject.SetActive(true);
        }

        public void ResetSelf()
        {
            circleImage.color = Color.white;
            gameObject.SetActive(false);
        }
    }
}
