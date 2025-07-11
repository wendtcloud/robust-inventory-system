using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;
using Unity.Netcode;

public class ItemContainerItemUI : IRuntimeDataHolder
{
    public FixedString128Bytes ContainerGUID;
    public int SlotIndex;
    public string ItemId;
    public int Amount;
    public RuntimeDataOperationService RuntimeDataOperationService = new();

    public Dictionary<string, object> RuntimeData => RuntimeDataOperationService.RuntimeData;

    public ItemContainerItemUI(string itemId, int amount, FixedString128Bytes containerGUID, int slotIndex,
        Dictionary<string, object> runtimeData = null)
    {
        ItemId = itemId;
        Amount = amount;
        ContainerGUID = containerGUID;
        SlotIndex = slotIndex;
        RuntimeDataOperationService.UpdateRuntimeData(runtimeData ?? new Dictionary<string, object>());
    }

    public ItemContainerItemUI Clone()
    {
        return new ItemContainerItemUI(
            ItemId,
            Amount,
            new FixedString128Bytes(""), // prevents FixedString NullReferenceException
            -1,
            RuntimeData
        );
    }

    // Helper method to serialize RuntimeData to JSON
    public string SerializeRuntimeData()
    {
        if (RuntimeData == null || RuntimeData.Count == 0)
            return "{}";

        try
        {
            return JsonUtility.ToJson(new SerializableRuntimeData(RuntimeData));
        }
        catch
        {
            return "{}";
        }
    }

    // Helper method to deserialize RuntimeData from JSON
    public static Dictionary<string, object> DeserializeRuntimeData(string json)
    {
        if (string.IsNullOrEmpty(json) || json == "{}")
            return new Dictionary<string, object>();

        try
        {
            var serializable = JsonUtility.FromJson<SerializableRuntimeData>(json);
            return serializable.ToDictionary();
        }
        catch
        {
            return new Dictionary<string, object>();
        }
    }

    // Enhanced serializable wrapper for RuntimeData with type preservation
    [System.Serializable]
    private class SerializableRuntimeData
    {
        public List<string> keys = new();
        public List<string> values = new();
        public List<string> types = new(); // Track the original types

        public SerializableRuntimeData(Dictionary<string, object> data)
        {
            foreach (var kvp in data)
            {
                keys.Add(kvp.Key);
                values.Add(kvp.Value?.ToString() ?? "");
                types.Add(kvp.Value?.GetType().Name ?? "String");
            }
        }

        public Dictionary<string, object> ToDictionary()
        {
            var result = new Dictionary<string, object>();
            for (var i = 0; i < keys.Count && i < values.Count && i < types.Count; i++)
            {
                var key = keys[i];
                var value = values[i];
                var type = types[i];

                // Convert back to original type
                var convertedValue = ConvertStringToType(value, type);
                result[key] = convertedValue;
            }

            return result;
        }

        private object ConvertStringToType(string value, string typeName)
        {
            if (string.IsNullOrEmpty(value))
                return null;

            try
            {
                switch (typeName)
                {
                    case "Int32":
                        return int.Parse(value);
                    case "Single":
                        return float.Parse(value);
                    case "Double":
                        return double.Parse(value);
                    case "Boolean":
                        return bool.Parse(value);
                    case "String":
                    default:
                        return value;
                }
            }
            catch
            {
                // If conversion fails, return as string
                return value;
            }
        }
    }
}