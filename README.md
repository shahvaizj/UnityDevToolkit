# 🛠️ Unity Developer Toolkit

> A curated collection of reusable Unity tools, components, and utilities — built over years of real game development and open-sourced for the community.

---

## What Is This?

This repository is a growing library of production-ready Unity tools covering animation, UI, visual feedback, and common gameplay systems. Every tool here was born out of an actual need during development — nothing is included just to fill space.

Whether you're a solo indie developer racing against time or a studio looking for solid, drop-in utilities, this toolkit is built to save you hours.

---

## ✨ What's Included

### 🎞️ Tween System
Lightweight, code-driven tweening without the overhead of a full-blown library. Animate values, positions, colors, and more with clean, chainable syntax.

- Smooth interpolation for position, rotation, scale, alpha, and color
- Easing functions (linear, ease-in, ease-out, bounce, elastic, and more)
- Loop and ping-pong support
- Callback hooks on start, update, and complete

### 📐 Gizmo Utilities
Drop-on components and a static helper library for scene debugging and visualization. Shapes are filled by default with a wireframe toggle in the Inspector.

- Components for every common shape: box, sphere, circle, cone, capsule, arrow, path
- Filled by default — toggle wireframe per component without touching code
- Static shape library: wire circle, arc, sector, cone, capsule, cylinder, torus
- Composite helpers: labeled spheres, range rings, grids, live raycasts, waypoint paths

### 📦 Scale Animations
Simple, inspector-friendly scale animation components for punchy, responsive UI and world-space feedback.

- Pop-in / pop-out animations
- Bounce-on-enable with configurable curves
- Looping idle scale pulses
- One-call animation triggers from code or UnityEvents

### 🖥️ UI Components
Custom UI components that extend Unity's built-in UI toolkit with practical, game-ready features.

- Typewriter text effect with configurable speed and callback support
- Scroll snap (snapping scroll rect for menus and card carousels)
- Animated progress bars with easing
- Safe area panel (handles notches and device insets correctly)
- Tooltip system with auto-positioning

### Custom Log
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
https://github.com/shahvaizj/unity-developer-toolkit.git
```

**Option 2 — Manual**

Clone or download this repository and drop the contents into your project's `Assets/` folder.

```bash
git clone https://github.com/shahvaizj/unity-developer-toolkit.git
```

### Requirements

- Unity **2021.3 LTS** or newer
- No third-party dependencies required

---

## 📖 Usage Examples

### Tween
```csharp
// Move an object over 0.5 seconds with ease-out
transform.TweenPosition(targetPos, 0.5f, Ease.OutQuad)
         .OnComplete(() => Debug.Log("Done!"));
```

### Scale Animation
```csharp
// Trigger a pop animation on a UI element
myPanel.GetComponent<ScaleAnimation>().Play();
```

### Gizmo Utilities
```csharp
// Drop-on component — no code needed, configure in Inspector
// Add BoxGizmo, SphereGizmo, ConeGizmo, etc. to any GameObject

// Or use the static API from your own MonoBehaviour:
using ShahvaizJ.GizmoUtils;

private void OnDrawGizmosSelected()
{
    GizmoUtils.DrawLabeledSphere(transform.position, 5f, Color.yellow, "Detection");
    GizmoShapes.DrawWireSector(transform.position, Vector3.up, transform.forward, 60f, 5f, Color.cyan);
    GizmoArrow.Draw(transform.position, transform.forward, 2f, Color.blue);
}
```

---

## 📁 Project Structure

```
Assets/
└── DevToolkit/
    ├── Tweening/
    │   ├── Core/
    │   └── Easings/
    ├── GizmoUtils/
    │   ├── Scripts/
    │   │   ├── Core/          (GizmoShapes, GizmoArrow, GizmoUtils)
    │   │   └── Components/    (BoxGizmo, SphereGizmo, ConeGizmo, …)
    │   └── Scenes/
    ├── Animations/
    │   └── ScaleAnimation/
    └── UI/
        ├── TypewriterText/
        ├── ScrollSnap/
        ├── ProgressBar/
        ├── SafeAreaPanel/
        └── Tooltip/
```

---

## 🗺️ Roadmap

- [ ] Audio utilities (fade in/out, layered ambience manager)
- [ ] Object pooling system
- [ ] Scene transition framework
- [ ] Additional easing curves and custom curve editor
- [ ] Full documentation site

Have a tool you'd like to see? Open an issue — suggestions are always welcome.

---

## 🤝 Contributing

Contributions are welcome. If you've built something that belongs here, please:

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-tool-name`)
3. Keep code clean, commented, and self-contained
4. Submit a pull request with a brief description of what the tool does and why it's useful

Please follow the existing code style. One tool per PR where possible.

---

## 📜 License

This project is licensed under the **MIT License** — free to use in personal and commercial projects. See [LICENSE](LICENSE) for full terms.

---

## 👤 Author

Made by **Shahvaiz** — indie game developer, currently building [*You May Never Leave*](https://shahvaizj.itch.io/you-may-never-leave), a paranormal sci-fi horror puzzle game.

If this toolkit saves you time, a ⭐ on the repo goes a long way.

---

*Built from real projects. Shared for everyone.*
