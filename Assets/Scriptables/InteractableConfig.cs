using System;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Scriptables
{
    [CreateAssetMenu(fileName = "Interactable", menuName = "Interactable/InteractableConfig")]
    [Serializable]
    public class InteractableConfig : ScriptableObject
    {
        public Mesh ObjectMesh;
        public CommonFields.InteractableType InteractableType;
        public int Width;
        public int Height;
    }
}
