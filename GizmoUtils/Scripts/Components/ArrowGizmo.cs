using UnityEngine;

namespace ShahvaizJ.GizmoUtils
{
    public class ArrowGizmo : MonoBehaviour
    {
        [SerializeField] private Color color = Color.yellow;
        [SerializeField] private float length = 2f;
        [SerializeField] private float headLength = 0.3f;
        [SerializeField] private float headAngle = 25f;
        [SerializeField] private Vector3 localDirection = Vector3.forward;
        [SerializeField] private bool wireframe = false;
        [SerializeField] private bool onlyWhenSelected = false;

        private void OnDrawGizmos()         { if (!onlyWhenSelected) Draw(); }
        private void OnDrawGizmosSelected() { if  (onlyWhenSelected) Draw(); }

        private void Draw()
        {
            Vector3 dir = transform.TransformDirection(localDirection.normalized);
            GizmoArrow.Draw(transform.position, dir, length, color, headLength, headAngle);

            if (!wireframe)
            {
                Gizmos.color = color;
                Gizmos.DrawSphere(transform.position + dir * length, headLength * 0.4f);
            }
        }
    }
}
