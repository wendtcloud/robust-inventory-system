using System;
using System.Collections.Generic;
using UnityEngine;

public interface IRuntimeDataService
{
    void InitializeFromRuntimeData(Dictionary<string, object> runtimeData);
}