using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class UpgradeObject
{
    public UpgradeType upgardeType;
    public GameObject upgradeObj;
}

public class UpgradeManager : MonoSingleton<UpgradeManager>
{
    public List<UpgradeObject> upgradeObjectList;

    public int mineLevel = 1;
    public Prison prison; 
    public List<MineWorker> workerList;
    public ServeNPC npc;

    [SerializeField] private bool isGetMoneyFirst = false;

    void Start()
    {
        Init();
    }

    void Update()
    {
        if (!isGetMoneyFirst && GameManager.Instance.money > 0)
        {
            SetUpgradeZone(UpgradeType.Weapon2, true);
            isGetMoneyFirst = true;
        }
    }

    private void Init()
    {
        upgradeObjectList.ForEach(x => x.upgradeObj.SetActive(false));
    }

    public void OpenUpgrade(UpgradeType type)
    {
        switch (type)
        {
            case UpgradeType.Weapon2:
                {
                    Debug.Log($"weapon2 개방");
                    SetUpgradeZone(UpgradeType.Weapon2, false);
                    mineLevel++;
                    GameManager.Instance.player.SetFindRange(mineLevel);

                    SetUpgradeZone(UpgradeType.Weapon3, true);
                    SetUpgradeZone(UpgradeType.MineWorker, true);
                }
                break;

            case UpgradeType.Weapon3:
                {
                    Debug.Log($"weapon3 개방");
                    SetUpgradeZone(UpgradeType.Weapon3, false);
                    mineLevel++;
                    GameManager.Instance.player.SetFindRange(mineLevel);
                }
                break;

            case UpgradeType.NPC:
                {
                    Debug.Log($"npc 개방");
                    SetUpgradeZone(UpgradeType.NPC, false);
                    npc.gameObject.SetActive(true);
                }
                break;

            case UpgradeType.Prison:
                {
                    Debug.Log($"Prison 개방");
                    SetUpgradeZone(UpgradeType.Prison, false);
                    prison.OpenArea();
                }
                break;

            case UpgradeType.MineWorker:
                {
                    Debug.Log($"Mine Worker 개방");
                    SetUpgradeZone(UpgradeType.MineWorker, false);
                    
                    workerList.ForEach(x => x.gameObject.SetActive(true));

                    SetUpgradeZone(UpgradeType.NPC, true);
                }
                break;
        }

        SoundManager.Instance.PlaySound(SoundType.BuySuccess);
    }

    public void SetUpgradeZone(UpgradeType type, bool active)
    {
        upgradeObjectList.Find(x => x.upgardeType.Equals(type)).upgradeObj.SetActive(active);
    }
}
