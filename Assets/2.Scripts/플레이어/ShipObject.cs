using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Newtonsoft.Json;

[CreateAssetMenu(fileName = "new Ship", menuName = "Scriptable Object/Ship Data", order = int.MaxValue)]
public class ShipObject : ScriptableObject
{
    public ShipObjectData shipObjectData = new ShipObjectData();

    // [Space]
    // public ShipType shipType;
    // public string shipCode;
    // public GameObject shipBody;
    // public Sprite shipImage;

    // [Space]
    // // 1000, 5, 3.5  700, 7, 4.5   1200 5.5 2.5
    // public Stat baseMaxHp = new Stat();
    // public Stat baseMoveSpeed = new Stat();
    // public Stat baseRotationSpeed = new Stat();
    // public Stat baseDamage = new Stat();

    // [Space]

    // public int shipCost;

    // [Space]

    // public List<WeaponObject> basicWeapon = new List<WeaponObject>();
    // public List<PassiveObject> basicPassive = new List<PassiveObject>();
    // public List<ShipUpgradeModules> shipUpgradeModuleList = new List<ShipUpgradeModules>();


}

[System.Serializable]
public class ShipObjectData
{
    public ShipType shipType;
    public string shipCode;
    [JsonIgnore]
    [SerializeField]
    public GameObject shipBody;
    [JsonIgnore]
    [SerializeField]
    public Sprite shipImage;

    [Space]
    // 1000, 5, 3.5  700, 7, 4.5   1200 5.5 2.5
    [SerializeField]
    public Stat baseMaxHp = new Stat();
    [SerializeField]
    public Stat baseMoveSpeed = new Stat();
    [SerializeField]
    public Stat baseRotationSpeed = new Stat();
    [SerializeField]
    public Stat baseDamage = new Stat();

    [Space]

    public int shipCost;

    [Space]

    [JsonIgnore]
    public List<WeaponObject> basicWeapon = new List<WeaponObject>();
    [JsonIgnore]
    public List<PassiveObject> basicPassive = new List<PassiveObject>();
    public List<ShipUpgradeModules> shipUpgradeModuleList = new List<ShipUpgradeModules>();

    public ShipUpgradeModules GetModule(ShipUpgradeType type)
    {
        for (int i = 0; i < shipUpgradeModuleList.Count; i++)
        {
            if (shipUpgradeModuleList[i].upgradeType == type)
                return shipUpgradeModuleList[i];
        }

        return null;
    }

    /// <summary>
    /// ??? ????????? ?????? ????????? ??????????????? ?????? ??????
    /// </summary>
    public void UpgradeShipData(ShipUpgradeType upgradeType, ShipObjectData lastestShipData)
    {
        var module = lastestShipData.GetCurrentModule(upgradeType);

        switch (upgradeType)
        {
            case ShipUpgradeType.Health:
                baseMaxHp.AddFloatModifier(module.statUpgradeValueForLevel);
                break;

            case ShipUpgradeType.Damage:
                baseDamage.AddFloatModifier(module.statUpgradeValueForLevel);
                break;

            case ShipUpgradeType.Speed:
                baseMoveSpeed.AddFloatModifier(module.statUpgradeValueForLevel);
                baseRotationSpeed.AddFloatModifier(module.statUpgradeValueForLevel);
                break;

        }
    }

    /// <summary>
    /// upgradeType??? ?????? ????????? ???????????? ??????
    /// </summary>
    public ShipUpgradeModules GetCurrentModule(ShipUpgradeType upgradeType)
    {
        for (int i = 0; i < shipUpgradeModuleList.Count; i++)
        {
            if (shipUpgradeModuleList[i].upgradeType == upgradeType)
                return shipUpgradeModuleList[i];
        }

        Debug.LogError("???????????? ?????? ????????? ????????????.");
        return null;
    }
}

[System.Serializable]
public class ShipUpgradeModules
{
    public ShipUpgradeType upgradeType;
    public int currentUpgrade = 0;
    public int maxUpgrade = 5;

    [Space]

    public float statUpgradeValueForLevel = 0;
    public int upgradeCostForLevel = 100;

    public void UpgradeThisModule(ShipObjectData currentData)
    {
        currentUpgrade++;

        // switch (upgradeType)
        // {
        //     case ShipUpgradeType.Health:
        //         currentData.baseMaxHp.AddFloatModifier(statUpgradeValueForLevel);
        //         break;

        //     case ShipUpgradeType.Damage:
        //         currentData.baseDamage.AddFloatModifier(statUpgradeValueForLevel);
        //         break;

        //     case ShipUpgradeType.Speed:
        //         currentData.baseMoveSpeed.AddFloatModifier(statUpgradeValueForLevel);
        //         currentData.baseRotationSpeed.AddFloatModifier(statUpgradeValueForLevel);
        //         break;

        // }
    }

    public int GetUpgradeCost()
    {
        return upgradeCostForLevel * (currentUpgrade);
    }
}

public enum ShipUpgradeType { Health, Damage, Speed }
public enum ShipType { Attacker, Scout, Boomber, Carrier, BattleShip }
