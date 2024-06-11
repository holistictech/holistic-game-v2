using System;
using Scriptables;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class Sketch : MonoBehaviour
    {
        [SerializeField] private MeshFilter meshFilter;
        private SketchUIHelper _uiHelper;

        public static event Action OnPlacementConfirmed;
        public static event Action OnPlacementCancelled;
        

        public void ConfigureObjectMesh(Mesh mesh)
        {
            meshFilter.mesh = mesh;
        }

        public void ConfigureSize(InteractableConfig config)
        {
            var amount = config.ScaleAmount;
            transform.localScale = new Vector3(amount, amount, amount);
        }

        public void SetUIHelperReference(SketchUIHelper uiHelper)
        {
            _uiHelper = uiHelper;
            _uiHelper.SetSketchReference(this);
        }

        public void DisableButtonsWhileMoving()
        {
            _uiHelper.DisableButtons();
        }

        public void EnableButtons(Vector3 position)
        {
            _uiHelper.EnableButtons(position);
        }

        private void DestroyObject()
        {
            Destroy(gameObject);
        }

        public bool ButtonsEnabled()
        {
            return _uiHelper.ButtonEnabled();
        }

        public void ConfirmPlacement()
        {
            OnPlacementConfirmed?.Invoke();
            DestroyObject();
        }

        public void CancelPlacement()
        {
            OnPlacementCancelled?.Invoke();
            DestroyObject();
        }
    }
}
