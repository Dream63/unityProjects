using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    public static UI instance;

    [Header("Info UI")]
    public GameObject Ui;
    public GameObject empty;
    public Image bloodScreen, waveObjectGlow;
    public GameObject waveObject, lvlupObject, lvlupImage;
    public TextMeshProUGUI timerText, expText, hpText, ammoText;
    public AudioSource lvlAudio, waveAudio, buttonAudio;
    [System.NonSerialized] public TextMeshProUGUI waveText, lvlupText;

    public GameObject expBar, hpBar, ammoBar;
    [System.NonSerialized] public RectTransform expBarRectTransform, hpBarRectTransform, ammoBarRectTransform;

    int currentlvlup = 1;
    int currentwave = 1;
    bool spinLvlUpBg = false;

    [Header("Stats menu text")]
    public GameObject statButton;
    public GameObject statMenu;
    public TextMeshProUGUI pointsLeft;
    public TextMeshProUGUI priceHp, priceDmg, priceSpeed, priceAccuracy, priceMagCapacity, priceRegen, priceCards;
    public TextMeshProUGUI lvlHp, lvlDmg, lvlSpeed, lvlAccuracy, lvlMagCapacity, lvlRegen, lvlCards;

    public GameObject lvlupDisplayField, statsDisplayField;
    public Image statsDisplayButton, lvlUpStatsButton;
    public TextMeshProUGUI displayStats;

    [Header("Cards")]
    public GameObject cardButton;
    public GameObject closeCardButton;
    public GameObject cardField;
    public GameObject cardPrefab;
    [System.NonSerialized] public GameObject cardHand;
    public Sprite common, rare, epic, legendary, mythic;
    public GameObject pistol, smg, ak, m249, mini, sPump, sAuto, sniper;

    [Header("Game over")]
    public GameObject gameOverScreen;
    public TextMeshProUGUI finalScore;
    public TextMeshProUGUI bestScore;
    public GameObject restartButton, menuButton;
    public AudioSource gameOverAudio;

    [Header("Completion screen")]

    public GameObject completionScreen;
    public TextMeshProUGUI score;
    public AudioClip newBGM;
    public TMP_FontAsset gameCompleteWaveFont;

    [Header("Mics")]

    public GameObject pauseMenu;
    float localTimescale =0;

    private void Awake()
    {
        instance = this;
        statMenu.SetActive(false);
    }
    void Start()
    {
        expBarRectTransform = expBar.GetComponent<RectTransform>();
        hpBarRectTransform = hpBar.GetComponent<RectTransform>();
        ammoBarRectTransform = ammoBar.GetComponent<RectTransform>();
        waveText = waveObject.GetComponentInChildren<TextMeshProUGUI>();
        lvlupText = lvlupObject.GetComponentInChildren<TextMeshProUGUI>();

        waveObject.SetActive(false);
        lvlupObject.SetActive(false);

        if (PlayerStats.Instance.CheapestShopPrice() <= Player.instance.statPointsLeft)
            instance.statButton.GetComponent<Image>().color = new(1f, 1f, 1f, 1f);
        if(Player.instance.lvlupCardLeft > 0)
            instance.cardButton.GetComponent<Image>().color = new(1f, 1f, 1f, 1f);

        if(true)
        {
            Mesh myMesh = new Mesh();

            Vector3[] vertices = new Vector3[4];
            Vector2[] uv = new Vector2[4];
            int[] triangles = new int[6];
            Vector3 camera00 = Camera.main.ScreenToWorldPoint(new(0, 0));
            Vector3 camera11 = Camera.main.ScreenToWorldPoint(new(Camera.main.pixelWidth, Camera.main.pixelHeight));
            float canvasScale = CameraManager.Instance.canvasScaler.gameObject.GetComponent<RectTransform>().localScale.x;

            vertices[0] = new Vector3(camera00.x / canvasScale, camera00.y / canvasScale);
            vertices[1] = new Vector3(camera00.x / canvasScale, camera11.y / canvasScale);
            vertices[2] = new Vector3(camera11.x / canvasScale, camera11.y / canvasScale);
            vertices[3] = new Vector3(camera11.x / canvasScale, camera00.y / canvasScale);
            
            uv[0] = new Vector2(0, 0);
            uv[1] = new Vector2(0, 1);
            uv[2] = new Vector2(1, 1);
            uv[3] = new Vector2(1, 0);
            
            triangles[0] = 0;
            triangles[1] = 1;
            triangles[2] = 2;
            triangles[3] = 0;
            triangles[4] = 2;
            triangles[5] = 3;

            myMesh.vertices = vertices;
            myMesh.uv = uv;
            myMesh.triangles = triangles;

            bloodScreen.GetComponent<MeshFilter>().mesh = myMesh;
            bloodScreen.GetComponent<MeshRenderer>().material.color = new(1f, 1f, 1f, 1f - Mathf.Clamp((Player.instance.health * 2) / (PlayerStats.Instance.baseHp * PlayerStats.Instance.multHp), 0f, 2f));        
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(LvlUpAnouncement());
        }

        if (spinLvlUpBg)
        {
            Vector3 rotation = lvlupImage.transform.rotation.eulerAngles;
            rotation.z += 45f * Time.deltaTime;
            lvlupImage.transform.rotation = Quaternion.Euler(rotation);
        }
    }

    public IEnumerator LvlUpAnouncement()
    {
        currentlvlup++;
        lvlupObject.SetActive(true);
        spinLvlUpBg = true;
        lvlAudio.Play();
        yield return new WaitForSeconds(3f);
        lvlupImage.transform.rotation = Quaternion.Euler(0, 0, 0);
        if (currentlvlup == 2)
        {
            lvlupObject.SetActive(false);
            spinLvlUpBg = false;
        }
        currentlvlup--;
        yield return null;
    }
    public IEnumerator WaveAnouncement(int wave)
    {
        currentwave++;
        waveObject.SetActive(true);
        waveAudio.Play();
        waveText.text = "Wave " + wave.ToString();
        yield return new WaitForSeconds(3f);
        if (currentwave == 2)
            waveObject.SetActive(false);
        currentwave--;
        yield return null;
    }
    public void TimerUpdate(int time, Color color)
    {
        timerText.color = color;
        timerText.text = time.ToString();
    }
    public void TimerUpdate(int time)
    {
        timerText.text = time.ToString();
    }
    public void BarTransform(RectTransform bar, float value, float valueWhenFull, int barWidth)
    {
        float percent = value / valueWhenFull;
        percent = Mathf.Clamp(percent, 0f, 1f);
        bar.sizeDelta = new Vector2(percent * barWidth, bar.sizeDelta.y);

    }
    public void UpdateExpBar()
    {
        expText.text = Player.instance.exp.ToString("0") + " / " + Player.instance.expReqToLvlup.ToString() + "\n\nLvl " + Player.instance.level;
        BarTransform(expBarRectTransform, Player.instance.exp, Player.instance.expReqToLvlup, 1200);
    }
    public void UpdateHpBar()
    {
        hpText.text = Player.instance.health.ToString("0");
        BarTransform(hpBarRectTransform, Player.instance.health, PlayerStats.Instance.baseHp * PlayerStats.Instance.multHp, 274);
        bloodScreen.GetComponent<MeshRenderer>().material.color = new(1f, 1f, 1f, 1f - Mathf.Clamp((Player.instance.health * 2) / (PlayerStats.Instance.baseHp * PlayerStats.Instance.multHp), 0f, 2f));
    }
    public void UpdateAmmoBar()
    {
        ammoText.text = Firearm.instance.bulletsLeft.ToString("0");
        BarTransform(ammoBarRectTransform, Firearm.instance.bulletsLeft, Firearm.instance.magazineCapasity, 108);
    }
    public void Button()
    {
        buttonAudio.Play();
    }
    public void CursorOverButton()
    {
        if(!Input.GetMouseButton(0))
        Firearm.instance.pointerOverButton = true;
    }
    public void CursorDownButton()
    {
        Firearm.instance.pointerOverButton = false;
    }
    public void StatMenuOpen()
    {
        statMenu.SetActive(true);
        pointsLeft.text = "points left: " + Player.instance.statPointsLeft.ToString();
        Time.timeScale = 0f;
        PlayerStats.Instance.UpdateStatValues();
    }
    public void StatMenuClose()
    {
        statMenu.SetActive(false);
        Time.timeScale = 1f;
    }
    public Card CreateCard(Vector3 pos, float scale)
    {
        GameObject cardObject = Instantiate(instance.cardPrefab, pos, Quaternion.identity, cardHand.transform);
        cardObject.transform.localScale = new Vector3(scale,scale,1);
        Card card = cardObject.GetComponent<Card>();
        int numOfTraits = 1;
        float value = 1;
        float rarity = Random.Range(0f, 1f);
        bool isWeaponCard = rarity < 0.50f ? Random.Range(0f, 1f) > 0.95f : Random.Range(0f, 1f) > 0.75f && rarity <= 0.98f;


        if (!isWeaponCard)
        {
            if (rarity < 0.5f)
            {
                numOfTraits = Random.Range(1, 4);
                value = Random.Range(1.05f, 1.2f);
            }
            if (rarity < 0.8f && rarity >= 0.5f)
            {
                numOfTraits = Random.Range(2, 5);
                value = Random.Range(1.1f, 1.4f);
            }
            if (rarity < 0.95f && rarity >= 0.8f)
            {
                numOfTraits = Random.Range(3, 6);
                value = Random.Range(1.25f, 1.6f);
            }
            if (rarity < 0.98f && rarity >= 0.95f)
            {
                numOfTraits = Random.Range(4, 7);
                value = Random.Range(1.4f, 1.8f);
            }
            if (rarity >= 0.98f)
            {
                numOfTraits = Random.Range(2, 4);
                value = Random.Range(1.8f, 2.2f);
            }

            bool canMultHp = value < 1.5f;
            bool canMultDmg = value < 1.5f;
            bool canMultAcc = value < 1.5;
            bool canMultSpeed = value < 1.8f && value > 1.1f;
            bool canMultReload = value < 1.8f && value > 1.1f;
            bool canMultMag = value < 1.8f && value > 1.1f;
            bool canMultFirerate = value < 2f && value > 1.4f;
            bool canMultExp = value > 1.4f;

            bool canAddPierce = value > 1.8f;
            bool canAddBullets = value > 2f;

            float multHp = 1, multDmg = 1, multAcc = 1, multSpeed = 1, multReload = 1, multMag = 1, multFirerate = 1, multExp = 1;
            float baseHp = 0, baseDmg = 0, baseSpeed = 0, baseMag = 0, basePierce = 0, baseBulletsPerShot = 0;

            for (int i = 0, iteration = 0; i < numOfTraits || iteration > 100; iteration++)
            {
                int trait = Random.Range(1, 15);
                if (trait == 1 && canMultHp && multHp == 1f)
                { multHp *= 1 + Random.Range(value - 1.1f, value - 0.9f); i++; }
                if (trait == 2 && canMultHp && baseHp == 0)
                { baseHp += Random.Range(value - 1f, value - 0.8f) * 50; i++; }

                if (trait == 3 && canMultDmg && multDmg == 1f)
                { multDmg *= 1 + Random.Range(value - 1.1f, value - 0.9f) / 2; i++; }
                if (trait == 4 && canMultDmg && baseDmg == 0)
                { baseDmg += Random.Range(value - 1.1f, value - 0.9f) * 15; i++; }

                if (trait == 5 && canMultAcc && multAcc == 1f)
                { multAcc *= 1 + Random.Range(value - 1.1f, value - 0.9f) / 2; i++; }

                if (trait == 6 && canMultSpeed && multSpeed == 1f)
                { multSpeed *= 1 + Random.Range(value - 1.1f, value - 0.9f) / 5; i++; }
                if (trait == 7 && canMultSpeed && baseSpeed == 0)
                { baseSpeed += Random.Range(value - 1.1f, value - 0.9f); i++; }

                if (trait == 8 && canMultReload && multReload == 1f)
                { multReload *= 1 + Random.Range(value - 1.1f, value - 0.9f) / 5; i++; }

                if (trait == 9 && canMultMag && multMag == 1f)
                { multMag *= 1 + Random.Range(value - 1.1f, value - 0.9f) / 2; i++; }
                if (trait == 10 && canMultMag && baseMag == 0)
                { baseMag += Random.Range(value - 1.1f, value - 0.9f) * 10; i++; }

                if (trait == 11 && canMultFirerate && multFirerate == 1f)
                { multFirerate *= 1 + Random.Range(value - 1.1f, value - 0.9f) / 6; i++; }

                if (trait == 12 && canMultExp && multExp == 1f)
                { multExp *= 1 + Random.Range(value - 1.1f, value - 0.9f) / 2; i++; }

                if (trait == 13 && canAddPierce && baseBulletsPerShot == 0)
                { baseBulletsPerShot += 1; i++; }

                if (trait == 14 && canAddBullets && basePierce == 0)
                { basePierce += 1; i++; }
            }
            card.CreateCardInfo(cardObject, baseDmg, multDmg, baseHp, multHp, baseSpeed, multSpeed, multAcc, baseMag, multMag, multFirerate, multReload, basePierce, baseBulletsPerShot, multExp);
        }

        if (isWeaponCard)
        {
            GameObject cardWeapon = pistol;
            if (rarity < 0.5f)
            {
                cardWeapon = pistol;
            }
            if (rarity < 0.8f && rarity >= 0.5f)
            {
                int randWeapon = Random.Range(0, 2);
                if (randWeapon == 0)
                {
                    cardWeapon = smg;
                }
                if (randWeapon == 1)
                {
                    cardWeapon = ak;
                }
            }
            if (rarity < 0.95f && rarity >= 0.8f)
            {
                int randWeapon = Random.Range(0, 2);
                if (randWeapon == 0)
                {
                    cardWeapon = m249;
                }
                if (randWeapon == 1)
                {
                    cardWeapon = sPump;
                }
            }
            if (rarity < 0.98f && rarity >= 0.95f)
            {
                int randWeapon = Random.Range(0, 3);
                if (randWeapon == 0)
                {
                    cardWeapon = sAuto;
                }
                if (randWeapon == 1)
                {
                    cardWeapon = sniper;
                }
                if (randWeapon == 2)
                {
                    cardWeapon = mini;
                }
            }
            card.CreateWeaponCardInfo(cardObject, cardWeapon);
        }

        if (rarity < 0.5f)
        {
            card.cardBg.sprite = common;
            card.SetGlow(new(0f, 0f, 0f, 0f));
        }
        if (rarity < 0.8f && rarity >= 0.5f)
        {
            card.cardBg.sprite = rare;
            card.SetGlow(new(0.9f, 1f, 1f, 0.2f));
        }
        if (rarity < 0.95f && rarity >= 0.8f)
        {
            card.cardBg.sprite = epic;
            card.SetGlow(new(1f, 1f, 0.9f, 0.5f));
        }
        if (rarity < 0.98f && rarity >= 0.95f)
        {
            card.cardBg.sprite = legendary;
            card.SetGlow(new(1f, 0.8f, 0.2f, 1f));
        }
        if (rarity >= 0.98f)
        {
            card.cardBg.sprite = mythic;
            card.cardText.color = Color.white;
            card.SetGlow(new(1f, 0.0f, 0.7f, 1f));
        }

        return card;
    }
    public void GetCardHand()
    {
        if(Player.instance.lvlupCardLeft > 0 && statMenu.activeSelf == false)
        {
            Player.instance.lvlupCardLeft--;
            if(Player.instance.lvlupCardLeft == 0)
                cardButton.GetComponent<Image>().color = new(1f,1f,1f,0.5f);

            if (cardHand)
                Destroy(cardHand);
            cardHand = Instantiate(empty, cardField.transform);
            Vector3 center = Player.instance.GetPlayerPos();
            float cameraSize = Camera.main.orthographicSize;
            Time.timeScale = 0f;
            instance.cardField.SetActive(true);
            if (PlayerStats.Instance.baseCards == 3)
            {
                CreateCard(center + new Vector3(cameraSize, 0, 0), 1.1f);
                CreateCard(center, 1.1f);
                CreateCard(center + new Vector3(-cameraSize, 0, 0), 1.1f);
            }
            if (PlayerStats.Instance.baseCards == 4)
            {
                CreateCard(center + new Vector3(cameraSize / 2, cameraSize / 2, 0), 0.9f);
                CreateCard(center + new Vector3(cameraSize / 2, -cameraSize / 2, 0), 0.9f);
                CreateCard(center + new Vector3(-cameraSize / 2, cameraSize / 2, 0), 0.9f);
                CreateCard(center + new Vector3(-cameraSize / 2, -cameraSize / 2, 0), 0.9f);
            }
            if (PlayerStats.Instance.baseCards == 5)
            {
                CreateCard(center + new Vector3(cameraSize / 2, cameraSize / 2, 0), 0.9f);
                CreateCard(center + new Vector3(-cameraSize / 2, cameraSize / 2, 0), 0.9f);

                CreateCard(center + new Vector3(cameraSize, -cameraSize / 2, 0), 0.9f);
                CreateCard(center + new Vector3(0, -cameraSize / 2), 0.9f);
                CreateCard(center + new Vector3(-cameraSize, -cameraSize / 2, 0), 0.9f);
            }
            if (PlayerStats.Instance.baseCards == 6)
            {
                CreateCard(center + new Vector3(cameraSize, cameraSize / 2, 0), 0.9f);
                CreateCard(center + new Vector3(0, cameraSize / 2), 0.9f);
                CreateCard(center + new Vector3(-cameraSize, cameraSize / 2, 0), 0.9f);

                CreateCard(center + new Vector3(cameraSize, -cameraSize / 2, 0), 0.9f);
                CreateCard(center + new Vector3(0, -cameraSize / 2), 0.9f);
                CreateCard(center + new Vector3(-cameraSize, -cameraSize / 2, 0), 0.9f);
            }
            Instantiate(closeCardButton, center + new Vector3(cameraSize * 1.55f, cameraSize / 1.25f, 0), Quaternion.Euler(0,0,0), cardHand.transform);
        }

    }
    public void OpenPause()
    {
        localTimescale = Time.timeScale;
        Time.timeScale = 0f;
        pauseMenu.SetActive(true);
        UI.instance.Button();
    }
    public void ClosePause()
    {
        Time.timeScale = localTimescale;
        pauseMenu.SetActive(false);
        UI.instance.Button();
    }
    public void DisplayStatsMenu()
    {
        lvlupDisplayField.SetActive(false);
        statsDisplayField.SetActive(true);
        lvlUpStatsButton.color = new(1f, 1f, 1f);
        statsDisplayButton.color = new(0.9f, 0.9f, 0.9f);
        displayStats.text = PlayerStats.Instance.GetStats();
    }
    public void DisplayLvlUpMenu()
    {
        lvlupDisplayField.SetActive(true);
        statsDisplayField.SetActive(false);
        lvlUpStatsButton.color = new(0.9f, 0.9f, 0.9f);
        statsDisplayButton.color = new(1f, 1f, 1f);
        pointsLeft.text = "points left: " + Player.instance.statPointsLeft.ToString();
        PlayerStats.Instance.UpdateStatValues();
    }
}
