using System;
using UnityEngine;

public class HotbarOperationService : IHotbarOperationService
{
    public event Action<GameObject> OnHandItemChanged;

    private LocalHotbarContainer hotbar;

    public HotbarOperationService(LocalHotbarContainer hotbar)
    {
        this.hotbar = hotbar;
    }

    public ItemContainerItemUI GetItemInSlot(int slot)
    {
        if (hotbar.Slots[slot].IsEmpty)
            return null;

        var item = hotbar.Slots[slot].GetHeldItem();
        return item;
    }

    public void UpdateHandItem(int slot)
    {
        var item = GetItemInSlot(slot);
        if (item == null)
        {
            Debug.Log("There is no item in active slot!");
            OnHandItemChanged?.Invoke(null);
            return;
        }

        var itemDefinition = ItemDatabaseManager.Instance.GetItemById(item.ItemId);
        Debug.Log($"Updating hand item to {itemDefinition.ItemName}");
        OnHandItemChanged?.Invoke(itemDefinition.Prefab);
    }

    public void ClearHandItem()
    {
    }
}