using System;
using System.Collections.Generic;
using UnityEngine;

public class RuntimeDataOperationService : IRuntimeDataOperationService, IRuntimeDataService
{
    public Dictionary<string, object> RuntimeData = new();

    public event Action<Dictionary<string, object>> OnRuntimeDataUpdated;

    public void UpdateRuntimeDataValue(string key, object value)
    {
        RuntimeData[key] = value;
        OnRuntimeDataUpdated?.Invoke(RuntimeData);
    }

    public void UpdateRuntimeData(Dictionary<string, object> runtimeData)
    {
        RuntimeData = runtimeData;
    }

    public void InitializeFromRuntimeData(Dictionary<string, object> runtimeData)
    {
        RuntimeData = runtimeData;
    }
}