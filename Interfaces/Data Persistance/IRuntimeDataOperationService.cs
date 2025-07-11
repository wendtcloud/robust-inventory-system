using System;
using System.Collections.Generic;

public interface IRuntimeDataOperationService
{
    public event Action<Dictionary<string, object>> OnRuntimeDataUpdated;
    void UpdateRuntimeDataValue(string key, object value);
    void UpdateRuntimeData(Dictionary<string, object> runtimeData);
}