using System;
using System.Collections;
using UnityEngine;
using Util;

public class Firearm : MonoBehaviour
{
    public static Firearm instance;

    [Header("Weapon Stats")]

    public GameObject bulletPrefab;
    public float accuracy = 5; // spread per 10 units (point - 0, circle - 40)
    public float firerate = 300; // shots per minute
    public Vector2 bulletSpawnOffsetUnits = new(6f, 0f); // distance between weapon object and spawned bullet

    public float magazineCapasity = 100; // amount of bullets (-100 for infinity)
    public float reloadTime = 2f;
    public float bulletsPerShot = 1; // number of bullets shot at a time
    public float defaultWeaponSpeed = 100;
    public float speedWhenShooting = 30;
    public float speedWhenReloading = 20;
    public float numberOfFixedShootingFrames = 10;
    public Vector3 weaponOffset = Vector3.zero;
    public Vector3 handsGripOffset = Vector3.zero;

    float baseFirerate;
    float baseReloadTime;
    float baseAccuracy;
    float baseMagazineCapasity;
    float shootingAngle;
    float shotCooldown;
    float cooldownTillShot = 0;
    [NonSerialized] public float bulletsLeft;
    float shootingFramesLeft = 0;

    [Header("Sound Effects")]

    public AudioClip shotSound;
    public float shotVolume;
    public float shotPitch;
    public AudioSource reloadAudioSource;
    public GameObject audioSourcePrefab;
    float reloadAudioPitch;

    int shotSourceToPlay = 0;
    const int numberOfShotAudioSources = 10;
    GameObject[] shotAudioObjects = new GameObject[numberOfShotAudioSources];

    AudioSource[] shotSources = new AudioSource[numberOfShotAudioSources];

    [Header("Misc")]

    public bool invertX = false;
    public bool invertY = false;
    bool isReloading = false;
    bool canShoot = true;
    [NonSerialized] public bool pointerOverButton = false;

    public const float RADS = (float)(Math.PI / 180);
    public const float EULER = (float)(180f / Math.PI);


    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        reloadAudioPitch = reloadAudioSource.pitch;
        baseFirerate = firerate;
        baseReloadTime = reloadTime;
        baseAccuracy = accuracy;
        baseMagazineCapasity = magazineCapasity;
        RecalcValues();
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
        UI.instance.UpdateAmmoBar();
    }

    void Update()
    {

        Vector3 woCenter = Player.instance.GetPlayerPos();
        woCenter.y += weaponOffset.y;
        transform.position = Utils.PointOnCircle(woCenter, weaponOffset.x, shootingAngle);

        Player.instance.wpScript = this;
        if (cooldownTillShot > 0)
            cooldownTillShot -= Time.deltaTime;


        // reload
        if (bulletsLeft <= 0 && bulletsLeft >= -10 && !isReloading && shootingFramesLeft <= 0)
        {
            StartCoroutine(Reload());
            isReloading = true;
        }

    }

    private void FixedUpdate()
    {
        if (Player.instance.isAlive)
        {
            if (!Input.GetMouseButton(0))
            {
                canShoot = true;
            }
            if (pointerOverButton)
                canShoot = false;

            // LMB
            if (Input.GetMouseButton(0) && cooldownTillShot <= 0 && bulletsLeft > 0 && !isReloading && canShoot)
            {
                Shot(shootingAngle);
            }

            // Speed change when shooting
            if (shootingFramesLeft > 0)
            {
                if (shootingFramesLeft == numberOfFixedShootingFrames)
                    Player.instance.dynamicSpeed = PlayerStats.Instance.baseSpeed * defaultWeaponSpeed * speedWhenShooting / 10000;
                shootingFramesLeft--;
            }
            else if (isReloading)
                Player.instance.dynamicSpeed = PlayerStats.Instance.baseSpeed * defaultWeaponSpeed * speedWhenReloading / 10000;
            else if (!isReloading)
                Player.instance.dynamicSpeed = PlayerStats.Instance.baseSpeed * defaultWeaponSpeed / 100;

            // Weapon Rotation
            shootingAngle = Utils.AngleBetweenPositionAndMouse(transform.position, invertX, invertY);
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
        }
    }

    public void ReloadCheck()
    {
        // manual reload 
        if (bulletsLeft < magazineCapasity && bulletsLeft >= -10 && !isReloading && shootingFramesLeft <= 0)
        {
            StartCoroutine(Reload());
            isReloading = true;
        }
    }
    public IEnumerator Reload()
    {
        reloadAudioSource.Play();
        yield return new WaitForSeconds(reloadTime);

        RecalcValues();
        bulletsLeft = magazineCapasity;
        UI.instance.UpdateAmmoBar();
        isReloading = false;

        yield return null;
    }
    void Shot(float shootingAngle)
    {
        shootingFramesLeft = numberOfFixedShootingFrames;
        for (int i = 0; i < bulletsPerShot; i++)
        {
            Quaternion bulletRotation = Quaternion.Euler(0, 0, AccuracyRandom(accuracy) + shootingAngle);
            Vector3 bulletPosition = Utils.PointOnCircle(new(transform.position.x, transform.position.y + bulletSpawnOffsetUnits.y, transform.position.z), bulletSpawnOffsetUnits.x, shootingAngle, invertX, invertY);

            if (bulletsLeft > 0 || bulletsLeft <= -10)
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
        UI.instance.UpdateAmmoBar();
    }
    float AccuracyRandom(float accuracy)
    {
        return UnityEngine.Random.Range(-accuracy * 4.5f, accuracy * 4.5f);
    }
    public float GetShootingAngle()
    {
        return shootingAngle;
    }
    public void RecalcValues()
    {
        firerate = baseFirerate * PlayerStats.Instance.multFirerate;
        reloadTime = baseReloadTime / PlayerStats.Instance.multReloadTime;
        reloadAudioSource.pitch = reloadAudioPitch * PlayerStats.Instance.multReloadTime;
        magazineCapasity = Mathf.Floor((baseMagazineCapasity + PlayerStats.Instance.baseMagazine) * PlayerStats.Instance.multMagazine);
        accuracy = Mathf.Clamp(baseAccuracy / PlayerStats.Instance.multAccuracy, 0, 40);
        shotCooldown = 60 / firerate;
    }
}
