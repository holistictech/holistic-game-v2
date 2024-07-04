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
        private bool _rotatable;

        public static event Action<Quaternion> OnPlacementConfirmed;
        public static event Action OnPlacementCancelled;
        

        public void ConfigureObjectMesh(Mesh mesh, bool rotatable)
        {
            meshFilter.mesh = mesh;
            _rotatable = rotatable;
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

        public void RotateSelf()
        {
            var currentRotation = transform.rotation.eulerAngles;
            var newYRotation = currentRotation.y + 90f;
            transform.rotation = Quaternion.Euler(currentRotation.x, newYRotation % 360, currentRotation.z);
        }


        public void DisableButtonsWhileMoving()
        {
            _uiHelper.DisableButtons();
        }

        public void EnableButtons(Vector3 position)
        {
            _uiHelper.EnableButtons(position);
        }

        public void DestroyObject()
        {
            _uiHelper.DisableButtons();
            _uiHelper.SetSketchReference(null);
            Destroy(gameObject);
        }

        public bool ButtonsEnabled()
        {
            return _uiHelper.ButtonEnabled();
        }

        public bool GetRotatableStatus()
        {
            return _rotatable;
        }

        public void ConfirmPlacement()
        {
            OnPlacementConfirmed?.Invoke(transform.rotation);
            //DestroyObject();
        }

        public void CancelPlacement()
        {
            OnPlacementCancelled?.Invoke();
            DestroyObject();
        }
    }
}
