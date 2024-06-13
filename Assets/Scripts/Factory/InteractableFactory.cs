using System;
using Interactables;
using Scriptables;
using UnityEngine;
using Utilities;
using Utilities.Helpers;
using static Utilities.Helpers.CommonFields;

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
                case InteractableType.Concrete:
                    instance.AddComponent<ConcreteBuilding>();
                    break;
                case InteractableType.Barn:
                    instance.AddComponent<BarnBuilding>();
                    break;
                case InteractableType.Animal:
                    instance.AddComponent<Animal>();
                    break;
                case InteractableType.Field:
                    instance.AddComponent<Field>();
                    break;
                case InteractableType.Plant:
                    instance.AddComponent<Plant>();
                    break;
                default:
                    throw new ArgumentException("No such interactable type :" + config.InteractableType);
            }
            return instance;
        }
    }
}
