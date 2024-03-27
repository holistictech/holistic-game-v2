using UnityEngine;
using Utilities;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "Interactable", menuName = "Interactable/InteractableConfig")]
    public class InteractableConfig : ScriptableObject
    {
        public Mesh Object;
        public CommonFields.InteractableType InteractableType;
        public int Width;
        public int Height;
    }
}
