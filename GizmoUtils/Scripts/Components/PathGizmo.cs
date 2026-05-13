using UnityEngine;

namespace ShahvaizJ.GizmoUtils
{
    public class PathGizmo : MonoBehaviour
    {
        [SerializeField] private Color pathColor = Color.white;
        [SerializeField] private Color nodeColor = Color.yellow;
        [SerializeField] private float nodeRadius = 0.1f;
        [SerializeField] private bool loop = false;
        [SerializeField] private bool wireframe = false;
        [SerializeField] private bool onlyWhenSelected = false;
        [SerializeField] private Transform[] waypoints = {};

        private void OnDrawGizmos()         { if (!onlyWhenSelected) Draw(); }
        private void OnDrawGizmosSelected() { if  (onlyWhenSelected) Draw(); }

        private void Draw()
        {
            if (waypoints == null || waypoints.Length < 2) return;

            for (int i = 0; i < waypoints.Length; i++)
            {
                if (waypoints[i] == null) continue;

                Gizmos.color = nodeColor;
                if (wireframe)
                    Gizmos.DrawWireSphere(waypoints[i].position, nodeRadius);
                else
                    Gizmos.DrawSphere(waypoints[i].position, nodeRadius);

                // Segment to next
                int next = (i + 1) % waypoints.Length;
                if (waypoints[next] == null) continue;
                if (i < waypoints.Length - 1 || loop)
                {
                    Gizmos.color = pathColor;
                    Gizmos.DrawLine(waypoints[i].position, waypoints[next].position);
                }
            }
        }
    }
}
