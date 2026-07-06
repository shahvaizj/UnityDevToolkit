# Confirmation Dialog

A reusable modal yes/no confirmation popup that dispatches its result to a callback and to UnityEvents.

## Features

- Single `Show(...)` call opens the dialog with a title, message, and result callback
- `Action<bool>` callback fires `true` on confirm, `false` on cancel — no polling required
- `OnOpened`, `OnClosed`, `OnConfirmed`, `OnCancelled` UnityEvents for Inspector-driven wiring
- Per-call override of confirm/cancel button labels (e.g. "Delete" / "Keep")
- Static `Instance` for easy global access from any script
- `Close()` to dismiss programmatically without triggering a confirm/cancel result

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/UnityDevToolkit.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Build a dialog panel on a Canvas: a background/backdrop, a title text, a message text, and Confirm/Cancel buttons. Attach the **Confirmation Dialog** component to the panel's root GameObject (it requires a `CanvasGroup`, added automatically).
2. Assign **Title Label**, **Message Label**, **Confirm Button**, and **Cancel Button** in the Inspector. Optionally assign **Confirm Button Label** / **Cancel Button Label** if you want per-call text overrides.
3. Set the **Default Title**, **Default Confirm Text**, and **Default Cancel Text** fields.
4. Leave **Hidden On Awake** checked so the dialog starts closed.
5. Call `Show(...)` from any script (via `ConfirmationDialog.Instance`) to open the dialog and receive the result in your callback.

### Scripting API

```csharp
using ShahvaizJ.ConfirmationDialog;

// Simple usage with the default title
ConfirmationDialog.Instance.Show("Delete this save file?", confirmed =>
{
    if (confirmed)
        DeleteSaveFile();
});

// Custom title and per-call button labels
ConfirmationDialog.Instance.Show(
    title: "Quit Game",
    message: "Any unsaved progress will be lost.",
    onResult: confirmed =>
    {
        if (confirmed)
            Application.Quit();
    },
    confirmText: "Quit",
    cancelText: "Stay");

// Dismiss without firing a result (e.g. the action became unavailable)
ConfirmationDialog.Instance.Close();

// Query state
if (ConfirmationDialog.Instance.IsOpen)
    Debug.Log("A confirmation is currently pending.");
```

## License

MIT — see [LICENSE](LICENSE).
