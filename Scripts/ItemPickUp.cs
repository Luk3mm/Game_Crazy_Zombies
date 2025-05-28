using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Heal,
    Armor
}

public class ItemPickUp : MonoBehaviour
{
    public ItemType itemType;
    public int healAmount;
    public float armorDuration;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.gameObject.CompareTag("Player"))
        {
            return;
        }

        PlayerController player = collision.GetComponent<PlayerController>();

        if(player == null)
        {
            return;
        }

        switch (itemType)
        {
            case ItemType.Heal:
                player.Heal(healAmount);
                break;

            case ItemType.Armor:
                player.ActiveArmor(armorDuration);
                break;
        }

        Destroy(gameObject);
    }
}
