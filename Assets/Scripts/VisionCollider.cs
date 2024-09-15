using System.Collections;
using UnityEngine;

public class VisionCollider : MonoBehaviour
{
    private TurretController turretController; // reference to the TurretController script

    void Start()
    {
        // find the TurretController script
        turretController = GameObject.Find("Turret").GetComponent<TurretController>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // check if the player has entered the vision range
        if (collision.gameObject.CompareTag("Player"))
        {
            turretController.isPlayerInVisionRange = true;
            // start shooting coroutine
            turretController.StartCoroutine(turretController.ShootContinuously());
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // check if the player has exited the vision range
        if (collision.gameObject.CompareTag("Player"))
        {
            turretController.isPlayerInVisionRange = false;
        }
    }
}