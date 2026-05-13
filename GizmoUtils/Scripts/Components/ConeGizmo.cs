using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ShahvaizJ.GizmoUtils
{
    public class ConeGizmo : MonoBehaviour
    {
        [SerializeField] private Color color = Color.yellow;
        [SerializeField] private float halfAngle = 30f;
        [SerializeField] private float length = 3f;
        [SerializeField] private Vector3 localDirection = Vector3.forward;
        [SerializeField] private bool wireframe = false;
        [SerializeField] private bool onlyWhenSelected = false;

        private void OnDrawGizmos()         { if (!onlyWhenSelected) Draw(); }
        private void OnDrawGizmosSelected() { if  (onlyWhenSelected) Draw(); }

        private void Draw()
        {
            Vector3 dir        = transform.TransformDirection(localDirection.normalized);
            Vector3 apex       = transform.position;
            Vector3 baseCenter = apex + dir * length;
            float   baseRadius = length * Mathf.Tan(halfAngle * Mathf.Deg2Rad);

            if (wireframe)
            {
                GizmoShapes.DrawWireCone(apex, dir, halfAngle, length, color);
            }
            else
            {
#if UNITY_EDITOR
                Handles.color = color;
                Handles.DrawSolidDisc(baseCenter, dir, baseRadius);
#endif
                Gizmos.color = color;
                Vector3 right = Vector3.Cross(dir, Vector3.up).normalized;
                if (right.sqrMagnitude < 0.001f)
                    right = Vector3.Cross(dir, Vector3.right).normalized;
                Vector3 up = Vector3.Cross(right, dir);

                Gizmos.DrawLine(apex, baseCenter + right * baseRadius);
                Gizmos.DrawLine(apex, baseCenter - right * baseRadius);
                Gizmos.DrawLine(apex, baseCenter + up    * baseRadius);
                Gizmos.DrawLine(apex, baseCenter - up    * baseRadius);
            }
        }
    }
}
