using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Events.Leaderboard
{
    public class LeaderboardEntry : MonoBehaviour
    {
        [SerializeField] private Image avatar;
        [SerializeField] private Sprite[] possibleAvatars;
        [SerializeField] private TextMeshProUGUI usernameField;
        [SerializeField] private TextMeshProUGUI scoreField;

        private bool _isLocalUser;
        
        public void ConfigureEntry(LeaderboardUserModel user)
        {
            avatar.sprite = GetRandomSprite();
            usernameField.text = user.Username;
            scoreField.text = "100";
            _isLocalUser = user.IsLocal;
        }

        private Sprite GetRandomSprite()
        {
            return possibleAvatars[Random.Range(0, possibleAvatars.Length)];
        }

        public void ResetSelf()
        {
            usernameField.text = "";
            _isLocalUser = false;
        }

        public bool GetIsLocal()
        {
            return _isLocalUser;
        }
    }
}
