using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Netcode;
using UnityEditor;
using UnityEngine;

public class NetworkItemContainer : NetworkBehaviour, IItemContainerService, IUniqueNetworkIdService
{
    [SerializeField] private ItemContainerConfig config;
    [SerializeField] private List<ItemContainerSlotUI> slots;
    public ItemContainerConfig Config => config;
    public List<ItemContainerSlotUI> Slots => slots;
    private ItemContainerCore core;

    private NetworkVariable<FixedString128Bytes> containerGuid =
        new(readPerm: NetworkVariableReadPermission.Everyone, writePerm: NetworkVariableWritePermission.Server);

    public NetworkVariable<FixedString128Bytes> UniqueNetworkId => containerGuid;

    public override void OnNetworkSpawn()
    {
        InitializeUniqueNetworkId();
        Initialize(Config, Slots);

        if (IsServer) return;
        RequestContainerDataSyncServerRpc(NetworkManager.Singleton.LocalClientId);
    }

    private void Start()
    {
        ItemContainersManager.Instance.RegisterContainer(this);
    }

    public void InitializeUniqueNetworkId()
    {
        if (!IsServer) return;
        if (!UniqueNetworkId.Value.IsEmpty || UniqueNetworkId.Value.ToString() != "")
            return;
        UniqueNetworkId.Value = Guid.NewGuid().ToString();
    }

    public void Initialize(ItemContainerConfig config, List<ItemContainerSlotUI> slots)
    {
        core = new ItemContainerCore(config, slots, GetContainerGUID());
        core.InitializeSlots(this);
    }

    public string GetContainerGUID()
    {
        return UniqueNetworkId.Value.ToString();
    }

    public bool TryAddItem(string itemId, int amount)
    {
        if (IsServer)
            TryAddItemClientRpc(itemId, amount);
        else
            TryAddItemServerRpc(itemId, amount);
        return true;
    }

    [ServerRpc(RequireOwnership = false)]
    private void TryAddItemServerRpc(string itemId, int amount)
    {
        TryAddItemClientRpc(itemId, amount);
    }

    [ClientRpc]
    private void TryAddItemClientRpc(string itemId, int amount)
    {
        if (core.TryAddItemToStack(itemId, amount)) return;
        core.TryAddItemToEmptySlot(itemId, amount);
    }

    public void ClearSlot(int slotIndex)
    {
        core.ClearSlot(slotIndex);
    }

    public void SetRuntimeData(int slotIndex, string key, object value)
    {
        if (!IsOwner) return;
        // Apply to runtime data locally or send over network
    }

    [ServerRpc(RequireOwnership = false)]
    private void RequestContainerDataSyncServerRpc(ulong clientId)
    {
        if (!Config.IsNetworkSynced) return;

        foreach (var slot in core.Slots)
        {
            var heldItem = slot.GetHeldItem();
            if (heldItem != null)
                SendSlotDataClientRpc(clientId, slot.SlotIndex, heldItem.ItemId, heldItem.Amount,
                    heldItem.SerializeRuntimeData());
        }
    }

    [ClientRpc]
    private void SendSlotDataClientRpc(ulong targetClientId, int slotIndex, string itemId, int amount,
        string runtimeDataJson)
    {
        if (!Config.IsNetworkSynced) return;
        if (NetworkManager.Singleton.LocalClientId != targetClientId) return;


        var slot = core.Slots[slotIndex];
        var runtimeData = ItemContainerItemUI.DeserializeRuntimeData(runtimeDataJson);
        slot.SetHeldItem(new ItemContainerItemUI(itemId, amount, GetContainerGUID(), slotIndex, runtimeData));
    }
}