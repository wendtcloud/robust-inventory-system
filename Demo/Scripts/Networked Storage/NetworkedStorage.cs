using System;
using System.Collections.Generic;
using UnityEngine;

public class NetworkedStorage : NetworkItemContainer
{
    private List<string> items = new() { "metalore", "goldore" };

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            var random = UnityEngine.Random.Range(0, items.Count);
            TryAddItem(items[random], 1);
        }
    }
}