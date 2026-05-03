using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable] public enum SoundType{ BuySuccess, BuyUpgrade, GetItem, GetMoney, ItemMaking, Mining_level1, Mining_level2, OreBreak_level2, OreStacking, PayMoney }

[Serializable]
public class ClipInfo
{
    public SoundType type;
    public AudioSource source;
}


public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField] List<ClipInfo> clipList;


    public void PlaySound(SoundType type)
    {
        clipList.Find(x => x.type.Equals(type)).source.Play();
    }
}
