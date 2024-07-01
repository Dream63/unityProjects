using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Util;

public class Bullet : MonoBehaviour
{
    public float damage = 5;
    public float bulletSpeed = 100; // units per second
    public float bulletDisapearCountdown = 2; // time till bullet disapear
    public float pierse = 1;
    public bool friendly = true;


    void Start()
    {
        bulletDisapearCountdown = Mathf.Clamp(bulletDisapearCountdown, 0, 60);
        if (friendly)
        damage = Mathf.Clamp((damage + PlayerStats.Instance.baseDmg) * PlayerStats.Instance.multDmg, 0, 999999);
        pierse += PlayerStats.Instance.basePierce;
    }

    void Update()
    {
        transform.position = Utils.PointOnCircle( transform.position, 
                                                  bulletSpeed * Time.deltaTime, 
                                                  transform.rotation.eulerAngles.z );

        bulletDisapearCountdown -= Time.deltaTime;
        if (bulletDisapearCountdown <= 0)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D enemyCollider)
    {
        if(friendly)
        if (enemyCollider.gameObject.TryGetComponent<Enemy>(out Enemy enemy))
        {
            pierse -= enemy.sturdiness;
            enemy.TakeDamage(damage);
            if (pierse <= 0)
                Destroy(gameObject);
        }
        if (!friendly)
            if (enemyCollider.gameObject.TryGetComponent<Player>(out Player player))
            {
                player.TakeDamage(damage);
                Destroy(gameObject);
            }
    }
}
