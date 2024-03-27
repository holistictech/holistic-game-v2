using System;
using Interactables;
using Scriptables;
using UnityEngine;
using Utilities;

namespace Factory
{
    public class InteractableFactory : MonoBehaviour
    {
        public static GameObject SpawnInstance(GameObject baseObject, InteractableConfig config, Transform parent)
        {
            GameObject instance = Instantiate(baseObject.gameObject, parent.position, Quaternion.identity);
            instance.transform.SetParent(parent);

            switch (config.InteractableType)
            {
                case CommonFields.InteractableType.Concrete:
                    instance.AddComponent<ConcreteBuilding>();
                    break;
                case CommonFields.InteractableType.Barn:
                    instance.AddComponent<BarnBuilding>();
                    break;
                case CommonFields.InteractableType.Animal:
                    instance.AddComponent<Animal>();
                    break;
                default:
                    throw new ArgumentException("No such interactable type :" + config.InteractableType);
            }
            return instance;
        }
    }
}
