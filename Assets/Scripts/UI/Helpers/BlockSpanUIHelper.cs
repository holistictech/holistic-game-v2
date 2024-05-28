using System;
using System.Collections.Generic;
using Scriptables.QuestionSystem;
using UI.CorsiBlockTypes;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Helpers
{
    public class BlockSpanUIHelper : CorsiBlockUIHelper
    {
        [SerializeField] private CorsiBlock blockImage;

        private List<CorsiBlock> _blockPool = new List<CorsiBlock>();

        private void Start()
        {
            InstantiatePool();
        }

        public override void AssignQuestions(List<Question> roundQuestions)
        {
            foreach (var question in roundQuestions)
            {
                var block = GetAvailableBlock();
                block.ConfigureSelf(question, this);
            }
        }

        private void InstantiatePool()
        {
            for (int i = 0; i < 9; i++)
            {
                var tempBlock = Instantiate(blockImage, transform);
                _blockPool.Add(tempBlock);
            }
        }

        private CorsiBlock GetAvailableBlock()
        {
            foreach (var block in _blockPool)
            {
                if (!block.isActiveAndEnabled)
                {
                    return block;
                }
            }

            throw new Exception("Could not find available block");
        }
    }
}
