using UnityEngine;
using GizmosLibraryPlugin;

namespace UnityExplorer.TransformGizmos
{
    public class GlobalEulerAnglesGizmo : BaseTransformGizmo
    {
        public override bool CheckSelected(Ray ray, float maxDistanceToSelect)
        {
            return false;
        }

        public override bool IsSelected()
        {
            return false;
        }

        public override void OnRender()
        {
            GLHelper.DrawOnGlobalReference(() =>
            {
                //Global position axis
                Vector3 localFowardAxis = transform.forward;
                Vector3 localRightAxis = transform.right;

                //Local Euler Rotation
                Vector3 yRotationAxis = Vector3.Cross(Vector3.up, localFowardAxis);
                //if (yRotationAxis.ApproxEquals(Vector3.zero))
                if (Math.Abs(yRotationAxis.x) < 0.0001 && Math.Abs(yRotationAxis.y) < 0.0001 && Math.Abs(yRotationAxis.z) < 0.0001)
                {
                    yRotationAxis = localRightAxis;
                }

                GLDraw.WireframeCircle(1f, yRotationAxis, localFowardAxis, transform.position, Color.red, 16);
                GLDraw.WireframeCircle(1f, Vector3.up, Vector3.forward, transform.position, Color.yellow, 16);
                GLDraw.WireframeCircle(1f, localFowardAxis, transform.up, transform.position, Color.cyan, 16);
            });
        }

        public override void OnSelected(Ray ray)
        {
            return;
        }

        public override void Reset()
        {
            return;
        }
    }
}
