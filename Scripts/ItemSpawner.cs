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
    public Transform[] spawnPoints;

    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating(nameof(AttemptSpawn), spawnInterval, spawnInterval);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void AttemptSpawn()
    {
        foreach(var item in spawnableItens)
        {
            if(Random.value <= item.spawnChance)
            {
                Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
                Instantiate(item.prefab, spawnPoint.position, item.prefab.transform.rotation);
                //break; //Caso queira que seja um por vez;
            }
        }
    }
}
