using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class LocalItemContainer : NetworkBehaviour, IItemContainerService
{
    [SerializeField] private ItemContainerConfig config;
    [SerializeField] private List<ItemContainerSlotUI> slots;
    public ItemContainerConfig Config => config;
    public List<ItemContainerSlotUI> Slots => slots;
    private ItemContainerCore core;
    private string localGUID;

    private void Start()
    {
        Initialize(Config, Slots);
    }

    public void Initialize(ItemContainerConfig config, List<ItemContainerSlotUI> slots)
    {
        if (!IsOwner)
        {
            gameObject.SetActive(false);
            return;
        }

        localGUID = Guid.NewGuid().ToString();
        core = new ItemContainerCore(config, slots, localGUID);
        core.InitializeSlots(this);
        core.OnSlotChanged += OnSlotChanged;
        ItemContainersManager.Instance.RegisterContainer(this);
    }

    public virtual void OnSlotChanged(int slotIndex)
    {
    }

    public string GetContainerGUID()
    {
        return localGUID;
    }

    public bool TryAddItem(string itemId, int amount)
    {
        if (core.TryAddItemToStack(itemId, amount)) return true;
        if (core.TryAddItemToEmptySlot(itemId, amount)) return true;
        return false;
    }

    public void ClearSlot(int slotIndex)
    {
        core.ClearSlot(slotIndex);
    }

    public void SetRuntimeData(int slotIndex, string key, object value)
    {
        // Local version: apply immediately
    }
}