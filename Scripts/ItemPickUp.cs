using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    Heal,
    Armor,
    SpeedBoost
}

public class ItemPickUp : MonoBehaviour
{
    public ItemType itemType;

    public int healAmount;
    public float armorDuration;

    public float speedBoostAmount;
    public float speedBoostDuration;

    private AudioSource pickupAudio;

    private void Awake()
    {
        pickupAudio = GetComponent<AudioSource>();
    }

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

            case ItemType.SpeedBoost:
                player.ActiveSpeedBoost(speedBoostAmount, speedBoostDuration);
                break;
        }

        if(pickupAudio != null && pickupAudio.clip != null)
        {
            AudioSource.PlayClipAtPoint(pickupAudio.clip, transform.position);
        }

        Destroy(gameObject);
    }
}
