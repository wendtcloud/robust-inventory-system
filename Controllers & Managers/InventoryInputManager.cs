using UnityEngine;
using UnityEngine.InputSystem;

public class InventoryInputManager : MonoBehaviour, IInventoryInputHandler, IHotbarInputHandler
{
    [Header("Services")] [SerializeField] private DragVisualizer dragVisualizer;

    [SerializeField] private LocalHotbarContainer hotbar;

    // [SerializeField] private Hotbar hotbar;
    private IItemDragService dragService;
    private IItemOperationService operationService;
    private IDragVisualizer visualizer;

    // Dependency Injection
    public void Initialize(IItemDragService dragService, IItemOperationService operationService,
        IDragVisualizer visualizer)
    {
        this.dragService = dragService;
        this.operationService = operationService;
        this.visualizer = visualizer;
    }

    private void Awake()
    {
        // Default initialization if not injected
        if (dragService == null)
        {
            dragService = new ItemDragService();
            operationService = new ItemOperationService();
            visualizer = dragVisualizer;
        }
    }

    public void HandleLeftClick(ItemContainerSlotUI slot)
    {
        if (dragService.IsDragging)
            HandleDropItem(slot);
        else
            HandlePickupItem(slot);
    }

    public void HandleRightClick(ItemContainerSlotUI slot)
    {
        if (dragService.IsDragging)
            HandleDropSingleItem(slot);
        else
            HandleSplitItem(slot);
    }

    public void HandleHoverEnter(ItemContainerSlotUI slot)
    {
        if (slot.IsEmpty) return;

        var item = slot.GetHeldItem();
        var itemDefinition = ItemDatabaseManager.Instance.GetItemById(item.ItemId);
        TooltipUI.Instance.SetItemName(itemDefinition.ItemName);
        TooltipUI.Instance.SetItemDescription(itemDefinition.ItemDescription);
        TooltipUI.Instance.SetTooltipActivity(true);
    }

    public void HandleHoverExit(ItemContainerSlotUI slot)
    {
        TooltipUI.Instance.SetTooltipActivity(false);
    }

    public void HandleActiveSlotSwap(int slot)
    {
        // hotbar.SetActiveSlot(slot);
    }

    private void Update()
    {
        if (dragService.IsDragging)
            visualizer.UpdatePosition(Input.mousePosition);
        else
            visualizer.HideDraggedItem();
    }

    // ====================
    // PRIVATE OPERATION METHODS
    // ====================

    private void HandlePickupItem(ItemContainerSlotUI slot)
    {
        if (slot.IsEmpty) return;

        var item = slot.GetHeldItem();
        slot.Clear();

        dragService.StartDrag(item);
        visualizer.ShowDraggedItem(item, Input.mousePosition);

        var itemDefinition = ItemDatabaseManager.Instance.GetItemById(item.ItemId);
        Debug.Log($"Picked up item: {itemDefinition.ItemName}");
    }

    public bool IsDroppedOnSameSlot(ItemContainerSlotUI slot, ItemContainerItemUI droppedItem)
    {
        // Fixed: Remove .Value since ContainerGUID is already a FixedString128Bytes
        return slot.ContainerGUID.Value == droppedItem.ContainerGUID &&
               slot.SlotIndex == droppedItem.SlotIndex;
    }

    private void HandleDropItem(ItemContainerSlotUI slot)
    {
        var draggedItem = dragService.GetDraggedItem();

        // Same slot - return item
        if (IsDroppedOnSameSlot(slot, draggedItem))
        {
            if (slot.IsEmpty) slot.SetHeldItem(draggedItem);
            EndDragOperation();
            return;
        }

        // Try to move to empty slot
        if (operationService.TryMoveItem(draggedItem, slot))
        {
            var draggedItemDefinition = ItemDatabaseManager.Instance.GetItemById(draggedItem.ItemId);
            Debug.Log($"Moved item: {draggedItemDefinition.ItemName}");
            EndDragOperation();
            return;
        }

        // Try to merge
        if (operationService.TryMergeItems(draggedItem, slot))
        {
            var draggedItemDefinition = ItemDatabaseManager.Instance.GetItemById(draggedItem.ItemId);
            Debug.Log($"Merged items: {draggedItemDefinition.ItemName}");
            EndDragOperation();
            return;
        }

        // Swap items
        if (operationService.TrySwapItems(draggedItem, slot, out var swappedItem))
        {
            // Start dragging the item that was in the slot
            dragService.StartDrag(swappedItem);
            swappedItem.ContainerGUID.Clear();
            visualizer.ShowDraggedItem(swappedItem, Input.mousePosition);
            var swappedItemDefinition = ItemDatabaseManager.Instance.GetItemById(swappedItem.ItemId);
            var draggedItemDefinition = ItemDatabaseManager.Instance.GetItemById(draggedItem.ItemId);

            Debug.Log(
                $"Swapped items: {draggedItemDefinition.ItemName} with {swappedItemDefinition.ItemName}");
            return;
        }
    }

    private void HandleDropSingleItem(ItemContainerSlotUI slot)
    {
        var draggedItem = dragService.GetDraggedItem();
        var singleItem = draggedItem.Clone();
        singleItem.Amount = 1;

        // Try to place single item
        if (slot.IsEmpty && operationService.TryMoveItem(singleItem, slot))
        {
            draggedItem.Amount--;
            UpdateDraggedItemDisplay();

            if (draggedItem.Amount <= 0) EndDragOperation();
            return;
        }

        // Try to merge single item
        if (operationService.TryMergeItems(singleItem, slot))
        {
            draggedItem.Amount--;
            UpdateDraggedItemDisplay();

            if (draggedItem.Amount <= 0) EndDragOperation();
        }
    }

    private void HandleSplitItem(ItemContainerSlotUI slot)
    {
        if (slot.IsEmpty) return;

        var slotItem = slot.GetHeldItem();
        if (slotItem.Amount <= 1)
        {
            HandleLeftClick(slot);
            return;
        }

        if (operationService.TrySplitItem(slot, out var splitItem))
        {
            dragService.StartDrag(splitItem);
            visualizer.ShowDraggedItem(splitItem, Input.mousePosition);
            var splitItemDefinition = ItemDatabaseManager.Instance.GetItemById(splitItem.ItemId);
            Debug.Log($"Split item: {splitItemDefinition.ItemName}");
        }
    }

    private void UpdateDraggedItemDisplay()
    {
        var draggedItem = dragService.GetDraggedItem();
        if (draggedItem != null) visualizer.ShowDraggedItem(draggedItem, Input.mousePosition);
    }

    private void EndDragOperation()
    {
        dragService.EndDrag();
        visualizer.HideDraggedItem();
    }

    public void OnHotbarActiveSlotSwap(InputValue value)
    {
        var rawValue = value.Get<float>();
        var slotNumber = Mathf.RoundToInt(rawValue);
        hotbar.SetActiveSlot(slotNumber - 1);
    }
}