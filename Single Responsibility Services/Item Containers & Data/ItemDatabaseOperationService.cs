using UnityEngine;

public class ItemDatabaseOperationService : IItemDatabaseOperationService
{
    private ItemDatabase ItemsDatabase;

    public ItemDatabaseOperationService(ItemDatabase itemDatabase)
    {
        ItemsDatabase = itemDatabase;
    }

    public ItemDefinition GetItemDefinition(string itemID)
    {
        if (ItemsDatabase.Database.TryGetValue(itemID, out var item))
        {
            Debug.Log($"Item of id: {itemID} found in database");
            return item;
        }

        Debug.LogWarning($"Item with id {itemID} not found in database");
        return null;
    }

    public GameObject GetItemPrefab(string itemID)
    {
        var item = GetItemDefinition(itemID);
        if (item == null) return null;
        if (item.Prefab == null) Debug.LogWarning($"There is no prefab for item with id {itemID}");
        return item.Prefab;
    }

    public bool IsItemInDatabase(string itemID)
    {
        var item = GetItemDefinition(itemID);
        return item != null;
    }

    public bool CompareItemDefinitions(ItemDefinition itemDefinition, ItemDefinition otherItemDefinition)
    {
        return itemDefinition == otherItemDefinition;
    }
}