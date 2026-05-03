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
    [Tooltip("현재 캐고있는 광물의 주인")]public GameObject own;

    [Header("VFX")]
    [Tooltip("광물이 파괴되었을 때, 출력할 이펙트")][SerializeField] ParticleSystem destoryParticle;

    // 코루틴 중복방지
    Coroutine respawnCoroutine = null;
    
    // 프로퍼티
    public bool isDestory => !obj.activeSelf;
    
    #region Public Method
    public void Init()
    {
        Spawn();
    }

    // 오브젝트 스폰했을 떄
    public void Spawn()
    {
        obj.SetActive(true);
        own = null;
        hp = 2;
        destoryParticle.gameObject.SetActive(false);
    }

    // 플레이어가 광물을 캘 때
    public void Mined_Player(float atk)
    {
        if (isDestory) return;

        PlaySound(UpgradeManager.Instance.mineLevel);
        if(!destoryParticle.gameObject.activeSelf) destoryParticle.gameObject.SetActive(true);
        destoryParticle.Play();

        hp -= atk;

        // 체력이 0일때 파괴
        if (hp <= 0)
        {
            GameManager.Instance.player.DestroyOre(this);
            GameManager.Instance.player.GetObject(ObjectType.Ore);
            Destory();
        }
    }

    // 일꾼이 광물을 캘 때
    public void Mind_Workers(float atk)
    {
        if (isDestory) return;

        PlaySound(1);
        if (!destoryParticle.gameObject.activeSelf) destoryParticle.gameObject.SetActive(true);
        destoryParticle.Play();

        hp -= atk;

        // 체력이 0일때 파괴
        if (hp <= 0)
        {
            GameManager.Instance.machine.AddOre(1);
            Destory();
        }
    }
    #endregion

    #region Private Method
    private void Destory()
    {

        obj.SetActive(false);
        // 리스폰 코루틴 실행
        if (respawnCoroutine != null)
        {
            StopCoroutine(respawnCoroutine);
            respawnCoroutine = null;
        }

        respawnCoroutine = StartCoroutine(RespawnCoroutine());
    }

    private void PlaySound(int level)
    {
        if (level == 1) SoundManager.Instance.PlaySound(SoundType.Mining_level1);
        else SoundManager.Instance.PlaySound(SoundType.Mining_level2);
    }
    #endregion
    
    #region Coroutine
    IEnumerator RespawnCoroutine()
    {
        yield return new WaitForSeconds(respawnTime);
        Spawn();
    }
    #endregion
}
