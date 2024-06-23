using System;
using System.Collections.Generic;
using System.Linq;
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
        private List<Option> _activeOptions = new List<Option>();
        private List<Color> _activeOptionColors = new List<Color>();
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
                tempOption.gameObject.SetActive(false);
                _pooledOptions.Add(tempOption);
            }
        }

        public void ConfigureOptions(List<Question> options)
        {
            optionParent.gameObject.SetActive(true);
            foreach (var element in options)
            {
                if (!_activeOptionColors.Contains((Color)element.GetQuestionItem()))
                {
                    var temp = GetAvailableOption();
                    temp.ConfigureOption(element, this);
                    _activeOptions.Add(temp);
                    _activeOptionColors.Add((Color)element.GetQuestionItem());
                }
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

        public void DisableActiveOptions()
        {
            _currentSelection = null;
            foreach (var option in _activeOptions)
            {
                option.ResetOption();
            }
            _activeOptions.Clear();
            _activeOptionColors.Clear();
            optionParent.gameObject.SetActive(false);
        }

        public static Question GetCurrentSelection()
        {
            return _currentSelection.GetQuestion();
        }

        private Option GetAvailableOption()
        {
            foreach (var option in _pooledOptions)
            {
                if (!option.gameObject.activeSelf)
                {
                    option.gameObject.SetActive(true);
                    return option;
                }
            }

            throw new Exception("Could not find available option. Need to pool more");
        }
    }
}
