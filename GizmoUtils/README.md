# Gizmo Utilities

> Extended editor Gizmo helpers that make scene debugging and visualization faster and more expressive.

## Features

- Drop-on components for every common shape — filled by default, toggle wireframe in the Inspector
- Static shape library: circle, arc, sector, cone, capsule, cylinder, torus
- Directional arrows with arrowheads and XYZ axis helpers
- Composite helpers: labeled spheres, crosses, range rings, grids, paths, and live raycasts

---

## Folder Structure

```
GizmoUtils/
├── Scripts/
│   ├── Core/               — static utility classes, call from your own MonoBehaviours
│   │   ├── GizmoShapes.cs
│   │   ├── GizmoArrow.cs
│   │   └── GizmoUtils.cs
│   └── Components/         — drop-on MonoBehaviour components
│       ├── BoxGizmo.cs
│       ├── SphereGizmo.cs
│       ├── CircleGizmo.cs
│       ├── ConeGizmo.cs
│       ├── CapsuleGizmo.cs
│       ├── ArrowGizmo.cs
│       └── PathGizmo.cs
├── Scenes/
│   └── Example.unity
└── Editor/
```

---

## Components

Add any component to a GameObject. All components share three common fields:

| Field | Default | Description |
|---|---|---|
| `Color` | varies | Gizmo color |
| `Wireframe` | `false` | Filled when off, wireframe when on |
| `Only When Selected` | `false` | Draw always, or only when the object is selected |

### BoxGizmo
Draws a box aligned to the object's local axes.

| Field | Default |
|---|---|
| Size | (1, 1, 1) |
| Offset | (0, 0, 0) |

Filled uses `DrawCube`. Wireframe uses `DrawWireCube`.

---

### SphereGizmo
Draws a sphere at the object's position.

| Field | Default |
|---|---|
| Radius | 1 |
| Offset | (0, 0, 0) |

Filled uses `DrawSphere`. Wireframe uses `DrawWireSphere`.

---

### CircleGizmo
Draws a flat disc in any axis plane.

| Field | Default |
|---|---|
| Radius | 1 |
| Normal | Y |
| Offset | (0, 0, 0) |

Filled uses `Handles.DrawSolidDisc`. Wireframe uses a segmented line circle.

---

### ConeGizmo
Draws a cone along a local direction. Useful for FOV and spotlight ranges.

| Field | Default |
|---|---|
| Half Angle | 30° |
| Length | 3 |
| Local Direction | (0, 0, 1) |

Filled draws a solid base disc with four edge lines to the apex. Wireframe draws the full wire cone.

---

### CapsuleGizmo
Draws a capsule along a local axis.

| Field | Default |
|---|---|
| Height | 2 |
| Radius | 0.5 |
| Axis | Y |

Filled draws solid spheres at both caps with four connecting body lines. Wireframe draws the full wire capsule.

---

### ArrowGizmo
Draws a directional arrow along a local direction.

| Field | Default |
|---|---|
| Length | 2 |
| Head Length | 0.3 |
| Head Angle | 25° |
| Local Direction | (0, 0, 1) |

Filled adds a solid sphere at the arrowhead tip. Wireframe draws the shaft and four-point head only.

---

### PathGizmo
Draws a path through an array of Transform waypoints.

| Field | Default |
|---|---|
| Path Color | white |
| Node Color | yellow |
| Node Radius | 0.1 |
| Loop | false |
| Waypoints | empty |

Drag any GameObjects into the Waypoints array. Filled draws solid node spheres. Wireframe draws wire node spheres.

---

## Core Scripts (Static API)

Use these directly from your own `OnDrawGizmos` / `OnDrawGizmosSelected` methods.

### `GizmoShapes`

| Method | Description |
|---|---|
| `DrawWireCircle(center, radius, normal, color)` | Circle in any plane |
| `DrawWireArc(center, normal, from, angle, radius, color)` | Arc sweeping any angle |
| `DrawWireSector(center, normal, from, angle, radius, color)` | Pie slice (two radial edges + arc) |
| `DrawWireCone(origin, direction, halfAngle, length, color)` | Cone |
| `DrawWireCapsule(start, end, radius, color)` | Capsule between two points |
| `DrawWireCylinder(center, axis, height, radius, color)` | Cylinder along any axis |
| `DrawWireTorus(center, normal, outerRadius, innerRadius, color)` | Ring / donut |

### `GizmoArrow`

| Method | Description |
|---|---|
| `Draw(from, to, color)` | Arrow between two world positions |
| `Draw(origin, direction, length, color)` | Arrow from a point along a direction |
| `DrawAxes(position, rotation, length)` | RGB XYZ axis arrows |

### `GizmoUtils`

| Method | Description |
|---|---|
| `DrawLabeledSphere(position, radius, color, label)` | Wire sphere with a Scene-view label |
| `DrawCross(position, size, color)` | 3D cross marker on all three axes |
| `DrawPath(points, color, loop)` | Connected waypoint path |
| `DrawRangeRings(center, radii[], color)` | Concentric horizontal rings |
| `DrawRaycast(origin, direction, maxDist, hitColor, missColor)` | Live raycast with hit/miss coloring |
| `DrawBounds(bounds, color)` | Wireframe from a `Bounds` struct |
| `DrawGrid(center, columns, rows, cellSize, color)` | Flat XZ grid |

---

## Code Example

```csharp
using UnityEngine;
using ShahvaizJ.GizmoUtils;

public class EnemyAI : MonoBehaviour
{
    public float detectionRange = 5f;
    public float attackRange    = 1.5f;
    public float fovAngle       = 60f;

    private void OnDrawGizmosSelected()
    {
        // Labeled detection sphere
        GizmoUtils.DrawLabeledSphere(transform.position, detectionRange, Color.yellow, "Detection");

        // Attack range ring
        GizmoShapes.DrawWireCircle(transform.position, attackRange, Vector3.up, Color.red);

        // FOV sector
        GizmoShapes.DrawWireSector(transform.position, Vector3.up, transform.forward, fovAngle, detectionRange, Color.cyan);

        // Forward arrow
        GizmoArrow.Draw(transform.position, transform.forward, 2f, Color.blue);

        // Live raycast
        GizmoUtils.DrawRaycast(transform.position, transform.forward, detectionRange, Color.green, Color.red);
    }
}
```

---

## Requirements

- Unity 6 (6000.x) or newer
- No third-party dependencies

## License

MIT — free to use in personal and commercial projects.
