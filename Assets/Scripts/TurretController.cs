using System.Collections;
using UnityEngine;

public class TurretController : MonoBehaviour
{
    public GameObject projectilePrefab; // your projectile prefab
    public Transform shootPoint; // the point where the projectile is spawned
    public float bulletSpeed = 5f;
    public float shootCooldown = 1f; // adjust the rate of fire

    private Collider2D visionCollider; // reference to the collider

    private Transform player; // the player's transform
    public bool isPlayerInVisionRange = false; // flag to check if player is in vision range
    private bool isCooldownActive = false; // flag to check if cooldown is active

    private Vector3 direction;

    void Start()
    {
        // find the player's transform
        player = GameObject.FindGameObjectWithTag("Player").transform;

        // find the collider
        visionCollider = GameObject.Find("VisionCollider").GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        // check if the player has entered the vision range
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInVisionRange = true;
            // start shooting coroutine
            StartCoroutine(ShootContinuously());
        }
    }

    void OnTriggerExit2D(Collider2D collision)
    {
        // check if the player has exited the vision range
        if (collision.gameObject.CompareTag("Player"))
        {
            isPlayerInVisionRange = false;
        }
    }

    void RotateTurret()
    {
        // calculate the direction from the turret to the player
        direction = player.position - transform.position;

        // calculate the angle between the turret and the player
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // set the turret's rotation
        transform.eulerAngles = new Vector3(0, 0, angle);
    }

    void Shoot()
    {
        // instantiate the projectile
        GameObject projectile = Instantiate(projectilePrefab, shootPoint.position, shootPoint.rotation);

        // set the projectile's velocity
        projectile.GetComponent<Rigidbody2D>().velocity = direction.normalized * bulletSpeed;
    }

    public IEnumerator ShootContinuously()
    {
        while (isPlayerInVisionRange)
        {
            RotateTurret(); // rotate the turret towards the player continuously
            if (!isCooldownActive)
            {
                Shoot();
                // start the cooldown coroutine
                StartCoroutine(ShootCooldown());
            }
            yield return null;
        }
    }

    IEnumerator ShootCooldown()
    {
        isCooldownActive = true;
        yield return new WaitForSeconds(shootCooldown);
        isCooldownActive = false;
    }
}