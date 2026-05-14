# Custom Log

A lightweight conditional debug logger for Unity 6. All calls are stripped from non-development builds via `[Conditional]` — zero runtime cost in release.

- Plain, warning, and error logs wrapping Unity's `Debug` class
- Colored variants: Blue, Red, Yellow, Green, Cyan, Orange
- All calls stripped in release builds via `[Conditional]`
- Optional context object to ping GameObjects from the console

## Installation

**Option 1 — Unity Package Manager (Git URL)**
```
https://github.com/shahvaizj/unity-custom-log.git
```

**Option 2 — Unity Package**

Download `Custom Log.unitypackage` from this repo and double-click to import.

**Option 3 — Manual**

Drop the `Custom Log/` folder into your project's `Assets/` directory.

## Usage

```csharp
using ShahvaizJ.CustomLog;

Log.Print("hello");
Log.Warning("watch out");
Log.Error("something broke");

Log.Blue("spawned");
Log.Green("health full");
Log.Yellow("low ammo");
Log.Red("took damage");
Log.Cyan("state changed");
Log.Orange("cooldown active");

// Optional context — click the log to ping the object in the hierarchy
Log.Blue("enemy detected", this);
```

## Build Stripping

Every method is decorated with `[Conditional("DEVELOPMENT_BUILD")]` and `[Conditional("UNITY_EDITOR")]`. Call sites are removed by the compiler in release builds — no `#if` guards needed at the call site.

## Colors

| Method | Hex |
|---|---|
| `Blue` | `#4FC3F7` |
| `Red` | `#EF5350` |
| `Yellow` | `#FFEE58` |
| `Green` | `#66BB6A` |
| `Cyan` | `#26C6DA` |
| `Orange` | `#FFA726` |

## License

MIT
