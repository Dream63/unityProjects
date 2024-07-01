using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class Card : MonoBehaviour
{


    GameObject cardObject;

    GameObject weaponPref;
    Firearm weaponScript;
    Bullet weaponBullet;
    public TextMeshProUGUI cardText;
    public Image cardBg, weaponImageComponent;
    public GameObject rarityGlow;

    bool isWeaponCard;
    public float multHp = 1, multDmg = 1, multAcc = 1, multSpeed = 1, multReload = 1, multMag = 1, multFirerate = 1, multExp = 1;
    public float baseHp = 0, baseDmg = 0, baseSpeed = 0, baseMag = 0, basePierce = 0, baseBulletsPerShot = 0;
    public void CreateCardInfo(GameObject cardPref,float baseDamage1, float multDamage1, float baseHp1, float multHp1, float baseSpeed1, float multSpeed1, float multAccuracy1,
             float baseMagazine1, float multMagazine1, float multFirerate1, float multReloadTime1, float basePierce1, float baseBulletsPerShot1, float multExp1)
    {
        cardObject = cardPref;
        weaponImageComponent.enabled = false;

        multHp = multHp1;
        baseHp = baseHp1;
        baseSpeed = baseSpeed1;
        multSpeed = multSpeed1;
        multDmg = multDamage1;
        baseDmg = baseDamage1;
        multAcc = multAccuracy1;
        multReload = multReloadTime1;
        multMag = multMagazine1;
        baseMag = baseMagazine1;
        multFirerate = multFirerate1;
        multExp = multExp1;
        basePierce = basePierce1;
        baseBulletsPerShot = baseBulletsPerShot1;
        CreateCardText();
    }

    public void CreateWeaponCardInfo(GameObject cardPref, GameObject weaponPref)
    {
        cardObject = cardPref;
        weaponImageComponent.sprite = weaponPref.GetComponentInChildren<SpriteRenderer>().sprite;
        this.weaponPref = weaponPref;
        weaponScript = this.weaponPref.GetComponent<Firearm>();
        weaponBullet = weaponScript.bulletPrefab.GetComponent<Bullet>();
        isWeaponCard = true;
        WeaponInfoText();
    }
    void CreateCardText()
    {
        cardText.text = "";
        if (multHp != 1)
            cardText.text += "\nHealth : x" + multHp.ToString("0.00");
        if (baseHp != 0)
            cardText.text += "\nBase health : " + baseHp.ToString("0.00");
        if (multDmg != 1)
            cardText.text += "\nDamage : x" + multDmg.ToString("0.00");
        if (baseDmg != 0)
            cardText.text += "\nBase damage : " + baseDmg.ToString("0.00");
        if (multSpeed != 1)
            cardText.text += "\nSpeed : x" + multSpeed.ToString("0.00");
        if (baseSpeed != 0)
            cardText.text += "\nBase speed : " + baseSpeed.ToString("0.00");
        if (multAcc != 1)
            cardText.text += "\nAccuracy : x" + multAcc.ToString("0.00");
        if (multReload != 1)
            cardText.text += "\nReload Time : x" + multReload.ToString("0.00");
        if (multMag != 1)
            cardText.text += "\nMagazine: x" + multMag.ToString("0.00");
        if (baseMag != 0)
            cardText.text += "\nBase magazine : " + baseMag.ToString("0");
        if (multFirerate != 1)
            cardText.text += "\nFirerate : x" + multFirerate.ToString("0.00");
        if (baseBulletsPerShot != 0)
            cardText.text += "\nBullets shot : +" + baseBulletsPerShot.ToString("0");
        if (basePierce != 0)
            cardText.text += "\nBullet piercing : +" + basePierce.ToString("0");
        if (multExp != 1)
            cardText.text += "\nExperience : x" + multExp.ToString("0.00");
    }
    void WeaponInfoText()
    {
        cardText.text = "\n\n\n\n \n\n\n\n";
        cardText.text += "Damage: " + weaponBullet.damage;
        cardText.text += "\nAccuracy: " + weaponScript.accuracy;
        cardText.text += "\nFirerate: " + weaponScript.firerate;
        cardText.text += "\nMagazine: " + weaponScript.magazineCapasity;
        cardText.text += "\nReloadTime: " + weaponScript.reloadTime;
        if(weaponScript.bulletsPerShot != 1)
            cardText.text += "\nBullets shot: " + weaponScript.bulletsPerShot;
        if(weaponBullet.pierse != 1)
            cardText.text += "\nPierce: " + weaponBullet.pierse;
    }
    public void SetGlow(Color color)
    {
        rarityGlow.GetComponent<Image>().color = color;
    }

    private void Update()
    {
        if(rarityGlow)
        rarityGlow.transform.rotation = Quaternion.Euler(rarityGlow.transform.rotation.eulerAngles + new Vector3(0, 0, 20 * Time.unscaledDeltaTime));
    }
    public void UseCard()
    {
        if (!isWeaponCard)
        {
            PlayerStats.Instance.SetStats(baseDmg, baseHp, baseSpeed, baseMag, basePierce, baseBulletsPerShot, multDmg, multHp, multSpeed, multAcc, multMag, multFirerate, multReload, multExp);
            Destroy(UI.instance.cardHand);
            UI.instance.Button();
            Time.timeScale = 1.0f;
            Firearm.instance.RecalcValues();
            UI.instance.UpdateAmmoBar();
            UI.instance.UpdateHpBar();
            UI.instance.displayStats.text = PlayerStats.Instance.GetStats();
            UI.instance.cardField.SetActive(false);
        }
        if(isWeaponCard)
        {
            Player.instance.SetWeapon(weaponPref);
            Destroy(UI.instance.cardHand);
            UI.instance.Button();
            Time.timeScale = 1.0f;
            UI.instance.cardField.SetActive(false);
        }
    }
}
