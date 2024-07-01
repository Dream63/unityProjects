using System;
using UnityEngine;
using UnityEngine.UI;
using Util;

public class Player : MonoBehaviour
{
    public static Player instance;

    public Arms Larm, Rarm;
    public GameObject head;

    public GameObject weaponSlot;
    public GameObject startingWeapon;
    GameObject weapon;
    [NonSerialized] public Firearm wpScript;

    Animator playerMovementAnimator;
    public AudioSource enemyDeathAudioSource, playerDamagedAudioSource;

    public float score = 0;
    public float exp = 0;
    public float health;
    [NonSerialized] public int level = 0;
    [NonSerialized] public int expReqToLvlup = 5;
    public int lvlupCardLeft = 0;
    public int statPointsLeft = 0;
    public int invincibilityTicks = 50;
    int invincibilityTicksLeft = 0;
    public bool isAlive= true;
    public bool debug = false;

    [NonSerialized] public float dynamicSpeed = 50;
    Vector3 playerPos;
    Vector3 frameMovement;

    private void Awake()
    {
        instance = this;
    }
    void Start()
    {
        health = Mathf.Floor(PlayerStats.Instance.baseHp * PlayerStats.Instance.multHp);
        playerPos = transform.position;
        playerMovementAnimator = gameObject.GetComponentInChildren<Animator>();
        weapon = new GameObject("empty");
        SetWeapon(startingWeapon);
        UI.instance.UpdateExpBar();
        UI.instance.UpdateHpBar();
    }
    void Update()
    {
        if (isAlive)
        {
            // Manual reload
            if (Input.GetKeyDown(KeyCode.R))
            {
                Firearm.instance.ReloadCheck();
            }

            // Movement
            float frameSpeed = dynamicSpeed * Time.deltaTime * PlayerStats.Instance.multSpeed;
            frameMovement = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                frameMovement.y += frameSpeed;
            }
            if (Input.GetKey(KeyCode.S))
            {
                frameMovement.y -= frameSpeed;
            }
            if (Input.GetKey(KeyCode.D))
            {
                frameMovement.x += frameSpeed;
            }
            if (Input.GetKey(KeyCode.A))
            {
                frameMovement.x -= frameSpeed;
            }
            if (Input.GetKeyUp(KeyCode.E))
            {
                if (UI.instance.cardField.activeSelf == false)
                    UI.instance.GetCardHand();
            }
            if (Input.GetKeyUp(KeyCode.F))
            {
                if (UI.instance.statMenu.activeSelf == true)
                    UI.instance.StatMenuClose();
                else UI.instance.StatMenuOpen();
            }
            if (Input.GetKeyUp(KeyCode.Escape))
            {
                if (UI.instance.pauseMenu.activeSelf == true)
                    UI.instance.ClosePause();
                else UI.instance.OpenPause();
            }

            if(debug)
            {
                if(Input.GetKeyUp(KeyCode.V))
                {
                    GameManager.Instance.StartNewWave();
                }
            }
            
            if (frameMovement.x != 0 && frameMovement.y != 0)
            {
                frameMovement.x /= 1.414f;
                frameMovement.y /= 1.414f;
            }

            // Animation change
            if (true)
            {
                bool movesRight = false, looksRight = false, moves = false;
                float shotingAngle = Firearm.instance.GetShootingAngle();
                looksRight = (shotingAngle < 90) && (shotingAngle > -90);
                //Debug.Log(shotingAngle);

                // Movement
                if (frameMovement != Vector3.zero)
                {
                    moves = true;
                    if (frameMovement.x > 0)
                        movesRight = true;
                    if (frameMovement.x < 0)
                        movesRight = false;
                }
                else
                {
                    moves = false;
                    movesRight = false;
                }

                // Set animator variables
                if (moves)
                    playerMovementAnimator.SetBool("Moving backwards", looksRight != movesRight);
                playerMovementAnimator.SetBool("Player moving", moves);

                transform.rotation = Quaternion.Euler(0, looksRight ? 180 : 0, 0);
                Larm.invertY = !looksRight;
                Rarm.invertY = looksRight;


                // Head tilt, hands invert
                if (shotingAngle < -90)
                {
                    head.transform.rotation = Quaternion.Euler(0, looksRight ? 180 : 0, (shotingAngle + 180) / 5);
                }
                else if (shotingAngle > 90)
                {
                    head.transform.rotation = Quaternion.Euler(0, looksRight ? 180 : 0, (shotingAngle - 180) / 5);
                }
                else
                {
                    head.transform.rotation = Quaternion.Euler(0, looksRight ? 180 : 0, -shotingAngle / 5);
                }
                Vector3 hgoCenter = new(transform.position.x, transform.position.y + wpScript.handsGripOffset.y, transform.position.z);
                Larm.endPos = Utils.PointOnCircle(hgoCenter, wpScript.handsGripOffset.x, shotingAngle);
                Rarm.endPos = Utils.PointOnCircle(hgoCenter, wpScript.handsGripOffset.x, shotingAngle);
            }

            transform.position = Utils.Clamp(transform.position + frameMovement, CameraManager.Instance.cameraMin + Vector2.one, CameraManager.Instance.cameraMax - Vector2.one);
            playerPos = transform.position;
        }
    }
    private void FixedUpdate()
    {
        if (invincibilityTicksLeft > 0)
            invincibilityTicksLeft--;
    }

    public void AddExp(float amount)
    {
        exp += amount * PlayerStats.Instance.multExp;

        while(exp >= expReqToLvlup)
        {
            exp -= expReqToLvlup;
            expReqToLvlup = Mathf.FloorToInt(expReqToLvlup * 1.1f) + 5;
            level++;
            statPointsLeft++;
            if (level % 3 == 0)
            {
                lvlupCardLeft++;
                statPointsLeft += 2;
                UI.instance.cardButton.GetComponent<Image>().color = new(1f, 1f, 1f, 1f);
            }
            StartCoroutine(UI.instance.LvlUpAnouncement());
            if(PlayerStats.Instance.CheapestShopPrice() <= statPointsLeft)
                UI.instance.statButton.GetComponent<Image>().color = new(1f,1f,1f,1f);
        }
        UI.instance.UpdateExpBar();
    }
    public Vector3 GetPlayerPos()
    {
        return playerPos;
    }
    public void SetWeapon(GameObject weaponPrefab) 
    {
        Destroy(weapon);
        weapon = Instantiate(weaponPrefab, playerPos, Quaternion.identity, weaponSlot.transform);
        wpScript = GetComponentInChildren<Firearm>();
    }
    public void TakeDamage(float damage)
    {
        health -= damage;
        invincibilityTicksLeft = invincibilityTicks;
        if(damage > 0)
        playerDamagedAudioSource.Play();
        if (health <= 0)
            GameManager.Instance.GameOver();
        Player.instance.health = Mathf.Clamp(Player.instance.health, 0, PlayerStats.Instance.baseHp * PlayerStats.Instance.multHp);
        UI.instance.UpdateHpBar();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Enemy enemy) && invincibilityTicksLeft == 0)
            TakeDamage(enemy.damage);

    }
}
