# Radial Progress Bar

A radial progress bar component with 0–1 fill API, color gradient, and smooth transitions.

## Features

- Image fill-amount driven radial gauge with simple 0–1 API
- Gradient color that shifts automatically as progress changes
- Optional smooth animated transitions with configurable speed
- Configurable fill method, origin, and direction (clockwise / counter-clockwise)
- `OnProgressChanged`, `OnFillComplete`, and `OnFillReset` UnityEvents

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/UnityDevToolkit.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Create a UI **Image** (right-click Canvas > UI > Image).
2. Assign a circular or ring sprite to the Image.
3. Add the **RadialProgressBar** component — it auto-configures the Image as Filled/Radial360.
4. Adjust **Color Gradient**, **Fill Origin**, and **Clockwise** in the Inspector.
5. Wire the **Events** in the Inspector, or drive progress from code.

### Scripting API

```csharp
using ShahvaizJ.RadialProgressBar;

RadialProgressBar bar = GetComponent<RadialProgressBar>();

bar.Progress = 0.75f;           // set target progress (smooth if enabled)
bar.SetProgressImmediate(1f);   // snap instantly, bypass smoothing
bar.IncrementProgress(0.1f);    // add to current progress
bar.ResetProgress();            // reset to zero instantly

bar.DisplayedProgress  // current visual fill amount (read-only)
bar.IsComplete         // true when displayed progress reaches 1.0
```

## License

MIT — see [LICENSE](LICENSE).
