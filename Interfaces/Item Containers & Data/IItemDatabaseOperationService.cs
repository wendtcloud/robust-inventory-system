using UnityEngine;

public interface IItemDatabaseOperationService
{
    ItemDefinition GetItemDefinition(string itemID);
    GameObject GetItemPrefab(string itemID);
    bool IsItemInDatabase(string itemID);
    bool CompareItemDefinitions(ItemDefinition itemDefinition, ItemDefinition otherItemDefinition);
}