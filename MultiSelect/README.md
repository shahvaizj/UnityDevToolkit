# MultiSelect

A UI option selector with left/right arrow buttons that cycles through a list of string values.

## Features

- Left / right arrow buttons cycle through a serialized `string[]` of options
- Populates the label from the options on `Awake`
- `CurSelected` (index) and `Value` (text) exposed for reads
- Optional wrap-around — arrows auto-disable at the ends when wrapping is off
- `OnSelectionChanged(int)` and `OnValueChanged(string)` UnityEvents
- Runtime `SetOptions`, `SetSelected`, and `SetValue` API

## Installation

Add via Unity Package Manager using the git URL:

```
https://github.com/shahvaizj/multiselect.git
```

Or copy the `Scripts/` folder into your project's `Assets/`.

## Usage

1. Add a **MultiSelect** component to a UI GameObject (e.g. a panel holding the two arrows and a label).
2. Assign the **Left Button**, **Right Button**, and **Label** (a TextMeshPro text) in the Inspector.
3. Fill the **Options** array with the string values to cycle through, and set the starting **Cur Selected** index.
4. Toggle **Wrap** depending on whether paging past the ends should loop or stop.
5. Wire **On Selection Changed** / **On Value Changed** in the Inspector, or subscribe from code.

### Scripting API

```csharp
using ShahvaizJ.MultiSelect;

MultiSelect select = GetComponent<MultiSelect>();

int index   = select.CurSelected; // current index (read-only)
string text = select.Value;       // current option text (read-only)

select.Next();           // advance (same as the right arrow)
select.Previous();       // go back (same as the left arrow)
select.SetSelected(2);   // jump to an index
select.SetValue("Hard"); // select by text; returns true if found

// swap the whole list at runtime (resets to the first entry)
select.SetOptions(new[] { "Easy", "Normal", "Hard" });
```

## License

MIT — see [LICENSE](LICENSE).
