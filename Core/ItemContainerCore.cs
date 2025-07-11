using System;
using System.Collections.Generic;
using UnityEngine;

public class ItemContainerCore
{
    // TODO: fix scenario: dragging 3 items, putting them per one in slot, last one deletes prevoious one and lands on targeted slot
    public List<ItemContainerSlotUI> Slots { get; private set; }
    public ItemContainerConfig Config { get; private set; }
    public string ContainerGuid { get; private set; }

    public event Action<int> OnSlotChanged;

    public ItemContainerCore(ItemContainerConfig config, List<ItemContainerSlotUI> slots, string containerGuid)
    {
        Config = config;
        Slots = slots;
        ContainerGuid = containerGuid;
    }

    public void InitializeSlots(IItemContainerService owner)
    {
        for (var slotIndex = 0; slotIndex < Slots.Count; slotIndex++)
        {
            var slot = Slots[slotIndex];
            slot.Initialize(ContainerGuid, Config.IsNetworkSynced, slotIndex);
            slot.OnSlotChanged += InvokeOnSlotChanged;
        }
    }

    public void DeInitializeSlots()
    {
        foreach (var slot in Slots)
            slot.OnSlotChanged -= InvokeOnSlotChanged;
    }

    private void InvokeOnSlotChanged(int slotIndex)
    {
        OnSlotChanged?.Invoke(slotIndex);
    }

    public bool TryAddItemToStack(string itemId, int amount)
    {
        // ... existing non-network stack logic ...
        if (!HasAvailableStack(itemId, amount, out var slotIndex)) return false;

        var itemDefinition = ItemDatabaseManager.Instance.GetItemById(itemId);
        if (itemDefinition == null)
        {
            Debug.LogWarning($"No item with id {itemId} found");
            return false;
        }

        var heldItem = Slots[slotIndex].GetHeldItem();
        Slots[slotIndex].SetAmount(heldItem.Amount + amount);
        return true;
    }

    public bool TryAddItemToEmptySlot(string itemId, int amount)
    {
        // ... existing non-network empty slot logic ...
        if (!HasEmptySlot(out var slotIndex)) return false;

        var itemDefinition = ItemDatabaseManager.Instance.GetItemById(itemId);
        if (itemDefinition == null)
        {
            Debug.LogWarning($"No item with id {itemId} found");
            return false;
        }

        Slots[slotIndex].SetHeldItem(new ItemContainerItemUI(itemId, amount, ContainerGuid, slotIndex));
        return true;
    }

    public bool HasAvailableStack(string itemId, int amount, out int slotIndex)
    {
        slotIndex = -1;
        foreach (var slot in Slots)
        {
            if (slot.IsEmpty) continue;

            var itemDefinition = ItemDatabaseManager.Instance.GetItemById(itemId);
            if (itemDefinition == null)
            {
                Debug.LogWarning($"No item with id {itemId} found");
                return false;
            }

            if (!slot.CanMerge(new ItemContainerItemUI(itemId, amount, ContainerGuid, slotIndex))) continue;

            slotIndex = slot.SlotIndex;
            return true;
        }

        return false;
    }

    public bool HasEmptySlot(out int slotIndex)
    {
        /*...*/
        slotIndex = -1;
        foreach (var slot in Slots)
            if (slot.IsEmpty)
            {
                slotIndex = slot.SlotIndex;
                return true;
            }

        return false;
    }

    public void ClearSlot(int slotIndex)
    {
        /*...*/
        Slots[slotIndex].Clear();
    }
}