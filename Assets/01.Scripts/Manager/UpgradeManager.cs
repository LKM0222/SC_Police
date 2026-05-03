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

    [Header("Data")]
    [Tooltip("현재 플레이어 채광 레벨")]public int mineLevel = 1;
    [Tooltip("업그레이드 할 감옥")]public Prison prison; 
    [Tooltip("잠금해제 시 활성화 할 광부 리스트")]public List<MineWorker> workerList;
    [Tooltip("잠금해제 시 활성화 할 NPC")]public ServeNPC npc;
    // [Tooltip("업그레이드 시작 위해 ")][SerializeField] private bool isGetMoneyFirst = false;

    [Header("UpgradeZone List")]
    [Tooltip("특정 상황에 활성화 할 UpgradeZone 리스트")]public List<UpgradeObject> upgradeObjectList;

    #region LifeCycle
    void Start()
    {
        Init();
    }
    #endregion

    #region Public Method
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
    #endregion

    #region Private Method
    private void Init()
    {
        upgradeObjectList.ForEach(x => x.upgradeObj.SetActive(false));
    }
    #endregion
}
