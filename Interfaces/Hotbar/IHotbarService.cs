using System;

public interface IHotbarService
{
    int ActiveSlot { get; }
    public event Action<int> OnActiveSlotChanged;
    void SetActiveSlot(int slot);
}