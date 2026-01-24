# 🎮 Unity Networked Inventory System

A comprehensive, multiplayer-ready inventory management system for Unity built with **Unity Netcode for GameObjects**. Features drag-and-drop functionality, hotbar management, tooltips, and persistent runtime data.

## ✨ Features

### 🌐 **Networked Inventory**
- **Client-Server Architecture**: Full multiplayer support using Unity Netcode
- **Real-time Synchronization**: Automatic syncing of inventory changes across clients
- **Server Authority**: Secure server-side validation for all inventory operations

### 🎯 **Drag & Drop System**
- **Intuitive UI**: Smooth drag-and-drop item management
- **Smart Stacking**: Automatic item merging and stack management
- **Item Splitting**: Right-click to split item stacks
- **Visual Feedback**: Live preview of dragged items

### 🔥 **Hotbar Management**
- **Quick Access**: Numbered hotbar slots for fast item switching
- **Active Item Display**: Visual representation of equipped items
- **Runtime Data**: Persistent item properties and modifications

### 💾 **Data Persistence**
- **Runtime Properties**: Custom data attached to individual items
- **Serialization**: JSON-based data storage for network transmission
- **Type Safety**: Automatic type conversion and validation

### 🎨 **UI Components**
- **Tooltips**: Rich item information on hover
- **Visual Indicators**: Active slot highlighting and status display
- **Responsive Design**: Clean, modern interface elements

## 🏗️ Architecture

### Core Components

```
📁 Core System
├── 🔧 ItemContainerCore - Business logic for inventory operations
├── 🌐 NetworkItemContainer - Networked inventory container
├── 📱 LocalItemContainer - Client-side inventory management
├── 🎯 ItemContainerSlotUI - Individual slot management
└── 📦 ItemContainerItemUI - Item data representation

📁 Services
├── 🔄 ItemDragService - Drag and drop operations
├── ⚡ ItemOperationService - Item manipulation logic
├── 🔥 HotbarService - Hotbar state management
└── 💾 RuntimeDataOperationService - Dynamic data handling

📁 Managers
├── 📋 ItemContainersManager - Container registration and lookup
├── 🎮 ItemDragManager - Global drag system coordinator
└── 📚 ItemDatabaseManager - Item definitions and prefabs
```

## 🚀 Quick Start

### Prerequisites
- Unity 2022.3 LTS or higher
- Unity Netcode for GameObjects package
- TextMeshPro package

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/yourusername/unity-inventory-system.git
   ```

2. **Import into Unity**
   - Open Unity Hub
   - Click "Add" and select the project folder
   - Open the project

3. **Setup Netcode**
   - Install Unity Netcode for GameObjects via Package Manager
   - Configure NetworkManager in your scene

### Basic Setup

1. **Create an Inventory Container**
   ```csharp
   // For networked inventories
   GameObject inventoryGO = new GameObject("NetworkInventory");
   NetworkItemContainer container = inventoryGO.AddComponent<NetworkItemContainer>();
   
   // For local inventories
   GameObject localInventoryGO = new GameObject("LocalInventory");
   LocalStorage localStorage = localInventoryGO.AddComponent<LocalStorage>();
   ```

2. **Configure Item Database**
   - Create `ItemDatabase` ScriptableObject
   - Add your item definitions
   - Assign to `ItemDatabaseManager`

3. **Setup UI**
   - Add inventory slots to your UI
   - Connect `ItemContainerSlotUI` components
   - Configure drag visualizer

## 📖 Usage Examples

### Adding Items
```csharp
// Add items to any container
container.TryAddItem("sword_001", 1);
container.TryAddItem("health_potion", 5);
```

### Hotbar Management
```csharp
// Set active hotbar slot
LocalHotbarContainer hotbar = GetComponent<LocalHotbarContainer>();
hotbar.SetActiveSlot(0);

// Update item runtime data
Dictionary<string, object> data = new Dictionary<string, object>
{
    ["durability"] = 85,
    ["enchantment"] = "fire_damage"
};
hotbar.UpdateActiveItemRuntimeData(data);
```

### Custom Item Behaviors
```csharp
public class CustomWeapon : MonoBehaviour, IHotbarItem, IRuntimeDataService
{
    public int durability;
    public string enchantment;
    
    public void InitializeFromRuntimeData(Dictionary<string, object> data)
    {
        if (data.ContainsKey("durability"))
            durability = (int)data["durability"];
        if (data.ContainsKey("enchantment"))
            enchantment = (string)data["enchantment"];
    }
    
    public void InitializeHotbar(LocalHotbarContainer hotbar)
    {
        // Setup weapon-specific hotbar behavior
    }
}
```

## 🎯 Key Features Deep Dive

### 🌐 Network Synchronization
The system uses Unity Netcode's `NetworkBehaviour` for seamless multiplayer support:
- **Server Authority**: All inventory changes are validated server-side
- **ClientRPC**: Efficient updates pushed to all clients
- **Automatic Sync**: New players receive full inventory state on join

### 🔄 Drag & Drop System
Advanced drag-and-drop with multiple interaction modes:
- **Left Click**: Pick up entire stack
- **Right Click**: Split stack or place single item
- **Smart Merging**: Automatic stacking of compatible items
- **Visual Feedback**: Real-time preview of drag operations

### 💾 Runtime Data System
Flexible data persistence for dynamic item properties:
- **Type-Safe Serialization**: Automatic type conversion and validation
- **Network Transmission**: Efficient JSON-based data sync
- **Extensible**: Easy to add custom properties to any item

## 🛠️ Configuration

### Item Database Setup
```csharp
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    public Dictionary<string, ItemDefinition> Database;
}
```

### Container Configuration
```csharp
[CreateAssetMenu(fileName = "ContainerConfig", menuName = "Inventory/Container Config")]
public class ItemContainerConfig : ScriptableObject
{
    public int MaxSlots = 20;
    public bool IsNetworkSynced = true;
    public bool AllowStacking = true;
}
```

## 🎨 UI Customization

### Slot Appearance
```csharp
[CreateAssetMenu(fileName = "SlotConfig", menuName = "Inventory/Slot UI Config")]
public class ItemContainerSlotUIConfig : ScriptableObject
{
    public Sprite ActiveSlotIcon;
    public Sprite PassiveSlotIcon;
    public Color ActiveColor = Color.white;
    public Color PassiveColor = Color.gray;
}
```

### Tooltip System
```csharp
// Automatic tooltip display on hover
TooltipUI.Instance.SetItemName(item.ItemName);
TooltipUI.Instance.SetItemDescription(item.Description);
TooltipUI.Instance.SetTooltipActivity(true);
```

## 🔧 Advanced Features

### Custom Item Types
Implement `IHotbarItem` for specialized item behaviors:
```csharp
public interface IHotbarItem
{
    void InitializeHotbar(LocalHotbarContainer hotbar);
}
```

### Runtime Data Services
Use `IRuntimeDataService` for persistent item properties:
```csharp
public interface IRuntimeDataService
{
    void InitializeFromRuntimeData(Dictionary<string, object> data);
}
```

### Container Services
Create custom containers by implementing `IItemContainerService`:
```csharp
public interface IItemContainerService
{
    bool TryAddItem(string itemId, int amount);
    void ClearSlot(int slotIndex);
    string GetContainerGUID();
}
```

## 🔍 Testing

### Test Adding Items
```csharp
// Add test items (example from NetworkedStorage)
private void Update()
{
    if (Input.GetKeyDown(KeyCode.T))
    {
        string[] testItems = { "metalore", "goldore" };
        int randomIndex = Random.Range(0, testItems.Length);
        TryAddItem(testItems[randomIndex], 1);
    }
}
```

### Debug Runtime Data
```csharp
// Monitor runtime data changes
public class HotbarItem : MonoBehaviour, IHotbarItem
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            hotbarItemIntProperty--;
            runtimeDataOperationService.UpdateRuntimeDataValue("hotbarItemIntProperty", hotbarItemIntProperty);
        }
    }
}
```

## 📊 Performance Considerations

- **Efficient Networking**: Only syncs changes, not full state
- **Smart Updates**: UI refreshes only when necessary
- **Memory Management**: Proper cleanup of event subscriptions
- **Scalable Architecture**: Service-based design for easy extension
