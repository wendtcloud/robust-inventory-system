using System.Collections.Generic;
using UnityEngine;

public interface IRuntimeDataHolder
{
    public Dictionary<string, object> RuntimeData { get; } // <string, object>
}