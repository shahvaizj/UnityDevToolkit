using UnityEngine;

namespace ShahvaizJ.GizmoUtils
{
    /// <summary>
    /// Primitive wire shape drawers. All methods set Gizmos.color before drawing.
    /// </summary>
    public static class GizmoShapes
    {
        private const int DefaultSegments = 32;

        /// <summary>Draws a wire circle in any plane defined by <paramref name="normal"/>.</summary>
        public static void DrawWireCircle(Vector3 center, float radius, Vector3 normal, Color color, int segments = DefaultSegments)
        {
            Gizmos.color = color;

            Vector3 tangent = Vector3.Cross(normal, Vector3.up).normalized;
            if (tangent.sqrMagnitude < 0.001f)
                tangent = Vector3.Cross(normal, Vector3.right).normalized;

            Vector3 prev = center + tangent * radius;
            for (int i = 1; i <= segments; i++)
            {
                float angle = i * 360f / segments;
                Vector3 next = center + Quaternion.AngleAxis(angle, normal) * tangent * radius;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }

        /// <summary>Draws a wire arc from <paramref name="from"/> sweeping <paramref name="angle"/> degrees around <paramref name="normal"/>.</summary>
        public static void DrawWireArc(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, Color color, int segments = DefaultSegments)
        {
            Gizmos.color = color;

            Vector3 dir = from.normalized * radius;
            int steps = Mathf.Max(2, Mathf.RoundToInt(segments * Mathf.Abs(angle) / 360f));
            Vector3 prev = center + dir;

            for (int i = 1; i <= steps; i++)
            {
                float t = angle * i / steps;
                Vector3 next = center + Quaternion.AngleAxis(t, normal) * dir;
                Gizmos.DrawLine(prev, next);
                prev = next;
            }
        }

        /// <summary>Draws a filled wire sector (pie slice) — two radial lines and an arc.</summary>
        public static void DrawWireSector(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius, Color color, int segments = DefaultSegments)
        {
            Gizmos.color = color;

            Vector3 dir    = from.normalized * radius;
            Vector3 arcEnd = Quaternion.AngleAxis(angle, normal) * dir;

            Gizmos.DrawLine(center, center + dir);
            Gizmos.DrawLine(center, center + arcEnd);
            DrawWireArc(center, normal, from, angle, radius, color, segments);
        }

        /// <summary>Draws a wire cone. Useful for FOV and spotlight visualization.</summary>
        public static void DrawWireCone(Vector3 origin, Vector3 direction, float halfAngle, float length, Color color, int segments = DefaultSegments)
        {
            Gizmos.color = color;

            direction = direction.normalized;
            float radius = length * Mathf.Tan(halfAngle * Mathf.Deg2Rad);
            Vector3 tip = origin + direction * length;

            Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
            if (right.sqrMagnitude < 0.001f)
                right = Vector3.Cross(direction, Vector3.right).normalized;
            Vector3 up = Vector3.Cross(right, direction);

            DrawWireCircle(tip, radius, direction, color, segments);

            Gizmos.DrawLine(origin, tip + right * radius);
            Gizmos.DrawLine(origin, tip - right * radius);
            Gizmos.DrawLine(origin, tip + up    * radius);
            Gizmos.DrawLine(origin, tip - up    * radius);
        }

        /// <summary>Draws a wire capsule between two end-sphere centers.</summary>
        public static void DrawWireCapsule(Vector3 start, Vector3 end, float radius, Color color, int segments = DefaultSegments)
        {
            Gizmos.color = color;

            Vector3 axis  = (end - start).normalized;
            Vector3 right = Vector3.Cross(axis, Vector3.up).normalized;
            if (right.sqrMagnitude < 0.001f)
                right = Vector3.Cross(axis, Vector3.forward).normalized;
            Vector3 fwd = Vector3.Cross(axis, right);

            // Body lines
            Gizmos.DrawLine(start + right * radius, end + right * radius);
            Gizmos.DrawLine(start - right * radius, end - right * radius);
            Gizmos.DrawLine(start + fwd   * radius, end + fwd   * radius);
            Gizmos.DrawLine(start - fwd   * radius, end - fwd   * radius);

            // End cap circles
            DrawWireCircle(start, radius, axis, color, segments);
            DrawWireCircle(end,   radius, axis, color, segments);

            // Hemisphere arcs
            int half = segments / 2;
            DrawWireArc(start, right, -axis, 180f, radius, color, half);
            DrawWireArc(start, fwd,   -axis, 180f, radius, color, half);
            DrawWireArc(end,   right,  axis, 180f, radius, color, half);
            DrawWireArc(end,   fwd,    axis, 180f, radius, color, half);
        }

        /// <summary>Draws a wire cylinder along <paramref name="axis"/>.</summary>
        public static void DrawWireCylinder(Vector3 center, Vector3 axis, float height, float radius, Color color, int segments = DefaultSegments)
        {
            Gizmos.color = color;

            axis = axis.normalized;
            Vector3 top    = center + axis * (height * 0.5f);
            Vector3 bottom = center - axis * (height * 0.5f);

            Vector3 right = Vector3.Cross(axis, Vector3.up).normalized;
            if (right.sqrMagnitude < 0.001f)
                right = Vector3.Cross(axis, Vector3.forward).normalized;
            Vector3 fwd = Vector3.Cross(axis, right);

            DrawWireCircle(top,    radius, axis, color, segments);
            DrawWireCircle(bottom, radius, axis, color, segments);

            Gizmos.DrawLine(top + right * radius, bottom + right * radius);
            Gizmos.DrawLine(top - right * radius, bottom - right * radius);
            Gizmos.DrawLine(top + fwd   * radius, bottom + fwd   * radius);
            Gizmos.DrawLine(top - fwd   * radius, bottom - fwd   * radius);
        }

        /// <summary>Draws a flat wire torus (ring) lying on the XZ plane at <paramref name="center"/>.</summary>
        public static void DrawWireTorus(Vector3 center, Vector3 normal, float outerRadius, float innerRadius, Color color, int segments = DefaultSegments)
        {
            DrawWireCircle(center, outerRadius, normal, color, segments);
            DrawWireCircle(center, innerRadius, normal, color, segments);

            Vector3 tangent = Vector3.Cross(normal, Vector3.up).normalized;
            if (tangent.sqrMagnitude < 0.001f)
                tangent = Vector3.Cross(normal, Vector3.right).normalized;

            float tubeRadius = (outerRadius - innerRadius) * 0.5f;
            float midRadius  = innerRadius + tubeRadius;
            int spokes = 8;
            for (int i = 0; i < spokes; i++)
            {
                float angle = i * 360f / spokes;
                Vector3 spokeDir  = Quaternion.AngleAxis(angle, normal) * tangent;
                Vector3 spokeCenter = center + spokeDir * midRadius;
                DrawWireCircle(spokeCenter, tubeRadius, spokeDir, color, segments / 4);
            }
        }
    }
}
