using UnityEngine;

namespace ShahvaizJ.GizmoUtils
{
    /// <summary>
    /// Draws directional arrows with a 4-point arrowhead.
    /// </summary>
    public static class GizmoArrow
    {
        private const float DefaultHeadLength = 0.2f;
        private const float DefaultHeadAngle  = 25f;

        /// <summary>Draws an arrow from <paramref name="from"/> to <paramref name="to"/>.</summary>
        public static void Draw(Vector3 from, Vector3 to, Color color,
            float headLength = DefaultHeadLength, float headAngle = DefaultHeadAngle)
        {
            Gizmos.color = color;
            Gizmos.DrawLine(from, to);
            DrawHead(to, (to - from).normalized, color, headLength, headAngle);
        }

        /// <summary>Draws an arrow starting at <paramref name="origin"/> pointing along <paramref name="direction"/>.</summary>
        public static void Draw(Vector3 origin, Vector3 direction, float length, Color color,
            float headLength = DefaultHeadLength, float headAngle = DefaultHeadAngle)
        {
            Draw(origin, origin + direction.normalized * length, color, headLength, headAngle);
        }

        /// <summary>Draws XYZ axes arrows at a transform's position and orientation.</summary>
        public static void DrawAxes(Vector3 position, Quaternion rotation, float length = 1f)
        {
            Draw(position, rotation * Vector3.right,   length, Color.red);
            Draw(position, rotation * Vector3.up,      length, Color.green);
            Draw(position, rotation * Vector3.forward, length, Color.blue);
        }

        private static void DrawHead(Vector3 tip, Vector3 dir, Color color, float headLength, float headAngle)
        {
            Gizmos.color = color;

            if (dir == Vector3.zero) return;

            Quaternion look = Quaternion.LookRotation(dir);

            Gizmos.DrawLine(tip, tip + look * Quaternion.Euler(0,  180 + headAngle, 0) * Vector3.forward * headLength);
            Gizmos.DrawLine(tip, tip + look * Quaternion.Euler(0,  180 - headAngle, 0) * Vector3.forward * headLength);
            Gizmos.DrawLine(tip, tip + look * Quaternion.Euler(180 + headAngle, 0, 0) * Vector3.forward * headLength);
            Gizmos.DrawLine(tip, tip + look * Quaternion.Euler(180 - headAngle, 0, 0) * Vector3.forward * headLength);
        }
    }
}
