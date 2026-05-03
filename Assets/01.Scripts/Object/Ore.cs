using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Ore : MonoBehaviour
{
    [Header("Data")]
    [Tooltip("광물 실제 오브젝트")][SerializeField] GameObject obj;
    [Tooltip("광물의 체력")][SerializeField] float hp = 2;
    [Tooltip("파괴 후, 다시 리스폰되는 시간")][SerializeField] float respawnTime;
    public bool isDestory => !obj.activeSelf;


    [Header("VFX")]
    [Tooltip("광물이 파괴되었을 때, 출력할 이펙트")][SerializeField] ParticleSystem destoryParticle;

    Coroutine respawnCoroutine = null;
    Player player;

    public GameObject own; // 현재 캐고있는 광물 주인




    public void Init(Player player)
    {
        this.player = player;
        Spawn();
    }

    public void Spawn()
    {
        obj.SetActive(true);
        own = null;
        destoryParticle.gameObject.SetActive(false);
    }

    // 캐릭터가 광물을 캘때(플레이어, NPC 포함)
    public void Mined(float atk)
    {
        if (isDestory) return;

        PlaySound(UpgradeManager.Instance.mineLevel);

        hp -= atk;

        // 체력이 0일때 파괴
        if (hp <= 0)
        {
            Destory();
        }
    }

    public bool WorkersMined(float atk)
    {
        PlaySound(1);

        if (isDestory)
        {
            //머신에 +1 해줘야됨.
            GameManager.Instance.machine.AddOre(1);
            return true;
        }
        else
        {
            hp -= atk;

            // 체력이 0일때 파괴
            if (hp <= 0)
            {
                obj.SetActive(false);
                destoryParticle.gameObject.SetActive(true);
                GameManager.Instance.machine.AddOre(1);

                // 리스폰 코루틴 실행
                if (respawnCoroutine != null)
                {
                    StopCoroutine(respawnCoroutine);
                    respawnCoroutine = null;
                }

                respawnCoroutine = StartCoroutine(RespawnCoroutine());
            }

            return false;
        }
    }

    private void Destory()
    {
        obj.SetActive(false);
        destoryParticle.gameObject.SetActive(true);
        player.DestroyOre(this);
        player.GetObject(ObjectType.Ore);

        // 리스폰 코루틴 실행
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
            respawnCoroutine = null;
        }

        respawnCoroutine = StartCoroutine(RespawnCoroutine());
    }

    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);
        Spawn();
    }


    private void PlaySound(int level)
    {
        if (level == 1) SoundManager.Instance.PlaySound(SoundType.Mining_level1);
        else SoundManager.Instance.PlaySound(SoundType.Mining_level2);
    }
}
