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
    public bool openPrison;
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
            upgradeObjectList.Find(x => x.upgardeType.Equals(UpgradeType.Weapon2)).upgradeObj.SetActive(true);
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
                    upgradeObjectList.Find(x => x.upgardeType.Equals(UpgradeType.Weapon2)).upgradeObj.SetActive(false);
                    mineLevel++;
                    GameManager.Instance.player.SetFindRange(mineLevel);

                    upgradeObjectList.Find(x => x.upgardeType.Equals(UpgradeType.Weapon3)).upgradeObj.SetActive(true);
                    upgradeObjectList.Find(x => x.upgardeType.Equals(UpgradeType.MineWorker)).upgradeObj.SetActive(true);
                }
                break;

            case UpgradeType.Weapon3:
                {
                    Debug.Log($"weapon3 개방");
                    upgradeObjectList.Find(x => x.upgardeType.Equals(UpgradeType.Weapon3)).upgradeObj.SetActive(false);
                    mineLevel++;
                    GameManager.Instance.player.SetFindRange(mineLevel);
                }
                break;

            case UpgradeType.NPC:
                {
                    Debug.Log($"npc 개방");
                    upgradeObjectList.Find(x => x.upgardeType.Equals(UpgradeType.NPC)).upgradeObj.SetActive(false);
                    npc.gameObject.SetActive(true);
                }
                break;

            case UpgradeType.Prison:
                {
                    Debug.Log($"Prison 개방");
                    upgradeObjectList.Find(x => x.upgardeType.Equals(UpgradeType.Prison)).upgradeObj.SetActive(false);
                }
                break;

            case UpgradeType.MineWorker:
                {
                    Debug.Log($"Mine Worker 개방");
                    upgradeObjectList.Find(x => x.upgardeType.Equals(UpgradeType.MineWorker)).upgradeObj.SetActive(false);
                    workerList.ForEach(x => x.gameObject.SetActive(true));

                    upgradeObjectList.Find(x => x.upgardeType.Equals(UpgradeType.NPC)).upgradeObj.SetActive(true);
                }
                break;
        }

        SoundManager.Instance.PlaySound(SoundType.BuySuccess);
    }
}
