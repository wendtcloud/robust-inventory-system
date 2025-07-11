using System.Collections.Generic;

public interface IItemContainerService
{
    ItemContainerConfig Config { get; }
    List<ItemContainerSlotUI> Slots { get; }
    string GetContainerGUID();
    bool TryAddItem(string itemId, int amount);
    void ClearSlot(int slotIndex);
    void SetRuntimeData(int slotIndex, string key, object value);
}