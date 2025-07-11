public class ItemOperationService : IItemOperationService
{
    public bool TryMoveItem(ItemContainerItemUI item, ItemContainerSlotUI targetSlot)
    {
        if (targetSlot.IsEmpty)
        {
            // Clear original slot if item has one
            // Fix: Check if ContainerGUID is not empty instead of null
            if (!item.ContainerGUID.IsEmpty)
            {
                var itemContainer = ItemContainersManager.Instance.GetContainerByGUID(item.ContainerGUID);
                if (itemContainer != null) itemContainer.ClearSlot(item.SlotIndex);
            }

            targetSlot.SetHeldItem(item);
            return true;
        }

        return false;
    }

    public bool TrySplitItem(ItemContainerSlotUI sourceSlot, out ItemContainerItemUI splitItem)
    {
        splitItem = null;

        if (sourceSlot.IsEmpty)
            return false;

        var sourceItem = sourceSlot.GetHeldItem();
        if (sourceItem.Amount <= 1)
            return false;

        var totalAmount = sourceItem.Amount;
        var amountToSplit = totalAmount / 2;
        var amountToRemain = totalAmount - amountToSplit;

        // Create split item
        splitItem = sourceItem.Clone();
        splitItem.Amount = amountToSplit;

        // Update source
        sourceSlot.SetAmount(amountToRemain);
        sourceSlot.Refresh();

        return true;
    }

    public bool TryMergeItems(ItemContainerItemUI sourceItem, ItemContainerSlotUI targetSlot)
    {
        if (targetSlot.IsEmpty)
            return false;

        if (targetSlot.TryToMerge(sourceItem))
        {
            // Clear original slot if source item has one
            // Fix: Check if ContainerGUID is not empty instead of null
            if (!sourceItem.ContainerGUID.IsEmpty)
            {
                var itemContainer = ItemContainersManager.Instance.GetContainerByGUID(sourceItem.ContainerGUID);
                if (itemContainer != null) itemContainer.ClearSlot(sourceItem.SlotIndex);
            }

            return true;
        }

        return false;
    }

    public bool TrySwapItems(ItemContainerItemUI item1, ItemContainerSlotUI slot2, out ItemContainerItemUI swappedItem)
    {
        swappedItem = slot2.GetHeldItem();

        // Remove item2 from its slot
        var itemContainer = ItemContainersManager.Instance.GetContainerByGUID(slot2.ContainerGUID.Value);
        if (itemContainer != null) itemContainer.ClearSlot(slot2.SlotIndex);

        // Place item1 in slot2
        slot2.SetHeldItem(item1);

        return true;
    }
}