# Middle Unity Developer Test — Answers

Unity 6000.1 · URP · uGUI + TextMeshPro · Input System · VContainer (DI).
Code in `Assets/Scripts/` (`OctoGames.*`). Demo scenes in `Assets/Scenes/`.

| Task | Code |
|---|---|
| 2 — Save/Load | `Saving/` |
| 3 — Popups | `UI/Popups/` |
| 4 — Refactor | `UI/CharactersView.cs` |
| 5 — Entities | `Gameplay/` |
| DI | `DI/` — one LifetimeScope per scene |

## 1. Coding principles

**Keep data and UI apart.** UI shows a model, it never owns the game state. Designers
change UI all the time; if logic lives in a view, every change can break gameplay and
nothing can be reused. Here: popups take a `PopupData`, `CharactersView` reads the
registry, saves store plain data classes (not MonoBehaviours).

**Watch the update loop and allocations.** `GetComponent`, `string.Format` and
`Debug.Log` every frame make garbage and cause stutter. 3D already uses the frame
budget, so UI should be near-free. Here: `CharactersView` caches refs, updates on an
interval, reuses a `StringBuilder`; `EntityRegistry` updates on events, not by
scanning the scene each frame.

## 2. Save / Load

`ISaveService` (injected): `Save / TryLoad / LoadOrDefault / Exists / Delete`.

```csharp
_save.Save("settings", settings); // any [Serializable] class
var s = _save.LoadOrDefault("settings", Settings.Default); // never throws
```

- **Safe loads.** Missing, broken or wrong-type data returns false / a fallback. It
  never throws.
- **Safe writes.** Write to a temp file, swap it in, keep a `.bak`. A crash while
  saving can't break the file, and a broken file falls back to the backup.
- **Reusable and testable.** Logic sits in `SaveService` behind `ISaveService`. The
  serializer is a constructor argument, so you can swap JsonUtility for Newtonsoft or
  an encrypted one without changing callers. Files go to `persistentDataPath`.

JsonUtility can't do dictionaries, subclasses or top-level lists — wrap those in a
`[Serializable]` class.

## 3. Popups

Content is data, built with a small builder, shown through `IPopupService`:

```csharp
PopupBuilder.Create()
    .Title("Quit?").Body("Unsaved progress will be lost.")
    .Button("Cancel", null) // 1-5 buttons, checked in Build()
    .Button("Quit", Application.Quit) // each button has a callback
    .Show(popupService);
```

- `PopupView` shows the data and is modal: a full-screen root with a `CanvasGroup`
  that blocks the background, a dim layer, the card, and buttons in a layout group.
- `PopupService` (behind `IPopupService`, injected) shows popups and pools them so it
  doesn't keep calling `Instantiate`.
- Buttons close the popup by default; `CloseOnButtonClick(false)` keeps it open (e.g.
  tutorials).

### 3.1 Components for the prefab

| Part | Components | Why |
|---|---|---|
| Shared canvas | `Canvas` + `CanvasScaler` + `GraphicRaycaster` | Draw, scale to resolution, route clicks. One canvas, not one per popup. |
| Popup root | `CanvasGroup` | Show/hide and block background clicks in one place. |
| Dim | `Image` (see-through, raycast target) | Darkens and blocks the background, so it's modal. |
| Card | `Image` (sliced sprite) | Rounded background for the content. |
| Title / Body | `TextMeshProUGUI` | Sharp, scalable text. |
| Button row | `HorizontalLayoutGroup` | Spaces 1-5 buttons by itself. |
| Button | `Button` + `Image` + child `TextMeshProUGUI` | Click handling, background, label. |

## 4. Refactor

**Bugs in the original.**
Compile: `[SerializedField]`; `GetComponents` (an array) put into one `Character`;
`List.Length`. Logic: average is upside down (`count / total`), divide by zero, and
it counts all entities instead of only active ones. Performance: runs in
`FixedUpdate`; `GetComponent` every tick; `string.Format` + `Debug.Log` every tick.

**Fixes (`CharactersView.cs`).**
- Update on a timed interval in `Update`, not `FixedUpdate`.
- Cache `TMP_Text`; values come from the injected `EntityRegistry`, so no
  `GetComponent` in the loop.
- Right and safe math: `count > 0 ? total / count : 0`.
- Count only active entities, reuse a `StringBuilder`, and set the text only when it
  changed (setting it rebuilds the mesh). No logging each frame.

## 5. Entity tracking

Entities add and remove themselves:

```csharp
[Inject] void Construct(EntityRegistry r) => _registry = r;
void OnEnable() => _registry?.Register(this);
void OnDisable() => _registry?.Unregister(this);
```

- **Disable and destroy use one path.** Unity calls `OnDisable` before `OnDestroy`,
  so both remove the entity the same way — no dead references. That's the common bug
  with cached `FindObjectsOfType` or a `List<Transform>`.
- **Fast and safe.** `HashSet` for O(1) add/remove. `GetActive()` filters into a
  reused list (no garbage) and skips nulls.
- **Testable.** `EntityRegistry` is a plain class in VContainer; entities are spawned
  through the resolver, so the registry is set before `OnEnable`.

## Bonus

- **Scale:** assembly definitions, a generic `EntityRegistry<T>` per type, versioned
  save data with async writes, Addressables + a queue for popups.
- **Designers:** define popups as data / ScriptableObjects shown by id;
  `[SerializeField]` + `[Tooltip]` on every tunable.
- **Profiling:** Unity Profiler (watch GC alloc, aim for 0 B/frame in UI), Frame
  Debugger for canvas rebuilds, Memory Profiler for leaks like entities that never
  unregister.
