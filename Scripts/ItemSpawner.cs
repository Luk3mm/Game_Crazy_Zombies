using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [System.Serializable]
    public class ItemSpawnData
    {
        public GameObject prefab;
        public ItemType type;
        [Range(0f, 100f)]
        public float spawnChance;
    }

    [System.Serializable]
    public class SpawnPointData
    {
        public Transform point;
        public int maxItens;
        [HideInInspector] public List<GameObject> spawnedItens = new List<GameObject>();
    }

    public ItemSpawnData[] spawnableItens;

    public float spawnInterval;
    public float spawnAreaRadius;
    public SpawnPointData[] spawnPoints;

    [HideInInspector]
    public bool canSpawn = true;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(AttemptSpawn), spawnInterval, spawnInterval);
    }

    private void AttemptSpawn()
    {
        if (!canSpawn)
        {
            return;
        }

        foreach(var point in spawnPoints)
        {
            if(point.spawnedItens.Count > point.maxItens)
            {
                continue;
            }

            foreach(var itemData in spawnableItens)
            {
                if(Random.value <= itemData.spawnChance / 100f)
                {
                    Vector2 offset = Random.insideUnitCircle * spawnAreaRadius;
                    Vector3 spawnPos = point.point.position + (Vector3)offset;

                    GameObject item = Instantiate(itemData.prefab, spawnPos, Quaternion.identity);
                    point.spawnedItens.Add(item);

                    ItemDespawnTracker tracker = item.AddComponent<ItemDespawnTracker>();
                    tracker.OnDespawn += () => point.spawnedItens.Remove(item);

                    //break; //Caso queira que seja um por vez;
                }
            }

        }
    }

    public void ResetSpawnedItems()
    {
        foreach(var point in spawnPoints)
        {
            for(int i = point.spawnedItens.Count - 1; i >= 0; i--)
            {
                if (point.spawnedItens[i] != null)
                {
                    Destroy(point.spawnedItens[i]);
                }
            }

            point.spawnedItens.Clear();
        }
    }

    private void OnDrawGizmosSelected()
    {
        if(spawnPoints == null || spawnPoints.Length == 0)
        {
            return;
        }

        Gizmos.color = Color.green;

        foreach (var point in spawnPoints)
        {
            if(point != null)
            {
                Gizmos.DrawWireSphere(point.point.position, spawnAreaRadius);
            }
        }
    }
}
