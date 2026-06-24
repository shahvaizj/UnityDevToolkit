# NotificationToast

Queue-based toast notification system with severity levels, slide animations, and per-severity styling.

## Features

- Four severity levels — Info, Success, Warning, Error — each with configurable background color, text color, and icon
- Smooth slide-in/slide-out animation from any direction (top, bottom, left, right) with cubic easing
- Automatic queue management — notifications display one at a time; extras queue up in order
- Configurable timing — independent fade-in, display, and fade-out durations
- Unscaled time support — toasts animate even while the game is paused
- `OnToastShown` and `OnToastDismissed` UnityEvents for wiring gameplay responses

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/UnityDevToolkit.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Create a **Canvas** with a toast panel (an Image + TextMeshProUGUI child) positioned where toasts should appear.
2. Add a **CanvasGroup** component to the toast panel.
3. Add the **NotificationToast** component to any GameObject in the scene.
4. Drag the toast panel's **RectTransform**, **CanvasGroup**, **Image** (background), and **TextMeshProUGUI** (message) into the corresponding fields.
5. Optionally assign an **Icon Image** and configure per-severity sprites.
6. Configure **Slide Direction**, **Slide Distance**, and timing values in the Inspector.
7. Call `Show()` from code or wire it to a UnityEvent.

### Scripting API

```csharp
using ShahvaizJ.NotificationToast;

// Show with default duration
NotificationToast.Instance.Show("Item collected!", ToastSeverity.Success);

// Show with custom duration
NotificationToast.Instance.Show("Connection lost", ToastSeverity.Error, 5f);

// Dismiss current toast and advance the queue
NotificationToast.Instance.DismissCurrent();

// Dismiss everything
NotificationToast.Instance.DismissAll();

// Read state
bool visible = NotificationToast.Instance.IsShowing;
int pending  = NotificationToast.Instance.QueueCount;
```

## License

MIT — see [LICENSE](LICENSE).
