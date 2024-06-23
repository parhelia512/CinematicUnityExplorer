// Credits to https://github.com/Vesper-Works/Unity-Explorer-For-Outer-Wilds/
using System.Collections.Generic;
using UnityEngine;

using GizmosLibraryPlugin;
using UnityExplorer.Inspectors;
using UnityExplorer.TransformGizmos;

using UniverseLib.Input;

#if INTEROP
using Il2CppInterop.Runtime.Injection;
#endif

namespace UnityExplorer
{
    public enum GizmoType
    {
        None,
        LocalPosition,
        LocalRotation,
        GlobalPosition,
        GlobalRotation
    }

    public class ExtendedTransformTools : MonoBehaviour
    {
#if CPP
        static ExtendedTransformTools()
        {
            ClassInjector.RegisterTypeInIl2Cpp<ExtendedTransformTools>();
        }

        public ExtendedTransformTools(IntPtr ptr) : base(ptr) { }
#endif
        public Transform targetTransform;

        float maxDistanceToSelect = 0.05f;

        private List<BaseTransformGizmo> EnabledGizmos = new();

        private List<BaseTransformGizmo> NewGizmo(GizmoType type){
            List<BaseTransformGizmo> returnList = new List<BaseTransformGizmo>();
            switch(type){
                case GizmoType.LocalPosition:
                    returnList.Add(new LocalPositionGizmo());
                    break;
                case GizmoType.LocalRotation:
                    returnList.Add(new LocalEulerAngleGizmo());
                    break;
                case GizmoType.GlobalPosition:
                    returnList.Add(new TransformOrientationGizmo());
                    break;
                case GizmoType.GlobalRotation:
                    returnList.Add(new LocalEulerAngleGizmo());
                    returnList.Add(new FromCameraViewRotationGizmo());
                    break;
            }
            return returnList;
        }

        public void ChangeGizmo(GizmoType type) {
            EnabledGizmos.Clear();
            if (type != GizmoType.None){
                EnabledGizmos.AddRange(NewGizmo(type));
            }
        }

        private void Update()
        {
            if (targetTransform == null){
                EnabledGizmos.ForEach((gizmo) => gizmo.Reset());
                return;
            }

            float scale = Vector3.Distance(Camera.current.transform.position, targetTransform.position) / 5f;

            EnabledGizmos.ForEach((gizmo) =>
            {
                gizmo.SetScale(scale);
                gizmo.Set(targetTransform);
            });

            Ray ray = Camera.current.ScreenPointToRay(IInputManager.MousePosition);

            if (IInputManager.GetMouseButtonDown(0))
            {
                bool noneIsSelected = EnabledGizmos.TrueForAll((gizmo) => !gizmo.IsSelected());

                if (noneIsSelected)
                {
                    EnabledGizmos.ForEach((gizmo) => gizmo.CheckSelected(ray, maxDistanceToSelect * scale));
                }
            }
            else if (IInputManager.GetMouseButton(0))
            {
                EnabledGizmos.ForEach((gizmo) =>
                {
                    if (gizmo.IsSelected())
                    {
                        gizmo.OnSelected(ray);
                    }
                });
            }
            else
            {
                EnabledGizmos.ForEach((gizmo) => gizmo.Reset());
            }

        }

        public void OnRenderObject()
        {
            if (targetTransform == null)
                return;

            GL.wireframe = true;

            GLHelper.SetDefaultMaterialPass(0, true);

            EnabledGizmos.ForEach((gizmo) => gizmo.OnRender());

            GL.wireframe = false;
        }
    }
}
