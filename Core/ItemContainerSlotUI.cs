using System;
using System.Collections.Generic;
using TMPro;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ItemContainerSlotUI : NetworkBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private ItemContainerSlotUIConfig Config;

    [Header("Configuration")] [SerializeField]
    private Image SlotRenderer;

    [SerializeField] private Image HeldItemRenderer;
    [SerializeField] private TextMeshProUGUI AmountText;

    private bool isActive;

    private ItemContainerItemUI HeldItem;

    // public ItemContainer Container;
    public FixedString128Bytes ContainerGUID;
    public bool IsNetworkedSynced;
    public bool IsEmpty => HeldItem == null;
    public int SlotIndex;

    public event Action<int> OnSlotChanged;

    public void Initialize(FixedString128Bytes containerGuid, bool isNetworkedSynced, int slotIndex)
    {
        // Container = container;
        ContainerGUID = containerGuid;
        IsNetworkedSynced = isNetworkedSynced;
        SlotIndex = slotIndex;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!IsSpawned)
            return;
        if (eventData.button == PointerEventData.InputButton.Left)
            ItemDragManager.Instance.HandleLeftClick(this);
        else if (eventData.button == PointerEventData.InputButton.Right)
            ItemDragManager.Instance.HandleRightClick(this);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsSpawned)
            ItemDragManager.Instance.HandleHoverEnter(this);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsSpawned)
            ItemDragManager.Instance.HandleHoverExit(this);
    }

    public ItemContainerItemUI GetHeldItem()
    {
        return HeldItem;
    }

    public void SetHeldItem(ItemContainerItemUI item)
    {
        Debug.Log("Set Held item invoked");
        if (IsNetworkedSynced)
        {
            if (IsServer)
                SetHeldItemClientRpc(item?.ItemId ?? "", item?.Amount ?? 0, item?.SerializeRuntimeData() ?? "{}");
            else
                SetHeldItemServerRpc(item?.ItemId ?? "", item?.Amount ?? 0, item?.SerializeRuntimeData() ?? "{}");

            return;
        }

        HeldItem = item;
        if (HeldItem != null)
        {
            HeldItem.ContainerGUID = ContainerGUID.Value;
            HeldItem.SlotIndex = SlotIndex;
        }

        Refresh();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetHeldItemServerRpc(string itemId, int amount, string runtimeDataJson)
    {
        SetHeldItemClientRpc(itemId, amount, runtimeDataJson);
    }

    [ClientRpc]
    private void SetHeldItemClientRpc(string itemId, int amount, string runtimeDataJson)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            HeldItem = null;
        }
        else
        {
            var runtimeData = ItemContainerItemUI.DeserializeRuntimeData(runtimeDataJson);
            HeldItem = new ItemContainerItemUI(itemId, amount, ContainerGUID.Value, SlotIndex, runtimeData);
        }

        Refresh();
    }

    public bool TryToMerge(ItemContainerItemUI item)
    {
        if (!CanMerge(item)) return false;
        // HeldItem.Amount += item.Amount;
        SetAmount(HeldItem.Amount + item.Amount);
        Refresh();
        return true;
    }

    public void SetAmount(int amount)
    {
        if (IsNetworkedSynced)
        {
            if (IsServer)
                SetAmountClientRpc(amount);
            else
                SetAmountServerRpc(amount);
            return;
        }

        HeldItem.Amount = amount;
        Refresh();
    }

    [ServerRpc(RequireOwnership = false)]
    private void SetAmountServerRpc(int amount)
    {
        SetAmountClientRpc(amount);
    }

    [ClientRpc]
    private void SetAmountClientRpc(int amount)
    {
        HeldItem.Amount = amount;
        Refresh();
    }

    public void Refresh()
    {
        var itemDefinition = ItemDatabaseManager.Instance.GetItemById(HeldItem.ItemId);
        HeldItemRenderer.sprite = itemDefinition.Icon;
        AmountText.text = HeldItem.Amount.ToString();
        OnSlotChanged?.Invoke(SlotIndex);
    }

    public void Clear()
    {
        if (IsNetworkedSynced)
        {
            if (IsServer)
                ClearClientRpc();
            else
                ClearServerRpc();

            return;
        }

        HeldItem = null;
        HeldItemRenderer.sprite = null;
        AmountText.text = "";
        OnSlotChanged?.Invoke(SlotIndex);
    }

    [ServerRpc(RequireOwnership = false)]
    private void ClearServerRpc()
    {
        ClearClientRpc();
    }

    [ClientRpc]
    private void ClearClientRpc()
    {
        HeldItem = null;
        HeldItemRenderer.sprite = null;
        AmountText.text = "";
        OnSlotChanged?.Invoke(SlotIndex);
    }

    public bool IsSameItem(ItemContainerItemUI item)
    {
        return HeldItem.ItemId == item.ItemId;
    }

    public bool CanMerge(ItemContainerItemUI item)
    {
        var itemDefinition = ItemDatabaseManager.Instance.GetItemById(HeldItem.ItemId);
        var amount = HeldItem.Amount + item.Amount;
        if (amount > itemDefinition.MaxStackSize) return false;
        if (!IsSameItem(item)) return false;
        return true;
    }

    public void SetActive(bool active)
    {
        isActive = active;
        SlotRenderer.sprite = active ? Config.ActiveSlotIcon : Config.PassiveSlotIcon;
    }
}