# TooltipSystem

Hover and long-press tooltips with smart screen-edge repositioning for Unity UI.

## Features

- Automatic screen-edge clamping — tooltip pivots and repositions to stay fully visible
- Configurable hover delay before the tooltip appears
- Long-press support for touch devices with movement tolerance
- Smooth fade-in/fade-out transitions with configurable durations
- Unscaled time support — tooltips work while the game is paused
- Pointer-following mode — tooltip tracks the cursor in real-time
- Drop-in `TooltipTrigger` component for per-element hover text
- `OnTooltipShown` and `OnTooltipHidden` UnityEvents

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/UnityDevToolkit.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Create a **Canvas** with a tooltip panel (an Image + TextMeshProUGUI child) — style it as your tooltip background.
2. Add a **CanvasGroup** component to the tooltip panel.
3. Add the **TooltipSystem** component to any GameObject in the scene.
4. Drag the tooltip panel's **RectTransform**, **CanvasGroup**, **Image** (background), and **TextMeshProUGUI** (message) into the corresponding fields.
5. Add a **TooltipTrigger** component to each UI element that should show a tooltip.
6. Set the **Tooltip Text** on each trigger. Optionally enable **Use Long Press** for touch input.

### Scripting API

```csharp
using ShahvaizJ.TooltipSystem;

// Show at pointer position (follows cursor)
TooltipSystem.Instance.Show("Equip this item");

// Show at a fixed screen position
TooltipSystem.Instance.Show("Health potion", new Vector2(400, 300));

// Hide with fade-out
TooltipSystem.Instance.Hide();

// Hide immediately (no fade)
TooltipSystem.Instance.HideImmediate();

// Read state
bool visible = TooltipSystem.Instance.IsVisible;
string text  = TooltipSystem.Instance.CurrentText;

// Change trigger text at runtime
GetComponent<TooltipTrigger>().TooltipText = "New description";
```

## License

MIT — see [LICENSE](LICENSE).
