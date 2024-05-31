using System;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class OptionPicker : MonoBehaviour
    {
        [SerializeField] private HorizontalLayoutGroup optionParent;
        [SerializeField] private Option optionPrefab;

        private List<Option> _pooledOptions = new List<Option>();
        private static Option _currentSelection;

        private void Start()
        {
            SpawnPool();
        }

        private void SpawnPool()
        {
            for(int i = 0; i < 3; i++)
            {
                var tempOption = Instantiate(optionPrefab, optionParent.transform);
                _pooledOptions.Add(tempOption);
            }
        }

        public void ConfigureOptions(List<Question> options)
        {
            foreach (var element in options)
            {
                var temp = GetAvailableOption();
                temp.ConfigureOption(element);
            }
        }

        public void SetOptionActive(Option option)
        {
            _currentSelection = option;
            foreach (var element in _pooledOptions)
            {
                if (element.isActiveAndEnabled)
                {
                    if (element != option)
                    {
                        element.ScaleOption(1f);
                    }
                }
            }
        }

        public static Question GetCurrentSelection()
        {
            return _currentSelection.GetQuestion();
        }

        private Option GetAvailableOption()
        {
            foreach (var option in _pooledOptions)
            {
                if (!option.isActiveAndEnabled)
                    return option;
            }

            throw new Exception("Could not find available option. Need to pool more");
        }
    }
}
