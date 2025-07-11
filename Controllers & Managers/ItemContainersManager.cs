using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class ItemContainersManager : MonoBehaviour
{
    public static ItemContainersManager Instance;
    public List<IItemContainerService> Containers = new();

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
    }

    public void RegisterContainer(IItemContainerService container)
    {
        if (!Containers.Contains(container))
            Containers.Add(container);
    }

    public IItemContainerService GetContainerByGUID(FixedString128Bytes GUID)
    {
        foreach (var container in Containers)
            if (container.GetContainerGUID() == GUID)
                return container;
        return null;
    }
}