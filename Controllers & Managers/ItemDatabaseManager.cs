using System;
using Unity.Collections;
using UnityEngine;

public class ItemDatabaseManager : MonoBehaviour
{
    public static ItemDatabaseManager Instance { get; private set; }

    [Header("Configuration")] [SerializeField]
    private ItemDatabase Items;

    private ItemDatabaseOperationService operationService;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(gameObject);
        }

        if (operationService == null) operationService = new ItemDatabaseOperationService(Items);
    }

    public bool CompareItems(ItemDefinition item1, ItemDefinition item2)
    {
        return operationService.CompareItemDefinitions(item1, item2);
    }

    public ItemDefinition GetItemById(string id)
    {
        return operationService.GetItemDefinition(id);
    }

    public GameObject GetItemPrefabById(string id)
    {
        return operationService.GetItemPrefab(id);
    }
}