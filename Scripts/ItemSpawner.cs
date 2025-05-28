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

    public ItemSpawnData[] spawnableItens;

    public float spawnInterval;
    public float spawnAreaRadius;
    public Transform[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(AttemptSpawn), spawnInterval, spawnInterval);
    }

    private void AttemptSpawn()
    {
        foreach(var item in spawnableItens)
        {
            if(Random.value <= item.spawnChance)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];

                Vector2 offset = Random.insideUnitCircle * spawnAreaRadius;
                Vector3 spawnPos = spawnPoint.position + (Vector3)offset;

                Instantiate(item.prefab, spawnPos, item.prefab.transform.rotation);
                //break; //Caso queira que seja um por vez;
            }
        }
    }
}
