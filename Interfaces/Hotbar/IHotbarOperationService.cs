using System;
using UnityEngine;

public interface IHotbarOperationService
{
    public event Action<GameObject> OnHandItemChanged;
    ItemContainerItemUI GetItemInSlot(int slot);
    void UpdateHandItem(int slot);
    void ClearHandItem();
}