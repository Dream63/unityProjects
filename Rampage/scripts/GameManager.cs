using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;
using Util;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header ("Spawn settings")]
    public float spawnCircleRadius = 50;
    public float sZombieChance = 0.1f;
    public GameObject normal, speedy, heavy, sniper, sniperBulletPref,
                      normal_s, speedy_s, heavy_s, sniper_s, sniper_sBulletPref;


    [NonSerialized] public int totalZombies = 0;
    [NonSerialized] public Enemy normalStats, speedyStats, heavyStats, sniperStats;
    [NonSerialized] public Enemy normal_s_Stats, speedy_s_Stats, heavy_s_Stats, shieldStats, sniper_s_Stats;
    Bullet sniperBullet, sniperSBullet;

    bool spawnSpeedy = false, 
         spawnHeavy = false, 
         naturalNormalsSpawn = true,
         spawnSnipers= false;
    bool sNormalSpawn = false,
         sSpeedySpawn = false,
         sHeavySpawn = false,
         sSniperSpawn = false;

    float enemySpawnCooldown = 0.1f;
    int wave = 1;
    int waveRecourses = 3;
    bool waveInProgress;
    bool completedGame = false;
    float timePerWave = 60;
    float timer = 10;
    bool stopTimer;
    [Header ("Misc")]
    public bool printFps = false;
    Vector3 fpsTimer = Vector3.zero;

    [Header("Transition")]
    public Image blackScreen;
    bool screenGoBlack, screenGoNormal;
    float blackScreenOpasity = 0;
    public float musicVolume;
    public AudioSource BGM;
    [NonSerialized] public bool changeBGM, stopWTCongratsMusic; // stop with transition
    [NonSerialized] public bool startWTCongratsMusic, startBGM;

    private void Awake()
    {
        Instance = this;
    }
    void Start()
    {
        normalStats = normal.GetComponent<Enemy>();
        speedyStats = speedy.GetComponent<Enemy>();
        heavyStats = heavy.GetComponent<Enemy>();
        sniperStats = sniper.GetComponent<Enemy>();
        normal_s_Stats = normal_s.GetComponent<Enemy>();
        speedy_s_Stats = speedy_s.GetComponent<Enemy>();
        heavy_s_Stats = heavy_s.GetComponent<Enemy>();
        shieldStats = heavy_s_Stats.shield.GetComponent<Enemy>();
        sniper_s_Stats = sniper_s.GetComponent<Enemy>();
        sniperBullet = sniperBulletPref.GetComponent<Bullet>();
        sniperSBullet = sniper_sBulletPref.GetComponent<Bullet>();
        StartCoroutine(TransitionBetweenScenesEnd());
    }

    void Update()
    {
        if (!stopTimer)
        timer -= Time.deltaTime;
        UI.instance.TimerUpdate(Mathf.FloorToInt(timer));

        if(totalZombies <= 0 && waveInProgress)
        {
            timer = 5;
            UI.instance.TimerUpdate(Mathf.FloorToInt(timer), new Color(1f, 1f, 0.4f));
            waveInProgress = false;

        }
        if (timer <= 0)
            if (wave == 101 && !completedGame)
            {
                timer = 10;
                UI.instance.TimerUpdate(Mathf.FloorToInt(timer), new Color(1f, 1f, 0.4f));
                Victory();
                completedGame = true;
            }
            else
                StartNewWave();
        if(printFps)
        fpsTimer = Utils.FPS(fpsTimer); // print fps

        if (screenGoBlack)
        {
            blackScreen.gameObject.SetActive(true);
            blackScreenOpasity += 1f * Time.unscaledDeltaTime;
            BGM.volume -= 1.5f * Time.unscaledDeltaTime;
            blackScreen.color = new(0f, 0f, 0f, blackScreenOpasity);
        }
        if (screenGoNormal)
        {
            blackScreen.gameObject.SetActive(true);
            blackScreenOpasity -= 1f * Time.unscaledDeltaTime;
            BGM.volume += 1.5f * Time.unscaledDeltaTime;
            BGM.volume = Mathf.Clamp(BGM.volume, 0, musicVolume);
            blackScreen.color = new(0f, 0f, 0f, blackScreenOpasity);
        }
        if (changeBGM)
        {
            BGM.volume -= 0.5f * Time.unscaledDeltaTime;
            if (BGM.volume == 0f)
            {
                changeBGM = false;
                BGM.Stop();
                BGM.clip = UI.instance.newBGM;
                startBGM = true;
            }
        }
        if (startBGM)
        {
            if(!BGM.isPlaying)
                BGM.Play();
            BGM.volume += 0.5f * Time.unscaledDeltaTime;
            if (BGM.volume >= musicVolume)
                BGM.volume = musicVolume;
                startBGM = false;
        }
    }


    public void StartNewWave()
    {
        Player.instance.TakeDamage(-PlayerStats.Instance.baseHp * PlayerStats.Instance.multHp * PlayerStats.Instance.regen);        
        waveInProgress = true;

        // specific-wave events
        if (wave % 3 == 0)
        {
            Player.instance.statPointsLeft++;
        }
        if (wave % 5 == 0)
        {
            if(timePerWave < 180)
                timePerWave += 60;
            BuffEnemies(20, true);
        }
        if (wave > 100)
        {
            BuffEnemies(100, false);
            if (wave > 110)
                BuffEnemies(25, false);

        }

        // enemy type spawn rules
        if (true)
        {
            //speedy
            if (wave >= 5 && (wave % 3 != 0 || UnityEngine.Random.Range(0, 5) < 2))
                spawnSpeedy = true;
            else spawnSpeedy = false;

            //heavy
            if (wave >= 10 && (wave % 2 != 0 || UnityEngine.Random.Range(0, 6) < 2))
                spawnHeavy = true;
            else spawnHeavy = false;

            //snipers
            if (wave >= 15 && (wave % 5 <= 1 || UnityEngine.Random.Range(0, 6) < 1))
                spawnSnipers = true;
            else spawnSnipers = false;

            //normals
            if ((spawnSpeedy || spawnSpeedy || spawnHeavy || spawnSnipers) && UnityEngine.Random.Range(0, 3) == 1)
                naturalNormalsSpawn = false;
            else naturalNormalsSpawn = true;

            // S-zombies spawn
            if (wave >= 20 && UnityEngine.Random.Range(0, 5) != 1)
                sNormalSpawn = true;
            else sNormalSpawn = false;

            if (wave >= 25 && UnityEngine.Random.Range(0, 4) != 1)
                sSpeedySpawn = true;
            else sSpeedySpawn = false;

            if (wave >= 30 && UnityEngine.Random.Range(0, 4) != 1)
                sHeavySpawn = true;
            else sHeavySpawn = false;

            if (wave >= 35 && UnityEngine.Random.Range(0, 3) != 1)
                sSniperSpawn = true;
            else sSniperSpawn = false;
        }

        int normals = 0, speeds = 0, heavys = 0, snipers = 0;
        int s_normals = 0, s_speeds = 0, s_heavys = 0, s_snipers = 0;
        int iteration = 0;

        // enemy forces creation
        while (waveRecourses > 0 && iteration < 1000)
        {
            if (spawnSpeedy && waveRecourses >= 2 && UnityEngine.Random.Range(0, 3) != 0)
            {
                if(sSpeedySpawn && UnityEngine.Random.Range(0f, 1f) <= sZombieChance && waveRecourses >= 6) {
                    s_speeds++;
                    waveRecourses -= 6;
                }
                else
                {
                    speeds++;
                    waveRecourses -= 2;
                }
                totalZombies++;
            }
            if (spawnHeavy && waveRecourses >= 4 && UnityEngine.Random.Range(0, 3) != 0 )
            {
                if (sHeavySpawn && UnityEngine.Random.Range(0f, 1f) <= sZombieChance && waveRecourses >= 12)
                {
                    s_heavys++;
                    waveRecourses -= 12;
                }
                else
                {
                    heavys++;
                    waveRecourses -= 4;
                }
                totalZombies++;
            }
            if (spawnSnipers && waveRecourses >= 10 && UnityEngine.Random.Range(0, 5) == 0)
            {
                if (sSniperSpawn && UnityEngine.Random.Range(0f, 1f) <= sZombieChance && waveRecourses >= 25)
                {
                    s_snipers++;
                    Debug.Log(sSniperSpawn);
                    waveRecourses -= 25;
                }
                else
                {
                    snipers++;
                    waveRecourses -= 10;
                }
                totalZombies++;
            }
            if ((naturalNormalsSpawn || waveRecourses < 2) && waveRecourses > 0)
            {
                if (sNormalSpawn && UnityEngine.Random.Range(0f, 1f) <= sZombieChance && waveRecourses >= 3)
                {
                    s_normals++;
                    waveRecourses -= 3;
                }
                else
                {
                    normals++; 
                    waveRecourses -= 1;
                }
                totalZombies++;
            }
            iteration++;
        }

        // New wave
        StartCoroutine(UI.instance.WaveAnouncement(wave));
        UI.instance.TimerUpdate((int)timePerWave, Color.white);
        timer = timePerWave;

        if (totalZombies > 100)
            enemySpawnCooldown = 0.03f;

        StartCoroutine(SpawnEnemies(normals, speeds, heavys, snipers, s_normals, s_speeds, s_heavys, s_snipers, enemySpawnCooldown));

        // next wave recources calc
        wave++;
        waveRecourses = Mathf.FloorToInt(Mathf.Pow(wave, 1.7f) + 2);
    }
    IEnumerator SpawnEnemies(int normals, int speeds, int heavys, int snipers, int s_normals, int s_speeds, int s_heavys, int s_snipers, float enemySpawnCooldown)
    {
        float randQ = UnityEngine.Random.Range(-180, 180); // random quater free of enemies

        for (int i = 0; i < normals; i++)
        {
            Instantiate(normal, Utils.PointOnCircle(Player.instance.GetPlayerPos(), spawnCircleRadius, UnityEngine.Random.Range(0 + randQ, 270 + randQ)), Quaternion.Euler(0, 0, 0));
            yield return new WaitForSeconds(enemySpawnCooldown);
        }
        for (int i = 0; i < speeds; i++)
        {
            Instantiate(speedy, Utils.PointOnCircle(Player.instance.GetPlayerPos(), spawnCircleRadius, UnityEngine.Random.Range(0 + randQ, 270 + randQ)), Quaternion.Euler(0, 0, 0));
            yield return new WaitForSeconds(enemySpawnCooldown);
        }
        for (int i = 0; i < heavys; i++)
        {
            Instantiate(heavy, Utils.PointOnCircle(Player.instance.GetPlayerPos(), spawnCircleRadius, UnityEngine.Random.Range(0 + randQ, 270 + randQ)), Quaternion.Euler(0, 0, 0));
            yield return new WaitForSeconds(enemySpawnCooldown);
        }
        for (int i = 0; i < snipers; i++)
        {
            Instantiate(sniper, Utils.PointOnCircle(Player.instance.GetPlayerPos(), spawnCircleRadius * 1.1f, UnityEngine.Random.Range(0 + randQ, 270 + randQ)), Quaternion.Euler(0, 0, 0));
            yield return new WaitForSeconds(enemySpawnCooldown);
        }
        for (int i = 0; i < s_normals; i++)
        {
            Instantiate(normal_s, Utils.PointOnCircle(Player.instance.GetPlayerPos(), spawnCircleRadius, UnityEngine.Random.Range(0 + randQ, 270 + randQ)), Quaternion.Euler(0, 0, 0));
            yield return new WaitForSeconds(enemySpawnCooldown);
        }
        for (int i = 0; i < s_speeds; i++)
        {
            Instantiate(speedy_s, Utils.PointOnCircle(Player.instance.GetPlayerPos(), spawnCircleRadius, UnityEngine.Random.Range(0 + randQ, 270 + randQ)), Quaternion.Euler(0, 0, 0));
            yield return new WaitForSeconds(enemySpawnCooldown);
        }
        for (int i = 0; i < s_heavys; i++)
        {
            Instantiate(heavy_s, Utils.PointOnCircle(Player.instance.GetPlayerPos(), spawnCircleRadius, UnityEngine.Random.Range(0 + randQ, 270 + randQ)), Quaternion.Euler(0, 0, 0));
            yield return new WaitForSeconds(enemySpawnCooldown);
        }
        for (int i = 0; i < s_snipers; i++)
        {
            Instantiate(sniper_s, Utils.PointOnCircle(Player.instance.GetPlayerPos(), spawnCircleRadius * 1.1f, UnityEngine.Random.Range(0 + randQ, 270 + randQ)), Quaternion.Euler(0, 0, 0));
            Debug.Log(1);
            yield return new WaitForSeconds(enemySpawnCooldown);
        }

        yield return null;
    }
    void BuffEnemies(float percent, bool buffExp)
    {
        sZombieChance += (1 - sZombieChance) * (percent / 200);

        normalStats.health *= 1 + 0.025f * percent;
        speedyStats.health *= 1 + 0.025f * percent;
        heavyStats .health *= 1 + 0.025f * percent;
        sniperStats.health *= 1 + 0.025f * percent;
        normal_s_Stats.health *= 1 + 0.025f * percent;
        speedy_s_Stats.health *= 1 + 0.025f * percent;
        heavy_s_Stats .health *= 1 + 0.025f * percent;
        shieldStats.health *= 1 + 0.025f * percent;
        sniper_s_Stats.health *= 1 + 0.025f * percent;

        if (buffExp) {
            normalStats.expDrop *= 1 + 0.03f * percent;
            speedyStats.expDrop *= 1 + 0.03f * percent;
            heavyStats.expDrop *= 1 + 0.03f * percent;
            sniperStats.expDrop *= 1 + 0.03f * percent;
            normal_s_Stats.expDrop *= 1 + 0.03f * percent;
            speedy_s_Stats.expDrop *= 1 + 0.03f * percent;
            heavy_s_Stats.expDrop *= 1 + 0.03f * percent;
            sniper_s_Stats.expDrop *= 1 + 0.03f * percent; }

        normalStats.damage *= 1 + 0.02f * percent;
        speedyStats.damage *= 1 + 0.02f * percent;
        heavyStats .damage *= 1 + 0.02f * percent;
        sniperBullet.damage *= 1 + 0.02f * percent;
        normal_s_Stats.damage *= 1 + 0.02f * percent;
        speedy_s_Stats.damage *= 1 + 0.02f * percent;
        heavy_s_Stats .damage *= 1 + 0.02f * percent;
        sniperSBullet.damage *= 1 + 0.02f * percent;

        normalStats.speed *= 1 + 0.005f * percent;
        speedyStats.speed *= 1 + 0.005f * percent;
        heavyStats .speed *= 1 + 0.005f * percent;
        sniperStats.speed *= 1 + 0.005f * percent;
        normal_s_Stats.speed *= 1 + 0.005f * percent;
        speedy_s_Stats.speed *= 1 + 0.005f * percent;
        heavy_s_Stats .speed *= 1 + 0.005f * percent;
        sniper_s_Stats.speed *= 1 + 0.005f * percent;

        normalStats.speed = Mathf.Clamp(normalStats.speed, 1, 33);
        speedyStats.speed = Mathf.Clamp(speedyStats.speed, 1, 36);
        heavyStats.speed = Mathf.Clamp(heavyStats.speed, 1, 28);
        sniperStats.speed = Mathf.Clamp(sniperStats.speed, 1, 50);
        normal_s_Stats.speed = Mathf.Clamp(normalStats.speed, 1, 35);
        speedy_s_Stats.speed = Mathf.Clamp(speedyStats.speed, 1, 39);
        heavy_s_Stats.speed = Mathf.Clamp(heavyStats.speed, 1, 32);
        sniper_s_Stats.speed = Mathf.Clamp(sniperStats.speed, 1, 50);

        sniperBullet.bulletSpeed *= 1 + 0.005f * percent;
        sniperSBullet.bulletSpeed *= 1 + 0.005f * percent;
        sniperStats.distanceToShot *= 1 + 0.001f * percent;
        sniper_s_Stats.distanceToShot *= 1 + 0.001f * percent;
        sniperBullet.bulletSpeed = Mathf.Clamp(sniperBullet.bulletSpeed, 10, 100);
        sniperSBullet.bulletSpeed = Mathf.Clamp(sniperSBullet.bulletSpeed, 10, 130);
        sniperStats.distanceToShot = Mathf.Clamp(sniperStats.distanceToShot, 10, 120);
        sniper_s_Stats.distanceToShot = Mathf.Clamp(sniper_s_Stats.distanceToShot, 10, 150);


    }
    public void GameOver()
    {
        stopTimer = true;
        Time.timeScale = 0f;
        Player.instance.GetComponent<BoxCollider2D>().enabled = false;
        Player.instance.GetComponent<Rigidbody2D>().simulated = false;
        Player.instance.isAlive = false;
        UI.instance.gameOverScreen.SetActive(true);
        UI.instance.gameOverAudio.Play();

        float bestScore = PlayerPrefs.HasKey("BestScore") ? PlayerPrefs.GetFloat("BestScore") : 0;

        if (bestScore < Player.instance.score)
        {
            bestScore = Player.instance.score;
            PlayerPrefs.SetFloat(key: "BestScore", bestScore);
            UI.instance.bestScore.text = "New best: " + bestScore;
            UI.instance.restartButton.GetComponent<Image>().sprite = UI.instance.legendary;
            UI.instance.menuButton.GetComponent<Image>().sprite = UI.instance.legendary;
        }
        else
        {
            UI.instance.bestScore.text = "Best: " + bestScore;
            UI.instance.finalScore.text = "Score: " + Player.instance.score;
        }
    }
    public void LoadScene(int index)
    {
        StartCoroutine(TransitionBetweenScenes(index));
    }
    public IEnumerator TransitionBetweenScenes(int index)
    {
        screenGoBlack = true;
        blackScreenOpasity = 0f;
        yield return new WaitForSecondsRealtime(1);
        screenGoBlack = false;
        Time.timeScale = 1f;
        SceneManager.LoadScene(index);
        yield return null;
    }
    public IEnumerator TransitionBetweenScenesEnd()
    {

        BGM.volume = 0;
        blackScreen.color = new(0f, 0f, 0f, 0f);
        blackScreenOpasity = 1f;
        screenGoNormal = true;
        yield return new WaitForSecondsRealtime(1);
        blackScreen.gameObject.SetActive(false);
        screenGoNormal = false;
        yield return null;
    }
    public void Quit()
    {
        Application.Quit();
    }
    public void Victory()
    {
        UI.instance.completionScreen.SetActive(true);
        Time.timeScale = 0f;
        changeBGM = true;

        UI.instance.waveObjectGlow.color = new(0.711f, 1f, 1f, UI.instance.waveObjectGlow.color.a);
        UI.instance.waveText.font = UI.instance.gameCompleteWaveFont;

        float bestScore = PlayerPrefs.HasKey("BestScore") ? PlayerPrefs.GetFloat("BestScore") : 0;
        
        if (bestScore < Player.instance.score)
        {
            bestScore = Player.instance.score;
            PlayerPrefs.SetFloat(key: "BestScore", bestScore);
            UI.instance.score.text = "New best: " + bestScore;
        }
        else
            UI.instance.score.text = "score: " + Player.instance.score;
    }
    public void ContinuePlaying()
    {
        UI.instance.completionScreen.SetActive(false);
        Time.timeScale = 1f;
    }
}
