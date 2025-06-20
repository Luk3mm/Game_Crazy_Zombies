using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HorderManager : MonoBehaviour
{
    [System.Serializable]
    public class Horde
    {
        public int enemiesToDefeat;
        public float enemySpawnInterval;
        public int maxEnemies;
        public float itemSpawnInterval;
        public float itemSpawnChance;
    }

    public Horde[] hordes;
    public EnemySpawner enemySpawner;
    public ItemSpawner itemSpawner;

    [Header("Transition Hordes")]
    public GameObject hordeTransitionPanel;
    public float intermissionDuration;

    private int currentHorderIndex;
    private int enemiesDefeated;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(StartHorde());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    IEnumerator StartHorde()
    {
        if(currentHorderIndex >= hordes.Length)
        {
            LoadEndScene();
            yield break;
        }

        enemiesDefeated = 0;
        enemySpawner.ResetSpawner();
        itemSpawner.ResetSpawnedItems();

        Horde horde = hordes[currentHorderIndex];

        enemySpawner.spawnInterval = horde.enemySpawnInterval;
        enemySpawner.maxEnemies = horde.maxEnemies;
        itemSpawner.spawnInterval = horde.itemSpawnInterval;
        foreach(var item in itemSpawner.spawnableItens)
        {
            item.spawnChance = horde.itemSpawnChance;
        }

        enemySpawner.canSpawn = true;
        itemSpawner.canSpawn = true;

        ZombieKillUI.instance?.ResetKillForHorde();

        yield return null;
    }

    public void RegisterEnemyDefeat()
    {
        ZombieKillUI.instance?.AddKill();

        enemiesDefeated++;

        if(currentHorderIndex < hordes.Length && enemiesDefeated >= hordes[currentHorderIndex].enemiesToDefeat)
        {
            StartCoroutine(HandleHordeEnd());
        }
    }

    IEnumerator HandleHordeEnd()
    {
        enemySpawner.canSpawn = false;
        itemSpawner.canSpawn = false;

        enemySpawner.ResetSpawner();
        itemSpawner.ResetSpawnedItems();

        if(hordeTransitionPanel != null)
            hordeTransitionPanel.SetActive(true);

        yield return new WaitForSeconds(intermissionDuration);

        if(hordeTransitionPanel != null)
            hordeTransitionPanel.SetActive(false);

        currentHorderIndex++;
        enemiesDefeated = 0;

        StartCoroutine(StartHorde());
    }

    private void LoadEndScene()
    {
        SceneManager.LoadScene(2);
    }
}
