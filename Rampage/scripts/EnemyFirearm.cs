using System;
using System.Collections;
using UnityEngine;
using Util;

public class EnemyFirearm : MonoBehaviour
{
    [Header("Weapon Stats")]

    public GameObject bulletPrefab;
    Enemy enemy;
    public float accuracy = 5; // spread per 10 units (point - 0, circle - 40)
    public float firerate = 300; // shots per minute
    public Vector2 bulletSpawnOffsetUnits = new(0f,0f); // distance between weapon object and spawned bullet
  
    public float magazineCapasity = 100; // amount of bullets (-100 for infinity)
    public float reloadTime = 2f; 
    public float bulletsPerShot = 1; // number of bullets shot at a time
    public float defaultWeaponSpeed = 100;
    public float speedWhenShooting = 30;
    public float speedWhenReloading = 20;
    public float numberOfFixedShootingFrames = 10;
    public Vector3 weaponOffset = Vector3.zero;
    public Vector3 handsGripOffset = Vector3.zero;

    float shootingAngle;
    float shotCooldown;
    [NonSerialized] public float cooldownTillShot = 0;
    [NonSerialized] public float bulletsLeft;
    float shootingFramesLeft = 0;

    [Header("Sound Effects")]

    public AudioClip shotSound;
    public float shotVolume;
    public float shotPitch;
    public AudioSource reloadAudioSource;
    public GameObject audioSourcePrefab;

    int shotSourceToPlay = 0;
    const int numberOfShotAudioSources = 2;
    GameObject[] shotAudioObjects = new GameObject[numberOfShotAudioSources];

    AudioSource[] shotSources = new AudioSource[numberOfShotAudioSources];
 
    [Header("Misc")]

    public bool invertX = false;
    public bool invertY = false;
    bool isReloading = false;

    public const float RADS = (float)(Math.PI / 180);
    public const float EULER = (float)(180f / Math.PI);


    private void Awake()
    {
        enemy = GetComponentInParent<Enemy>();
    }
    void Start()
    {
        shotCooldown = 60 / firerate;
        accuracy = Mathf.Clamp(accuracy, 0, 40);
        magazineCapasity = Mathf.Clamp(magazineCapasity, -100, 10000);
        bulletsLeft = magazineCapasity;
        for (int i = 0; i < numberOfShotAudioSources; i++)
        {
            shotAudioObjects[i] = Instantiate(audioSourcePrefab, gameObject.transform.position, Quaternion.Euler(0, 0, 0), gameObject.transform);
            shotSources[i] = shotAudioObjects[i].GetComponent<AudioSource>();
            shotSources[i].volume = shotVolume;
            shotSources[i].pitch = shotPitch;
            shotSources[i].clip = shotSound;
            shotSources[i].rolloffMode = AudioRolloffMode.Linear;
            shotSources[i].spatialBlend = 1f;
        }
    }

    void Update()
    {
        Vector3 woCenter = enemy.transform.position;
        woCenter.y += weaponOffset.y;
        woCenter.z = -0.02f;
        transform.position = Utils.PointOnCircle(woCenter, weaponOffset.x, shootingAngle);

        if (cooldownTillShot > 0)
            cooldownTillShot -= Time.deltaTime;

        shootingAngle = Utils.AngleBetweenPositions2D(transform.position, enemy.targetPos, invertX, invertY);

            // Weapon Rotation
        if ((shootingAngle <= 90 && shootingAngle >= -90))
            {
                if (invertX)
                    transform.rotation = Quaternion.Euler(180, 0, -shootingAngle);
                else 
                    transform.rotation = Quaternion.Euler(0, 0, shootingAngle);
            }
        else
            {
                if (invertX)
                    transform.rotation = Quaternion.Euler(0, 0, shootingAngle);
                else
                    transform.rotation = Quaternion.Euler(180, 0, -shootingAngle);
            }

            // reload
        if (bulletsLeft <= 0 && bulletsLeft >= -10 && !isReloading && shootingFramesLeft <= 0)
        {
            StartCoroutine(Reload());
            isReloading = true;
        }
       
    }

    private void FixedUpdate()
    {
        // Speed change when shooting
        if (shootingFramesLeft > 0)
        {
            if (shootingFramesLeft == numberOfFixedShootingFrames)
                enemy.dynamicSpeed = enemy.speed * defaultWeaponSpeed * speedWhenShooting / 10000;
            shootingFramesLeft--;
        }
        else if (isReloading)
            enemy.dynamicSpeed = enemy.speed * defaultWeaponSpeed * speedWhenReloading / 10000;
        else if (!isReloading)
            enemy.dynamicSpeed = enemy.speed * defaultWeaponSpeed / 100;

    }

    public IEnumerator Reload()
    {
        reloadAudioSource.Play();
        yield return new WaitForSeconds(reloadTime);

        bulletsLeft = magazineCapasity;
        isReloading = false;
        yield return null;
    }
    public void Shot(float shootingAngle)
    {
        shootingFramesLeft = numberOfFixedShootingFrames;
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Quaternion bulletRotation = Quaternion.Euler(0, 0, AccuracyRandom(accuracy) + shootingAngle);
            Vector3 boCenter = transform.position;
            boCenter.y += bulletSpawnOffsetUnits.y;
            Vector3 bulletPosition = Utils.PointOnCircle(boCenter, bulletSpawnOffsetUnits.x, shootingAngle, invertX, invertY);

            if(bulletsLeft > 0 || bulletsLeft <= -10)
            {
                Instantiate(bulletPrefab, bulletPosition, bulletRotation);
            }
        }
        if (bulletsLeft > 0)
            bulletsLeft--;

        cooldownTillShot = shotCooldown;

        shotSources[shotSourceToPlay].Play();
        shotSourceToPlay++;
        shotSourceToPlay %= numberOfShotAudioSources;
        
    }
    float AccuracyRandom(float accuracy)
    {
        return UnityEngine.Random.Range(-accuracy * 4.5f, accuracy * 4.5f);
    }
    public float GetShootingAngle()
    {
        return shootingAngle;
    }
}
