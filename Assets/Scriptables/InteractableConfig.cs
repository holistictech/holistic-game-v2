using System;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;
using Utilities.Helpers;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "Interactable", menuName = "Interactable/InteractableConfig")]
    [Serializable]
    public class InteractableConfig : ScriptableObject
    {
        public int MeshId;
        public CommonFields.InteractableType InteractableType;
        public int Width;
        public int Height;
    }
}
