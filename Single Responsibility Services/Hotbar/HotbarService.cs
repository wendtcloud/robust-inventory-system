using System;
using UnityEngine;

public class HotbarService : IHotbarService
{
    private int slot = 0;
    public int ActiveSlot => slot;
    public event Action<int> OnActiveSlotChanged;

    public void SetActiveSlot(int slot)
    {
        this.slot = slot;
        OnActiveSlotChanged?.Invoke(slot);
    }
}