using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mine : MonoBehaviour
{
    [Header("Mine Data")]
    [Tooltip("Ore를 스폰할 위치")][SerializeField] Transform parent;
    [Tooltip("소환할 Ore 프리팹")][SerializeField] Ore orePrefab;
    [Tooltip("가로 세로 몇개씩 소환할지 나타내는 벡터")][SerializeField] Vector2 array;
    [Tooltip("현재 소환되어있는 Ore 리스트")][SerializeField] List<Ore> oreList = new List<Ore>();

    bool isInit = false; // 초기화 체크

    #region Life Cycle
    void Start()
    {
        if (!isInit) Init();
    }
    #endregion

    #region Private Method
    // 게임 시작 시, 광물을 스폰하는 함수
    private void Init()
    {
        for (int i = 0; i < array.x * array.y; i++)
        {
            Ore ore = Instantiate(orePrefab, parent);
            ore.Init();
            oreList.Add(ore);
        }

        // 위치 지정
        for (int y = 0; y < array.y; y++)
        {
            for (int x = 0; x < array.x; x++)
            {
                Vector3 position = new Vector3(x, 0.5f, y);
                oreList[x + (int)(array.x * y)].gameObject.transform.localPosition = position;
            }
        }
    }
    #endregion
}
