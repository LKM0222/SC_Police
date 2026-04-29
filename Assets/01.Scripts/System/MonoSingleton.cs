using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 싱글톤 클래스로 만들기 위한 스크립트
// 이 스크립트를 상속받으면, 싱글톤 스크립트가 됨.
public abstract class MonoSingleton<T> : MonoBehaviour where T : MonoSingleton<T>
{
    public static T Instance { get; private set; }

    protected void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this as T;

    }

    protected void OnDestroy()
    {
        if (Instance != this as T)
        {
            return;
        }

        Instance = null;
    }

    protected virtual void OnAwakeRoutine() { }
    protected virtual void OnDestroyRoutine() { }
}
