using UnityEngine;

public class BombPool : MonoBehaviour
{
    [Header("予め生成するボムの量")]
    [SerializeField] int bombAmount = 3;

    [Header("ボムのプレハブ")]
    [SerializeField] GameObject bomb;

    Transform myTransform;
    Bomb[] pooledBombList;

    // 初期化処理
    public void Init()
    {
        myTransform = transform;
        pooledBombList = new Bomb[bombAmount];

        GameObject _obj;
        for(int i = 0; i < bombAmount; i++)
        {
            _obj = Instantiate(bomb, myTransform.transform);
            pooledBombList[i] = _obj.GetComponent<Bomb>();
            pooledBombList[i].Init();
            _obj.SetActive(false);
        }
    }

    /// <summary>
    /// 指定した座標にボムが設置されていないか調べる
    /// </summary>
    /// <param name="_searchCoords"></param>
    /// <returns></returns>
    public bool SearchCoordsDuplicate(Vector2 _searchCoords)
    {
        for(int i = 0;i < bombAmount; i++)
        {
            if (pooledBombList[i].gameObject.activeInHierarchy)
            {
                if(_searchCoords.x == pooledBombList[i].gameObject.transform.position.x)
                {
                    return true;
                }
                if (_searchCoords.y == pooledBombList[i].gameObject.transform.position.y)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// プールから非アクティブなオブジェクトを返す
    /// </summary>
    /// <returns></returns>
    public GameObject GetPooledBomb()
    {
        for(int i = 0; i< bombAmount; i++)
        {
            if (!pooledBombList[i].gameObject.activeInHierarchy)
            {
                return pooledBombList[i].gameObject;
            }
        }
        return null;
    }
}
