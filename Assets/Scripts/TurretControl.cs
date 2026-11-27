
using UnityEngine;

public class TurretControl : MonoBehaviour
{
    public ZombieManager ZombieManager;
    public Transform TurretGunTop;
    public Transform CarTransform;
    public ParticleSystem MuzzleFlash;
    public ParticleSystem ZombieDeathBlood;
    public ScrapMetalManager ScrapMetalManager;

    [Header("Settings")]
    public CurrentTurretLevel TurretLevel = CurrentTurretLevel.Normal;

    public float RotationSpeed = 180f;
    public float ShootingRange = 10f;
    public float AimTolerance = 2f;
    public float ShootingCooldown = 1f;
    public Vector3 AimOffset = new Vector3(0f, 1f, 0f);

    private Transform currentTarget;
    private float nextShootTime = 0f;

    void Update()
    {
        MuzzleFlash.Stop();

        updateTurretStats();

        // Find a new target if we don't have one
        if (currentTarget == null)
        {
            FindClosestZombie();
        }

        if (currentTarget == null)
            return;

        float distance = Vector3.Distance(
            CarTransform.position,
            currentTarget.position
        );

        if (distance > ShootingRange)
        {
            currentTarget = null;
            return;
        }

        Vector3 direction =
            (currentTarget.position + AimOffset) - TurretGunTop.position;

        if (direction.sqrMagnitude < 0.001f)
            return;

        // Red X is forward
        Quaternion targetRotation =
            Quaternion.LookRotation(direction.normalized, Vector3.up)
            * Quaternion.Euler(0f, -90f, 0f);

        // Rotate towards target
        TurretGunTop.rotation = Quaternion.RotateTowards(
            TurretGunTop.rotation,
            targetRotation,
            RotationSpeed * Time.deltaTime
        );

        // Check if turret is aimed at zombie
        float angle = Quaternion.Angle(
            TurretGunTop.rotation,
            targetRotation
        );

        // Shoot when aimed and cooldown is finished
        if (angle <= AimTolerance && Time.time >= nextShootTime)
        {
            Shoot();
        }
    }

    void updateTurretStats()
    {
        switch (TurretLevel)
        {
            case CurrentTurretLevel.Normal:
                RotationSpeed = 180f;
                ShootingRange = 10f;
                AimTolerance = 2f;
                ShootingCooldown = 1f;
                break;
            case CurrentTurretLevel.Improved:
                RotationSpeed = 200f;
                ShootingRange = 12f;
                AimTolerance = 1.5f;
                ShootingCooldown = 0.5f;
                break;
            case CurrentTurretLevel.Reinforced:
                RotationSpeed = 250f;
                ShootingRange = 14f;
                AimTolerance = 1.2f;
                ShootingCooldown = 0.25f;
                break;
            case CurrentTurretLevel.Armored:
                RotationSpeed = 350f;
                ShootingRange = 16f;
                AimTolerance = 1f;
                ShootingCooldown = 0.15f;
                break;
            case CurrentTurretLevel.Military:
                RotationSpeed = 500f;
                ShootingRange = 18f;
                AimTolerance = 0.8f;
                ShootingCooldown = 0.05f;
                break;
        }
    }

    void FindClosestZombie()
    {
        float closestDistance = Mathf.Infinity;

        foreach (var zombie in ZombieManager.Zombies)
        {
            if (zombie == null)
                continue;

            float distance = Vector3.Distance(
                CarTransform.position,
                zombie.transform.position
            );

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentTarget = zombie.transform;
            }
        }
    }

    void Shoot()
    {
        if (currentTarget == null)
            return;

        Debug.Log("Zombie killed: " + currentTarget.name);

        MuzzleFlash.Play();

        // Play blood effect at zombie's position
        if (ZombieDeathBlood != null)
        {
            ParticleSystem bloodEffect = Instantiate(
                ZombieDeathBlood,
                currentTarget.position,
                ZombieDeathBlood.transform.rotation
            );
            bloodEffect.Play();
            Destroy(bloodEffect.gameObject, bloodEffect.main.duration);
        }

        Destroy(currentTarget.gameObject);

        currentTarget = null;

        // Start shooting cooldown
        nextShootTime = Time.time + ShootingCooldown;
    }

    public void UpgradeTurret()
    {
        if (TurretLevel < CurrentTurretLevel.Military && ScrapMetalManager.ScrapMetalCount > ((int)TurretLevel * 10))
        {
            TurretLevel++;
            Debug.Log("Turret upgraded to: " + TurretLevel);
        }
        else
        {
            Debug.Log("Turret is already at maximum level or not enough scrap metal.");
        }
    }
}

public enum CurrentTurretLevel
{
    Normal,
    Improved,
    Reinforced,
    Armored,
    Military
}
