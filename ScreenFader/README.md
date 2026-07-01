# Screen Fader

A full-screen fade-to-color overlay for cutscenes, scene transitions, and death sequences, with async-friendly callbacks and unscaled time support.

## Features

- `FadeOut` / `FadeIn` transitions with optional `Action` callback — drop-in to any coroutine or async pipeline
- `FadeOutHoldIn` convenience method: fade out, hold the black screen, then fade back in
- AnimationCurve-driven easing for custom fade shapes (ease-in, ease-out, linear, or any custom shape)
- Configurable fade color — black, white, or any `Color`
- Unscaled time support — fades animate even while `Time.timeScale = 0`
- `SnapOpaque` / `SnapTransparent` for instant transitions without animation
- Static `Instance` for fire-and-forget calls from any script
- `OnFadeOutComplete` and `OnFadeInComplete` UnityEvents for Inspector wiring
- `IsFading`, `IsOpaque`, `IsTransparent`, and `Alpha` state properties

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/UnityDevToolkit.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

### Inspector Setup

1. Create a new empty GameObject, name it `ScreenFader`.
2. Add the **Screen Fader** component (`ShahvaizJ/Screen Fader` in the Add Component menu). This automatically adds a `Canvas` and `CanvasGroup`.
3. Set the Canvas **Render Mode** to **Screen Space - Overlay** and **Sort Order** to `32767` (the component does this automatically on Awake, but set it in the Inspector too so it is visible in the Editor).
4. Add a child `Image` that fills the entire Canvas (use **Rect Transform → Stretch** → anchor min (0,0), anchor max (1,1), offsets all 0). Set its color to your fade color (e.g. black) — the alpha value is ignored at runtime.
5. Assign that Image to the **Overlay** field on the Screen Fader component.
6. Set **Fade Color** to match the Image color (black by default).
7. Optionally wire **OnFadeOutComplete** and **OnFadeInComplete** events in the Inspector.

### Scripting API

```csharp
using ShahvaizJ.ScreenFader;

// --- Basic fade-out then scene load ---
ScreenFader.Instance.FadeOut(onComplete: () =>
{
    SceneManager.LoadScene("NextScene");
});

// --- Fade in when a scene starts ---
void Start()
{
    ScreenFader.Instance.SnapOpaque();          // start fully black
    ScreenFader.Instance.FadeIn(duration: 1f); // reveal the scene
}

// --- Fade out, hold for 0.5 s, fade in (e.g. death respawn) ---
ScreenFader.Instance.FadeOutHoldIn(holdDuration: 0.5f, onComplete: () =>
{
    Debug.Log("Respawn complete");
});

// --- Custom duration and color ---
ScreenFader.Instance.SetFadeColor(Color.white);
ScreenFader.Instance.FadeOut(duration: 0.3f);

// --- State queries ---
if (ScreenFader.Instance.IsFading)
    Debug.Log("Fade in progress");

if (ScreenFader.Instance.IsOpaque)
    Debug.Log("Screen is fully covered");
```

## License

MIT — see [LICENSE](LICENSE).
