using UnityEngine;

namespace Utilities
{
    public class PlayerInventory<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        
        private string CurrencyKey = "CurrencyKey";
        public int Currency { get; private set; }

        private string ExperienceKey = "ExperienceKey";

        public int Experience { get; private set; }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();

                    if (_instance == null)
                    {
                        GameObject singletonObject = new GameObject(typeof(T).Name);
                        _instance = singletonObject.AddComponent<T>();
                    }
                }

                return _instance;
            }
        }

        protected virtual void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this as T;
            DontDestroyOnLoad(gameObject);

            Currency = PlayerSaveManager.GetPlayerAttribute(CurrencyKey, 0);
            Experience = PlayerSaveManager.GetPlayerAttribute(ExperienceKey, 0 );
        }

        public void ChangeCurrencyAmount(int amount)
        {
            Currency += amount;
            PlayerSaveManager.SavePlayerAttribute(Currency, CurrencyKey);
        }

        public void ChangeExperienceAmount(int amount)
        {
            Experience += amount;
            PlayerSaveManager.SavePlayerAttribute(Experience, ExperienceKey);
        }
    }

}
