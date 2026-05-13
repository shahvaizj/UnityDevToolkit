using UnityEngine;

namespace ShahvaizJ.GizmoUtils
{
    public class BoxGizmo : MonoBehaviour
    {
        [SerializeField] private Color color = Color.green;
        [SerializeField] private Vector3 size = Vector3.one;
        [SerializeField] private Vector3 offset = Vector3.zero;
        [SerializeField] private bool wireframe = false;
        [SerializeField] private bool onlyWhenSelected = false;

        private void OnDrawGizmos()         { if (!onlyWhenSelected) Draw(); }
        private void OnDrawGizmosSelected() { if  (onlyWhenSelected) Draw(); }

        private void Draw()
        {
            Gizmos.color = color;
            Matrix4x4 prev = Gizmos.matrix;
            Gizmos.matrix = transform.localToWorldMatrix;

            if (wireframe)
                Gizmos.DrawWireCube(offset, size);
            else
                Gizmos.DrawCube(offset, size);

            Gizmos.matrix = prev;
        }
    }
}
