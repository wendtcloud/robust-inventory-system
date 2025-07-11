using System;
using System.Collections.Generic;
using UnityEngine;

public class HotbarItem : MonoBehaviour, IHotbarItem, IRuntimeDataService
{
    public int hotbarItemIntProperty;
    public int maxRuntimeInt = 20;

    private LocalHotbarContainer hotbar;
    private RuntimeDataOperationService runtimeDataOperationService;

    private void Awake()
    {
        if (runtimeDataOperationService == null) runtimeDataOperationService = new RuntimeDataOperationService();
        runtimeDataOperationService.OnRuntimeDataUpdated += UpdateHotbarRuntimeData;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            // Simple testing
            hotbarItemIntProperty--;
            runtimeDataOperationService.UpdateRuntimeDataValue("hotbarItemIntProperty", hotbarItemIntProperty);
        }
    }

    public void InitializeHotbar(LocalHotbarContainer hotbar)
    {
        this.hotbar = hotbar;
    }

    public void InitializeFromRuntimeData(Dictionary<string, object> runtimeData)
    {
        if (runtimeData.ContainsKey("hotbarItemIntProperty"))
        {
            hotbarItemIntProperty = (int)runtimeData["hotbarItemIntProperty"];
        }
        else
        {
            hotbarItemIntProperty = maxRuntimeInt;
            runtimeDataOperationService.UpdateRuntimeDataValue("hotbarItemIntProperty", hotbarItemIntProperty);
        }
    }

    private void UpdateHotbarRuntimeData(Dictionary<string, object> runtimeData)
    {
        hotbar.UpdateActiveItemRuntimeData(runtimeData);
    }
}