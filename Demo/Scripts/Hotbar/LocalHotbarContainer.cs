using System;
using System.Collections.Generic;
using UnityEngine;

public class LocalHotbarContainer : LocalItemContainer
{
    [Header("Configuration")] [SerializeField]
    private Transform ObjectHolderTransform;

    #region Runtime Changing Variables

    private IHotbarService hotbarService;
    private IHotbarOperationService hotbarOperationService;

    #endregion

    private void Awake()
    {
        if (hotbarService == null)
        {
            hotbarService = new HotbarService();
            hotbarOperationService = new HotbarOperationService(this);

            hotbarService.OnActiveSlotChanged += hotbarOperationService.UpdateHandItem;
            hotbarOperationService.OnHandItemChanged += HandleHandItemChange;
        }
    }

    public override void OnDestroy()
    {
        base.OnDestroy();
        hotbarService.OnActiveSlotChanged -= hotbarOperationService.UpdateHandItem;
        hotbarOperationService.OnHandItemChanged -= HandleHandItemChange;
    }


    public override void OnSlotChanged(int slotIndex)
    {
        if (slotIndex != hotbarService.ActiveSlot) return;

        var slot = Slots[slotIndex];
        if (slot.IsEmpty)
        {
            HandleHandItemChange(null);
            return;
        }

        var itemDefinition = ItemDatabaseManager.Instance.GetItemPrefabById(slot.GetHeldItem().ItemId);
        HandleHandItemChange(itemDefinition);
    }

    private void HandleHandItemChange(GameObject itemPrefab)
    {
        var childCount = ObjectHolderTransform.childCount;
        for (var i = childCount - 1; i >= 0; i--) Destroy(ObjectHolderTransform.GetChild(i).gameObject);

        if (itemPrefab == null) return;

        var spawnedItem = Instantiate(itemPrefab, ObjectHolderTransform);

        var hotbarSpawnedItem = spawnedItem.GetComponent<IHotbarItem>();
        if (hotbarSpawnedItem != null)
            hotbarSpawnedItem.InitializeHotbar(this);

        var runtimeDataService = spawnedItem.GetComponent<IRuntimeDataService>();
        var slotRuntimeData = GetActiveItemRuntimeData();

        Debug.Log($"Slot runtime data: {slotRuntimeData}");

        if (runtimeDataService != null)
            runtimeDataService.InitializeFromRuntimeData(slotRuntimeData);
    }

    public void SetActiveSlot(int slot)
    {
        hotbarService.SetActiveSlot(slot);
    }

    public void UpdateActiveItemRuntimeData(Dictionary<string, object> runtimeData)
    {
        var slot = Slots[hotbarService.ActiveSlot];
        if (slot.IsEmpty) return;

        var item = slot.GetHeldItem();
        item.RuntimeDataOperationService.UpdateRuntimeData(runtimeData);
    }

    public Dictionary<string, object> GetActiveItemRuntimeData()
    {
        var slot = Slots[hotbarService.ActiveSlot];
        if (slot.IsEmpty) return null;

        return slot.GetHeldItem().RuntimeData;
    }
}