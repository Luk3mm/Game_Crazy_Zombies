using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectilePickup : MonoBehaviour
{
    public int projectileAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerController player = collision.GetComponent<PlayerController>();

            if(player != null)
            {
                player.AddProjectile(projectileAmount);
            }

            Destroy(gameObject);
        }
    }
}
