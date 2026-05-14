# 🛠️ Unity Developer Toolkit

> A curated collection of reusable Unity tools, components, and utilities — built over years of real game development and open-sourced for the community.

---

## What Is This?

This repository is a growing library of production-ready Unity tools covering debugging, visualization, and common gameplay systems. Every tool here was born out of an actual need during development — nothing is included just to fill space.

Whether you're a solo indie developer racing against time or a studio looking for solid, drop-in utilities, this toolkit is built to save you hours.

---

## ✨ What's Included

### 📐 Gizmo Utilities
Drop-on components and a static helper library for scene debugging and visualization. Shapes are filled by default with a wireframe toggle in the Inspector.

- Components for every common shape: box, sphere, circle, cone, capsule, arrow, path
- Filled by default — toggle wireframe per component without touching code
- Static shape library: wire circle, arc, sector, cone, capsule, cylinder, torus
- Composite helpers: labeled spheres, range rings, grids, live raycasts, waypoint paths

### 🪵 Custom Log
A lightweight conditional debug logger with colored console output. All calls are automatically stripped from non-development builds.

- Plain, warning, and error logs wrapping Unity's `Debug` class
- Colored variants: Blue, Red, Yellow, Green, Cyan, Orange
- All calls stripped in release builds via `[Conditional]`
- Optional context object to ping GameObjects from the console

---

## 🚀 Getting Started

### Installation

**Option 1 — Unity Package Manager (Git URL)**

In Unity: `Window → Package Manager → + → Add package from git URL`

```
https://github.com/shahvaizj/UnityDevToolkit.git
```

**Option 2 — Manual**

Clone or download this repository and drop the tool folder(s) you need into your project's `Assets/` directory.

```bash
git clone https://github.com/shahvaizj/UnityDevToolkit.git
```

### Requirements

- Unity **6000.0** or newer
- No third-party dependencies required

---

## 📖 Usage Examples

### Gizmo Utilities
```csharp
using ShahvaizJ.GizmoUtils;

private void OnDrawGizmosSelected()
{
    GizmoUtils.DrawLabeledSphere(transform.position, 5f, Color.yellow, "Detection");
    GizmoShapes.DrawWireSector(transform.position, Vector3.up, transform.forward, 60f, 5f, Color.cyan);
    GizmoArrow.Draw(transform.position, transform.forward, 2f, Color.blue);
}
```

### Custom Log
```csharp
using ShahvaizJ.CustomLog;

Log.Print("hello");
Log.Warning("watch out");
Log.Error("something broke");

Log.Blue("spawned");
Log.Green("health full");
Log.Yellow("low ammo");
Log.Red("took damage");
```

---

## 📜 License

This project is licensed under the **MIT License** — free to use in personal and commercial projects. See [LICENSE](LICENSE) for full terms.

---

## 👤 Author

Made by **Shahvaiz** — indie game developer, currently building [*You May Never Leave*](https://shahvaizj.itch.io/you-may-never-leave), a paranormal sci-fi horror puzzle game.

If this toolkit saves you time, a ⭐ on the repo goes a long way.

---

*Built from real projects. Shared for everyone.*
