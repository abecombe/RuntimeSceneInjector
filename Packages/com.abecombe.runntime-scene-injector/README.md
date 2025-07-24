# Runtime Scene Injector

A lightweight, zero-configuration dependency injection tool for Unity. It automatically injects scene-based dependencies into your `MonoBehaviour`s at runtime using a simple `[Inject]` attribute.

## ✨ Features

- **Field and Method Injection**: Inject dependencies into fields or methods.
- **Collection Support**: Resolves all scene implementations into `List<T>`, `ISet<T>`, `T[]`, and more.
- **Inheritance Aware**: Correctly injects `private` members in base classes.
- **Optional Dependencies**: Use the `[Nullable]` attribute to mark dependencies that aren't required.

## 📦 Installation

1.  Open the Unity Package Manager
2.  Click the **+** button
3.  Select "**Add package from git URL...**"
4.  Enter `https://github.com/abecombe/RuntimeSceneInjector.git?path=Packages/com.abecombe.runntime-scene-injector`

## 🚀 How to Use

### 1\. Setup

Drag the **`InterfaceInjector.prefab`** into your scene. This prefab contains the manager component required for injection. A single instance is needed in every scene where you use this tool.

### 2\. Request Dependencies

Use the `[Inject]` attribute on fields or methods within any `MonoBehaviour`.

```csharp
using UnityEngine;
using System.Collections.Generic;
using RuntimeSceneInjector; // Make sure to include the namespace

public class Player : MonoBehaviour
{
    // Inject a single, required dependency
    [Inject]
    private IInputService _input;

    // Inject an optional dependency
    [Inject, Nullable]
    private IAudioService _audio;

    // Inject all implementations of an interface
    [Inject]
    private IReadOnlyList<IAbility> _abilities;

    // Method injection is also supported
    [Inject]
    private void Construct(IGameManager manager)
    {
        manager.RegisterPlayer(this);
    }
}
```

### 3\. Provide Dependencies

Ensure that `MonoBehaviour`s implementing the required interfaces (e.g., `IInputService`, `IGameManager`) are present on `GameObject`s within the scene. The injector will find them automatically.