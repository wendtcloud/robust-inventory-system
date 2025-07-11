using UnityEngine;
using UnityEngine.InputSystem;

public interface IHotbarInputHandler
{
    void HandleActiveSlotSwap(int slot);
    void OnHotbarActiveSlotSwap(InputValue value);
}