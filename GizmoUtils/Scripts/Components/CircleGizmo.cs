using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ShahvaizJ.GizmoUtils
{
    public class CircleGizmo : MonoBehaviour
    {
        public enum Axis { X, Y, Z }

        [SerializeField] private Color color = Color.white;
        [SerializeField] private float radius = 1f;
        [SerializeField] private Axis normal = Axis.Y;
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField] private bool wireframe = false;
        [SerializeField] private bool onlyWhenSelected = false;

        private void OnDrawGizmos()         { if (!onlyWhenSelected) Draw(); }
        private void OnDrawGizmosSelected() { if  (onlyWhenSelected) Draw(); }

        private void Draw()
        {
            Vector3 localNormal = normal switch
            {
                Axis.X => Vector3.right,
                Axis.Z => Vector3.forward,
                _      => Vector3.up,
            };

            Vector3 worldNormal = transform.TransformDirection(localNormal);
            Vector3 worldCenter = transform.position + transform.TransformVector(offset);

            if (wireframe)
            {
                GizmoShapes.DrawWireCircle(worldCenter, radius, worldNormal, color);
            }
            else
            {
#if UNITY_EDITOR
                Handles.color = color;
                Handles.DrawSolidDisc(worldCenter, worldNormal, radius);
#endif
            }
        }
    }
}
