using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    public static PlayerStats Instance;



    [Header("Stats")]
    public float baseHp = 100;
    public float baseDmg = 0, baseSpeed = 16, baseMagazine = 0, baseBulletsPerShot = 0, basePierce = 0, baseCards = 3;
    public float multDmg = 1, multSpeed = 1, multHp = 1, multMagazine = 1, multFirerate = 1, multAccuracy = 1, multReloadTime = 1, multExp = 1, regen = 0.2f;

    public int statLvlHp = 0, statLvlDmg = 0, statLvlSpeed = 0,  statLvlAccuracy = 0, statLvlMagazine = 0, statLvlRegen = 0, statLvlCards = 0;
    public float hpMultPerLvl = 1.25f, dmgMultPerLvl = 1.15f, speedMultPerLvl = 1.05f, accMultPerLvl = 1.15f, magMultPerLvl = 1.20f, regAddPerLvl = 1.10f, cardAddPerLvl = 1;



    private void Awake()
    {
        Instance = this;
    }

    public void UpdateStatValues()
    {
        UpdateStat(UI.instance.priceHp, UI.instance.lvlHp, statLvlHp, 10); // hp
        UpdateStat(UI.instance.priceDmg, UI.instance.lvlDmg, statLvlDmg, 10); // dmg
        UpdateStat(UI.instance.priceSpeed, UI.instance.lvlSpeed, statLvlSpeed, 10); // speed
        UpdateStat(UI.instance.priceAccuracy, UI.instance.lvlAccuracy, statLvlAccuracy, 10); // acc
        UpdateStat(UI.instance.priceMagCapacity, UI.instance.lvlMagCapacity, statLvlMagazine, 10); // mag
        UpdateStat(UI.instance.priceRegen, UI.instance.lvlRegen, statLvlRegen, 10, 1f, 1f); // regen
        UpdateStat(UI.instance.priceCards, UI.instance.lvlCards, statLvlCards, 3, 5f, 5f); // cards
    }
    public void UpdateStat(TextMeshProUGUI price, TextMeshProUGUI lvl, int statLvl, int maxLvl)
    {
        if (statLvl < maxLvl)
            price.text = Mathf.CeilToInt(statLvl * 0.5f + 0.25f).ToString();
        else price.text = "Max";
        lvl.text = "Lvl " + statLvl;
        UI.instance.pointsLeft.text = "points left: " + Player.instance.statPointsLeft.ToString();
        if (Instance.CheapestShopPrice() > Player.instance.statPointsLeft)
            UI.instance.statButton.GetComponent<Image>().color = new(1f, 1f, 1f, 0.5f);
    }
    public void UpdateStat(TextMeshProUGUI price, TextMeshProUGUI lvl, int statLvl, int maxLvl, float basePrice, float priceWithLvl)
    {
        if (statLvl < maxLvl)
            price.text = Mathf.CeilToInt(statLvl * priceWithLvl + basePrice).ToString();
        else price.text = "Max";
        lvl.text = "Lvl " + statLvl;
        UI.instance.pointsLeft.text = "points left: " + Player.instance.statPointsLeft.ToString();
    }
    public void LvlUpDamage()
    {
        if(statLvlDmg < 10 && Player.instance.statPointsLeft >= int.Parse(UI.instance.priceDmg.text))
        {
            statLvlDmg++;
            multDmg *= dmgMultPerLvl;
            Player.instance.statPointsLeft -= int.Parse(UI.instance.priceDmg.text);
            UpdateStat(UI.instance.priceDmg, UI.instance.lvlDmg, statLvlDmg, 10);
        }
    }
    public void LvlUpHp()
    {
        if (statLvlHp < 10 && Player.instance.statPointsLeft >= int.Parse(UI.instance.priceHp.text))
        {
            statLvlHp++;
            multHp *= hpMultPerLvl;
            Player.instance.health = Mathf.Floor(Player.instance.health * hpMultPerLvl);
            Player.instance.statPointsLeft -= int.Parse(UI.instance.priceHp.text);
            UpdateStat(UI.instance.priceHp, UI.instance.lvlHp, statLvlHp, 10);
            UI.instance.UpdateHpBar();
        }
    }
    public void LvlUpSped()
    {
        if (statLvlSpeed < 10 && Player.instance.statPointsLeft >= int.Parse(UI.instance.priceSpeed.text))
        {
            statLvlSpeed++;
            multSpeed *= speedMultPerLvl;
            Player.instance.statPointsLeft -= int.Parse(UI.instance.priceSpeed.text);
            UpdateStat(UI.instance.priceSpeed, UI.instance.lvlSpeed, statLvlSpeed, 10);
        }
    }
    public void LvlUpAccuracy()
    {
        if (statLvlAccuracy < 10 && Player.instance.statPointsLeft >= int.Parse(UI.instance.priceAccuracy.text))
        {
            statLvlAccuracy++;
            multAccuracy *= accMultPerLvl;
            Player.instance.statPointsLeft -= int.Parse(UI.instance.priceAccuracy.text);
            Firearm.instance.RecalcValues();
            UpdateStat(UI.instance.priceAccuracy, UI.instance.lvlAccuracy, statLvlAccuracy, 10);
        }
    }
    public void LvlUpMagazine()
    {
        if (statLvlMagazine < 10 && Player.instance.statPointsLeft >= int.Parse(UI.instance.priceMagCapacity.text))
        {
            statLvlMagazine++;
            multMagazine *= magMultPerLvl;
            Player.instance.statPointsLeft -= int.Parse(UI.instance.priceMagCapacity.text);
            Firearm.instance.RecalcValues();
            UpdateStat(UI.instance.priceMagCapacity, UI.instance.lvlMagCapacity, statLvlMagazine, 10);
            UI.instance.UpdateAmmoBar();
        }
    }
    public void LvlUpRegeneration()
    {
        if (statLvlRegen < 10 && Player.instance.statPointsLeft >= int.Parse(UI.instance.priceRegen.text))
        {
            statLvlRegen++;
            regen += regAddPerLvl;
            Player.instance.statPointsLeft -= int.Parse(UI.instance.priceRegen.text);
            Firearm.instance.RecalcValues();
            UpdateStat(UI.instance.priceRegen, UI.instance.lvlRegen, statLvlRegen, 10, 1f, 1f); 
        }
    }
    public void LvlUpCard()
    {
        if (statLvlCards < 3 && Player.instance.statPointsLeft >= int.Parse(UI.instance.priceCards.text))
        {
            statLvlCards++;
            baseCards += cardAddPerLvl;
            Player.instance.statPointsLeft -= int.Parse(UI.instance.priceCards.text);
            UpdateStat(UI.instance.priceCards, UI.instance.lvlCards, statLvlCards, 3, 5f, 5f); 
        }
    }
    public int CheapestShopPrice()
    {
        int[] prices = new int[7];
        if (statLvlHp != 10)
            prices[0] = int.Parse(UI.instance.priceHp.text);
        if (statLvlDmg != 10)
            prices[1] = int.Parse(UI.instance.priceDmg.text);
        if (statLvlSpeed != 10)
            prices[2] = int.Parse(UI.instance.priceSpeed.text);
        if (statLvlAccuracy != 10)
            prices[3] = int.Parse(UI.instance.priceAccuracy.text);
        if (statLvlMagazine != 10)
            prices[4] = int.Parse(UI.instance.priceMagCapacity.text);
        if (statLvlRegen != 10)
            prices[5] = int.Parse(UI.instance.priceRegen.text);
        if (statLvlCards != 3)
            prices[6] = int.Parse(UI.instance.priceCards.text);

        return Mathf.Min(prices);
    }

    public void SetStats(float baseDamage1, float baseHp1, float baseSpeed1, float baseMagazine1, float basePierce1, float baseBulletsPerShot1, 
                         float multDamage1, float multHp1, float multSpeed1, float multAccuracy1, float multMagazine1, float multFirerate1, float multReloadTime1, float multExp1)
    {
        baseDmg += baseDamage1;
        baseHp += baseHp1;
        baseSpeed += baseSpeed1;
        baseSpeed = Mathf.Clamp(baseSpeed, 1, 20);
        baseMagazine += baseMagazine1;
        baseMagazine = Mathf.Clamp(baseMagazine, 0, 100);
        basePierce += basePierce1;
        baseBulletsPerShot += baseBulletsPerShot1;

        multDmg *= multDamage1;
        multHp *= multHp1;
        multSpeed *= multSpeed1;
        multSpeed = Mathf.Clamp(multSpeed, 0.1f, 2);
        multAccuracy *= multAccuracy1;
        multMagazine *= multMagazine1;
        multMagazine = Mathf.Clamp(multMagazine, 0.1f, 100f);
        multFirerate *= multFirerate1;
        multReloadTime *= multReloadTime1;
        multExp *= multExp1;

        Firearm.instance.RecalcValues();
        UpdateStatValues();
    }

    public string GetStats()
    {
        string stats = "";

        stats += "Health : x" + multHp.ToString("0.00");
        stats += "\nBase health : " + baseHp.ToString("0.0");
        stats += "\nDamage : x" + multDmg.ToString("0.00");
        stats += "\nBase damage : " + baseDmg.ToString("0.0");
        stats += "\nSpeed : x" + multSpeed.ToString("0.00");
        stats += "\nBase speed : " + baseSpeed.ToString("0.0");
        stats += "\nAccuracy : x" + multAccuracy.ToString("0.00");
        stats += "\nReload Time : x" + multReloadTime.ToString("0.00");
        stats += "\nMagazine capasity: x" + multMagazine.ToString("0.00");
        stats += "\nBase magazine capasity : " + baseMagazine.ToString("0");
        stats += "\nFirerate : x" + multFirerate.ToString("0.00");
        stats += "\nBullets shot : +" + baseBulletsPerShot.ToString("0");
        stats += "\nBullet piercing : +" + basePierce.ToString("0");
        stats += "\nExperience : x" + multExp.ToString("0.00");
        stats += "\nRegeneration : " + (regen * 100).ToString("0") + "%";
        return stats;
    }
}
