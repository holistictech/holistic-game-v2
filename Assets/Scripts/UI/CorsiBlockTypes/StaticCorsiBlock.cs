using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace UI.CorsiBlockTypes
{
    public class StaticCorsiBlock : CorsiBlock
    {
        [SerializeField] private Sprite eggSprite;
        [SerializeField] private Sprite chickSprite;
        [SerializeField] private ParticleSystem dustEffect;
        
        public override void AnimateSelf()
        {
            blockImage.transform.DOShakeScale(0.5f, .3f).SetEase(Ease.Flash).OnComplete(() =>
            {
                dustEffect.Play();
                DOVirtual.DelayedCall(0.1f, () =>
                {
                    blockImage.sprite = chickSprite;
                    DOVirtual.DelayedCall(0.9f, ResetUI);
                });
            });
        }

        public override void ResetUI()
        {
            blockButton.interactable = true;
            blockImage.sprite = eggSprite;
            blockImage.color = Color.white;
        }
    }
}
