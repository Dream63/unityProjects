using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.Intrinsics;
using UnityEngine;
using Util;

public class Enemy : MonoBehaviour
{
    public static Enemy instance;
    public GameObject head;
    public bool defaultEnemyAI= true;
    public bool sniperEnemyAI = false;
    public bool shieldEnemyAI = false;
    public float speed;
    public float health;
    public float sturdiness = 1;
    public float damage;
    public float expDrop;
    bool isAlive = true;

    Animator enemyMovementAnimator;


    [Header("Sniper")]
    public GameObject sniperRifle;
    public EnemyFirearm srScript;
    public Arms larm, rarm;
    public float distanceToShot = 40;
    [NonSerialized] public float dynamicSpeed;
    bool aiming = false;
    public Vector3 targetPos;
    Animator animator;

    [Header("Shield")]
    public GameObject shield;
    public Vector3 weaponOffset;
    Enemy holder;

    private void Start()
    {
        if(sniperEnemyAI)
            animator = GetComponentInChildren<Animator>();
        if(shieldEnemyAI)
            holder = gameObject.GetComponentInParent<Enemy>();
    }

    void Update()
    {
        float shotingAngle = Utils.AngleBetweenPositions2D(transform.position, targetPos);
        bool looksRight = (shotingAngle < 90) && (shotingAngle > -90);

        if (defaultEnemyAI)
        {
            // Enemy move
            Vector3 distanceToTarget;
            Vector3 frameMovement = Vector3.zero;

            targetPos = Player.instance.GetPlayerPos();
            distanceToTarget = targetPos - transform.position;

            float totalDistance = distanceToTarget.magnitude;
            if (totalDistance > 0.05f)
            {
                float moveX = distanceToTarget.x / totalDistance;
                float moveY = distanceToTarget.y / totalDistance;
                float frameSpeed = speed * Time.deltaTime;

                frameMovement = new(moveX * frameSpeed, moveY * frameSpeed, 0f);
                Vector3 Pos = transform.position + frameMovement;
                Pos.z = -10f / totalDistance;

                transform.position = Pos;

            }
        }

        if (sniperEnemyAI)
        {
            targetPos = Player.instance.GetPlayerPos();
            Vector3 distanceToTarget = targetPos - transform.position;
            float totalDistance = distanceToTarget.magnitude;

            if (totalDistance <= distanceToShot || aiming)
            {
                // aim
                if(!aiming)
                {
                    StartCoroutine(SniperAiming());
                    aiming = true;
                }

                
                animator.SetBool("isMoving", false);
            }
            else
            {
                // move
                shotingAngle = 0;
                float moveX = distanceToTarget.x / totalDistance;
                float moveY = distanceToTarget.y / totalDistance;
                float frameSpeed = speed * Time.deltaTime;

                Vector3 frameMovement = new(moveX * frameSpeed, moveY * frameSpeed, 0f);
                Vector3 Pos = transform.position + frameMovement;
                Pos.z = -10f / totalDistance;

                transform.position = Pos;
                animator.SetBool("isMoving", true);
            }

            // Head tilt & hand invert
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
            larm.invertY = looksRight;
            rarm.invertY = looksRight;


            Vector3 hgoCenter = new(transform.position.x, transform.position.y + srScript.handsGripOffset.y, transform.position.z);
            hgoCenter.z -= 0.03f;
            larm.endPos = Utils.PointOnCircle(hgoCenter, srScript.handsGripOffset.x, shotingAngle);
            rarm.endPos = Utils.PointOnCircle(hgoCenter, srScript.handsGripOffset.x, shotingAngle);
        }

        transform.rotation = Quaternion.Euler(0, looksRight ? 180 : 0, 0);

        if (shieldEnemyAI)
        {
            transform.position = holder.transform.position + weaponOffset;
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    public void TakeDamage(float damage)
    {
        health -= damage;
        if (health <= 0)
            Death();
    }
    public void Death()
    {
        if(!shieldEnemyAI && isAlive)
        {
            isAlive = false;
            GameManager.Instance.totalZombies--; 
            Player.instance.AddExp(expDrop);
            Player.instance.enemyDeathAudioSource.transform.position = transform.position;
            Player.instance.enemyDeathAudioSource.Play();
            Player.instance.score += expDrop;
        }
        Destroy(gameObject);
    }
    IEnumerator SniperAiming()
    {
        yield return new WaitForSeconds(2 + srScript.cooldownTillShot);
        srScript.Shot(srScript.GetShootingAngle());
        if (srScript.bulletsLeft <= 0)
            StartCoroutine(srScript.Reload());
        aiming = false;
        yield return null;
    }
}
