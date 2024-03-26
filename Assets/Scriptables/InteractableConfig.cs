using UnityEngine;
using Utilities;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "Interactable", menuName = "Interactable/InteractableConfig")]
    public class InteractableConfig : ScriptableObject
    {
        public GameObject Object;
        public int Width;
        public int Height;
    }
}
