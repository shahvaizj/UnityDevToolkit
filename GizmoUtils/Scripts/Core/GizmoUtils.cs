using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace ShahvaizJ.GizmoUtils
{
    /// <summary>
    /// High-level composite Gizmo helpers built on top of GizmoShapes and GizmoArrow.
    /// </summary>
    public static class GizmoUtils
    {
        /// <summary>Draws a wire sphere with a Scene-view text label at its center.</summary>
        public static void DrawLabeledSphere(Vector3 position, float radius, Color color, string label)
        {
            Gizmos.color = color;
            Gizmos.DrawWireSphere(position, radius);
#if UNITY_EDITOR
            Handles.Label(position, label);
#endif
        }

        /// <summary>Draws a 3D cross marker (equal lines on all 3 axes).</summary>
        public static void DrawCross(Vector3 position, float size, Color color)
        {
            Gizmos.color = color;
            float h = size * 0.5f;
            Gizmos.DrawLine(position - Vector3.right   * h, position + Vector3.right   * h);
            Gizmos.DrawLine(position - Vector3.up      * h, position + Vector3.up      * h);
            Gizmos.DrawLine(position - Vector3.forward * h, position + Vector3.forward * h);
        }

        /// <summary>Draws a connected path through a series of points, optionally looping back to the start.</summary>
        public static void DrawPath(Vector3[] points, Color color, bool loop = false)
        {
            if (points == null || points.Length < 2) return;

            Gizmos.color = color;
            for (int i = 0; i < points.Length - 1; i++)
                Gizmos.DrawLine(points[i], points[i + 1]);

            if (loop)
                Gizmos.DrawLine(points[points.Length - 1], points[0]);
        }

        /// <summary>Draws concentric horizontal range rings at <paramref name="center"/>.</summary>
        public static void DrawRangeRings(Vector3 center, float[] radii, Color color)
        {
            foreach (float r in radii)
                GizmoShapes.DrawWireCircle(center, r, Vector3.up, color);
        }

        /// <summary>
        /// Performs a raycast and draws it in the Scene view.
        /// Hit segment is drawn in <paramref name="hitColor"/>; miss segment in <paramref name="missColor"/>.
        /// </summary>
        public static void DrawRaycast(Vector3 origin, Vector3 direction, float maxDistance, Color hitColor, Color missColor)
        {
            bool didHit = Physics.Raycast(origin, direction, out RaycastHit hit, maxDistance);

            if (didHit)
            {
                Gizmos.color = hitColor;
                Gizmos.DrawLine(origin, hit.point);
                Gizmos.DrawWireSphere(hit.point, 0.05f);

                Gizmos.color = new Color(missColor.r, missColor.g, missColor.b, 0.3f);
                Gizmos.DrawLine(hit.point, origin + direction.normalized * maxDistance);
            }
            else
            {
                Gizmos.color = missColor;
                Gizmos.DrawLine(origin, origin + direction.normalized * maxDistance);
            }
        }

        /// <summary>Draws a bounding box wireframe from a Bounds struct.</summary>
        public static void DrawBounds(Bounds bounds, Color color)
        {
            Gizmos.color = color;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        /// <summary>Draws a flat grid on the XZ plane centered at <paramref name="center"/>.</summary>
        public static void DrawGrid(Vector3 center, int columns, int rows, float cellSize, Color color)
        {
            Gizmos.color = color;

            float totalWidth  = columns * cellSize;
            float totalHeight = rows    * cellSize;
            Vector3 origin = center - new Vector3(totalWidth * 0.5f, 0f, totalHeight * 0.5f);

            for (int x = 0; x <= columns; x++)
            {
                Vector3 from = origin + Vector3.right * (x * cellSize);
                Gizmos.DrawLine(from, from + Vector3.forward * totalHeight);
            }

            for (int z = 0; z <= rows; z++)
            {
                Vector3 from = origin + Vector3.forward * (z * cellSize);
                Gizmos.DrawLine(from, from + Vector3.right * totalWidth);
            }
        }
    }
}
