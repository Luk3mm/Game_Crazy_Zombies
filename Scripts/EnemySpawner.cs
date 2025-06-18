using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    public GameObject enemyPrefab;
    public float spawnInterval;
    public int maxEnemies;

    [Header("Tilemap Settings")]
    public Tilemap spawnTilemap;
    public LayerMask obstacleLayer;

    [Header("Debug")]
    public bool showGizmos = true;
    public Color gizmoColor = Color.red;

    [HideInInspector]
    public bool canSpawn = true;
    private List<Vector3> spawnPositions = new List<Vector3>();
    private int currentEnemies;

    // Start is called before the first frame update
    void Start()
    {
        CollectSpawnPositions();
        StartCoroutine(SpawnRoutine());
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    private void CollectSpawnPositions()
    {
        spawnPositions.Clear();

        foreach(var pos in spawnTilemap.cellBounds.allPositionsWithin)
        {
            if (!spawnTilemap.HasTile(pos))
            {
                continue;
            }

            Vector3 worldPos = spawnTilemap.CellToWorld(pos) + spawnTilemap.tileAnchor;

            if(!Physics2D.OverlapCircle(worldPos, 0.2f, obstacleLayer))
            {
                spawnPositions.Add(worldPos);
            }
        }
    }

    IEnumerator SpawnRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(spawnInterval);

            if(!canSpawn || currentEnemies >= maxEnemies || spawnPositions.Count == 0)
            {
                continue;
            }

            Vector3 spawnPos = spawnPositions[Random.Range(0, spawnPositions.Count)];
            GameObject enemy = Instantiate(enemyPrefab, spawnPos, Quaternion.identity);

            EnemyController ec = enemy.GetComponent<EnemyController>();

            if(ec != null)
            {
                ec.OnDeath += OnEnemyDeath;
            }

            currentEnemies++;
        }
    }

    private void OnEnemyDeath()
    {
        currentEnemies = Mathf.Max(0, currentEnemies - 1);
        FindObjectOfType<HorderManager>()?.RegisterEnemyDefeat();
    }

    private void OnDrawGizmos()
    {
        if(!showGizmos || spawnTilemap == null)
        {
            return;
        }

        Gizmos.color = gizmoColor;

        foreach(var pos in spawnTilemap.cellBounds.allPositionsWithin)
        {
            if (spawnTilemap.HasTile(pos))
            {
                Vector3 worldPos = spawnTilemap.CellToWorld(pos) + spawnTilemap.tileAnchor;
                Gizmos.DrawWireCube(worldPos, Vector3.one * 0.5f);
            }
        }
    }
}
