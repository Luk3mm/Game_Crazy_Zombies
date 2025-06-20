using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ZombieKillUI : MonoBehaviour
{
    public Text enemyCounterText;

    private int totalKill = 0;

    public static ZombieKillUI instance;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void AddKill()
    {
        totalKill++;
        UpdateUI();
    }

    public void ResetKillForHorde()
    {
        UpdateUI();
    }

    private void UpdateUI()
    {
        if(enemyCounterText != null)
        {
            enemyCounterText.text = totalKill.ToString();
        }
    }
}
