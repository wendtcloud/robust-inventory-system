using AYellowpaper.SerializedCollections;
using UnityEngine;

[CreateAssetMenu(fileName = "Item Database", menuName = "Inventory System/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] [SerializedDictionary("Item ID", "Item")]
    public SerializedDictionary<string, ItemDefinition> Database;
}