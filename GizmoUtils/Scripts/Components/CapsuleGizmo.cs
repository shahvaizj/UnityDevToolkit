using UnityEngine;

namespace ShahvaizJ.GizmoUtils
{
    public class CapsuleGizmo : MonoBehaviour
    {
        public enum Axis { X, Y, Z }

        [SerializeField] private Color color = Color.magenta;
        [SerializeField] private float height = 2f;
        [SerializeField] private float radius = 0.5f;
        [SerializeField] private Axis axis = Axis.Y;
        [SerializeField] private bool wireframe = false;
        [SerializeField] private bool onlyWhenSelected = false;

        private void OnDrawGizmos()         { if (!onlyWhenSelected) Draw(); }
        private void OnDrawGizmosSelected() { if  (onlyWhenSelected) Draw(); }

        private void Draw()
        {
            Vector3 localAxis = axis switch
            {
                Axis.X => Vector3.right,
                Axis.Z => Vector3.forward,
                _      => Vector3.up,
            };

            Vector3 worldAxis = transform.TransformDirection(localAxis);
            float   bodyHalf  = Mathf.Max(0f, height * 0.5f - radius);
            Vector3 top       = transform.position + worldAxis *  bodyHalf;
            Vector3 bottom    = transform.position + worldAxis * -bodyHalf;

            Gizmos.color = color;

            if (wireframe)
            {
                GizmoShapes.DrawWireCapsule(bottom, top, radius, color);
            }
            else
            {
                Gizmos.DrawSphere(top,    radius);
                Gizmos.DrawSphere(bottom, radius);

                Vector3 right = Vector3.Cross(worldAxis, Vector3.up).normalized;
                if (right.sqrMagnitude < 0.001f)
                    right = Vector3.Cross(worldAxis, Vector3.forward).normalized;
                Vector3 fwd = Vector3.Cross(worldAxis, right);

                Gizmos.DrawLine(top + right * radius, bottom + right * radius);
                Gizmos.DrawLine(top - right * radius, bottom - right * radius);
                Gizmos.DrawLine(top + fwd   * radius, bottom + fwd   * radius);
                Gizmos.DrawLine(top - fwd   * radius, bottom - fwd   * radius);
            }
        }
    }
}
