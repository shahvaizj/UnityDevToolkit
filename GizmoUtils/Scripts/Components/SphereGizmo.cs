using UnityEngine;

namespace ShahvaizJ.GizmoUtils
{
    public class SphereGizmo : MonoBehaviour
    {
        [SerializeField] private Color color = Color.cyan;
        [SerializeField] private float radius = 1f;
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField] private bool wireframe = false;
        [SerializeField] private bool onlyWhenSelected = false;

        private void OnDrawGizmos()         { if (!onlyWhenSelected) Draw(); }
        private void OnDrawGizmosSelected() { if  (onlyWhenSelected) Draw(); }

        private void Draw()
        {
            Gizmos.color = color;
            Vector3 center = transform.position + transform.TransformVector(offset);

            if (wireframe)
                Gizmos.DrawWireSphere(center, radius);
            else
                Gizmos.DrawSphere(center, radius);
        }
    }
}
