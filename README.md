# Unity Networked Inventory System

A production-ready, multiplayer inventory management system for Unity built on **Unity Netcode for GameObjects**. Designed with a decoupled service architecture, it supports server-authoritative state synchronization, drag-and-drop UI, hotbar equipment handling, and dynamic item runtime serialization.

---

## Technical Specifications

### Network Architecture
- **Server Authority**: All container mutations, item moves, and stack splits undergo server validation prior to execution.
- **Delta Synchronization**: Transmits individual slot mutations over RPCs rather than full container state arrays to minimize bandwidth usage.
- **State Reconciliation**: Automatically synchronizes full container contents upon initial client connection or reconnection.

### Core Features
- **Drag & Drop Operations**: Left-click stack selection, right-click single-item deposition or stack division, and automatic stack merging based on item compatibility.
- **Equipment & Hotbar Binding**: Active slot tracking, quick-swap key bindings, and context-sensitive item activation hooks.
- **Dynamic Runtime Data**: JSON-serialized key-value dictionary attached to individual item instances to track dynamic state (e.g., durability, stats, enchantments).
- **Decoupled Architecture**: Logic separated into core domain models, networking wrappers, UI views, and static/dynamic services.

---

## Directory Structure

```
Core
├── ItemContainerCore.cs             # Platform-agnostic container logic
├── NetworkItemContainer.cs          # Netcode network behaviour wrapper
├── LocalItemContainer.cs            # Client-side local inventory state
├── ItemContainerSlotUI.cs           # UI view controller for inventory slots
└── ItemContainerItemUI.cs           # Visual representation of item instances

Services
├── ItemDragService.cs               # Drag-and-drop state machine
├── ItemOperationService.cs          # Operations for stacking, splitting, swapping
├── HotbarService.cs                 # Active slot state and selection logic
└── RuntimeDataOperationService.cs  # Serialization and modification of dynamic properties

Management
├── ItemContainersManager.cs         # Global registration registry for container GUIDs
├── ItemDragManager.cs               # Screen-space drag visualization coordinator
└── ItemDatabaseManager.cs           # Definition lookup and asset database
```

---

## Setup & Prerequisites

### Dependencies
- Unity 2022.3 LTS or higher
- Unity Netcode for GameObjects (`com.unity.netcode.gameobjects`)
- Unity TextMeshPro (`com.unity.textmeshpro`)

### Installation Procedure

1. **Clone Repository**
   ```bash
   git clone https://github.com/yourusername/unity-inventory-system.git
   ```

2. **Package Configuration**
   Verify `com.unity.netcode.gameobjects` is present in the Unity Package Manager.

3. **Scene Integration**
   Ensure an active `NetworkManager` instance is configured within your starting scene.

---

## Integration Guide

### 1. Initializing Containers

#### Networked Container
Attach `NetworkItemContainer` to a GameObject with an attached `NetworkObject` component.

```csharp
[RequireComponent(typeof(NetworkObject))]
public class NetworkInventoryInitializer : MonoBehaviour
{
    [SerializeField] private NetworkItemContainer networkContainer;

    private void Awake()
    {
        if (networkContainer == null)
            networkContainer = GetComponent<NetworkItemContainer>();
    }
}
```

#### Local Container
Use `LocalStorage` or `LocalItemContainer` for client-only UI panels, such as crafting menus or local chest previews.

```csharp
public class LocalInventoryInitializer : MonoBehaviour
{
    [SerializeField] private LocalStorage localStorage;

    private void Start()
    {
        localStorage.InitializeContainer(slotCount: 20);
    }
}
```

### 2. Item Mutations

```csharp
// Server-side item insertion
public void GrantLoot(NetworkItemContainer targetContainer, string itemId, int quantity)
{
    if (!targetContainer.IsServer) return;

    bool success = targetContainer.TryAddItem(itemId, quantity);
    if (!success)
    {
        // Handle inventory full condition
    }
}
```

### 3. Dynamic Property Binding

Implement `IRuntimeDataService` and `IHotbarItem` on wearable or usable item behaviours:

```csharp
public class WeaponEquipment : MonoBehaviour, IHotbarItem, IRuntimeDataService
{
    public int CurrentDurability { get; private set; }
    public string ElementalType { get; private set; }

    public void InitializeFromRuntimeData(Dictionary<string, object> data)
    {
        if (data.TryGetValue("durability", out object rawDurability))
        {
            CurrentDurability = System.Convert.ToInt32(rawDurability);
        }

        if (data.TryGetValue("element", out object rawElement))
        {
            ElementalType = rawElement as string;
        }
    }

    public void InitializeHotbar(LocalHotbarContainer hotbar)
    {
        // Bind equipment-specific input listeners or animations
    }
}
```

---

## API Reference

### Key Interfaces

#### `IItemContainerService`
Defines operational contracts for container data manipulation.

```csharp
public interface IItemContainerService
{
    bool TryAddItem(string itemId, int amount);
    bool TryRemoveItem(int slotIndex, int amount);
    void ClearSlot(int slotIndex);
    string GetContainerGUID();
}
```

#### `IRuntimeDataService`
Provides initialization contracts for items carrying unique instance state.

```csharp
public interface IRuntimeDataService
{
    void InitializeFromRuntimeData(Dictionary<string, object> data);
}
```

#### `IHotbarItem`
Contract for items responding to active hotbar selection.

```csharp
public interface IHotbarItem
{
    void InitializeHotbar(LocalHotbarContainer hotbar);
}
```

---

## Configuration Assets

### ScriptableObject Definitions

#### Container Configuration
```csharp
[CreateAssetMenu(fileName = "ContainerConfig", menuName = "Inventory/Container Configuration")]
public class ItemContainerConfig : ScriptableObject
{
    public int MaxSlots = 20;
    public bool IsNetworkSynced = true;
    public bool AllowStacking = true;
}
```

#### Slot UI Style Configuration
```csharp
[CreateAssetMenu(fileName = "SlotUIConfig", menuName = "Inventory/Slot UI Configuration")]
public class ItemContainerSlotUIConfig : ScriptableObject
{
    public Sprite DefaultSlotBackground;
    public Sprite ActiveSlotBackground;
    public Color NormalColor = Color.white;
    public Color HighlightedColor = Color.yellow;
}
```
