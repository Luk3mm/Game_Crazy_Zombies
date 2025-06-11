using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDespawnTracker : MonoBehaviour
{
    public Action OnDespawn;

    private void OnDestroy()
    {
        OnDespawn?.Invoke();
    }
}
