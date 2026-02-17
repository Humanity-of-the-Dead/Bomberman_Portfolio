using System.Collections;
using UnityEngine;

public class StageGenerator : MonoBehaviour
{
    [Header("ステージ情報")]
    [SerializeField] Vector2 stageOrigin = Vector2.zero;
    [SerializeField] int stageWidth = 13;
    [SerializeField] int stageHeight = 11;
    [SerializeField]float _tileSize = 1f;

    [Header("破壊可能ブロックの配置確率(0~100)")]
    [Range(0, 100)]
    [SerializeField] int blockPuttingChance = 90;

    [Header("プレハブ")]
    [SerializeField] GameObject unbreakBlock;
    [SerializeField] GameObject block;
    [SerializeField] GameObject player1;
    [SerializeField] GameObject player2;

    Transform myTransform;
    GameObject[] stageStatus;

    /// <summary>
    /// 初期化処理
    /// </summary>
    public void Init()
    {
        myTransform = transform;
        // StageObjectInit();
        StartCoroutine(StageObjectInit());
    }

    // /// <summary>
    // /// ステージのオブジェクト設置
    // /// </summary>
    // private void StageObjectInit()
    // {
    //     stageStatus = new GameObject[stageHeight * stageWidth];
    //     GameObject _obj;
    //     Vector2 _position;
    //
    //     // 破壊不能オブジェクト配置
    //     for (int i = 0; i < stageHeight * stageWidth; i++)
    //     {
    //         //Debug.Log(i + " : " + (i % stageHeight) % 2);
    //         if ((i / stageWidth) % 2 == 1 && (i % stageWidth) % 2 == 1)
    //         {
    //             _obj = Instantiate(unbreakBlock, myTransform.transform);
    //             stageStatus[i] = _obj;
    //
    //             //Debug.Log(i);
    //             _position.x = stageOrigin.x + i % stageWidth;
    //             _position.y = stageOrigin.y + (i / stageWidth) * -1;
    //
    //             _obj.transform.position = _position;
    //         }
    //         else stageStatus[i] = null;
    //     }
    //
    //     // 破壊可能オブジェクト配置
    //     // 四隅には配置しない
    //     for (int i = 0; i < stageHeight * stageWidth; i++)
    //     {
    //         //Debug.Log(i % stageWidth);
    //         if (i % stageWidth <= 1 || i % stageWidth >= stageWidth - 2)
    //         {
    //             if (i / stageWidth <= 1 || i / stageWidth >= stageHeight - 2)
    //                 continue;
    //         }
    //
    //         if (stageStatus[i] == null)
    //         {
    //             if (Random.Range(0, 100 + 1) <= blockPuttingChance)
    //             {
    //                 _obj = Instantiate(block, myTransform);
    //                 stageStatus[i] = _obj;
    //
    //                 _position.x = stageOrigin.x + i % stageWidth;
    //                 _position.y = stageOrigin.y + (i / stageWidth) * -1;
    //
    //                 _obj.transform.position = _position;
    //             }
    //         }
    //     }
    //
    //     // プレイヤー1配置
    //     _obj = Instantiate(player1);
    //
    //     _position.x = stageOrigin.x;
    //     _position.y = stageOrigin.y;
    //
    //     _obj.transform.position = _position;
    //
    //     // プレイヤー２配置
    //     _obj = Instantiate(player2);
    //
    //     _position.x = stageWidth - 1;
    //     _position.y = (stageHeight - 1) * -1;
    //
    //     _obj.transform.position = _position;
    // } 
    
    /// <summary>
    /// ステージのオブジェクト配置(コルーチン)
    /// </summary>
    IEnumerator StageObjectInit(){
        stageStatus=new GameObject[stageHeight*stageWidth];
        GameObject _obj;
        Vector2 _position;
        
        int instantiatedCount=0;//フレーム分割用カウンタ

        // プレイヤー1配置
        _obj = Instantiate(player1);

        _position.x = stageOrigin.x;
        _position.y = stageOrigin.y;

        _obj.transform.position = _position;

        // プレイヤー２配置
        _obj = Instantiate(player2);

        _position.x = stageWidth - 1;
        _position.y = (stageHeight - 1) * -1;

        _obj.transform.position = _position;

        // 破壊不能オブジェクト配置
        for (int i=0; i<stageHeight*stageWidth; i++){
            //Debug.Log(i + " : " + (i % stageHeight) % 2);
            if((i/stageWidth)%2==1&&(i%stageWidth)%2==1){
                _obj=Instantiate(unbreakBlock,myTransform.transform);
                stageStatus[i]=_obj;

                //Debug.Log(i);
                _position.x=stageOrigin.x+i%stageWidth;
                _position.y=stageOrigin.y+(i/stageWidth)*-1;

                _obj.transform.position=_position;
                
                instantiatedCount++;
                //10個生成ごとに1f待つ
                if(instantiatedCount%1==0) yield return null;
            }
            else stageStatus[i]=null;
        }

        // 破壊可能オブジェクト配置
        // 四隅には配置しない
        for (int i=0; i<stageHeight*stageWidth; i++){
            //Debug.Log(i % stageWidth);
            if(i%stageWidth<=1||i%stageWidth>=stageWidth-2){
                if(i/stageWidth<=1||i/stageWidth>=stageHeight-2)
                    continue;
            }

            if(stageStatus[i]==null){
                if(Random.Range(0,100+1)<=blockPuttingChance){
                    _obj=Instantiate(block,myTransform);
                    stageStatus[i]=_obj;

                    _position.x=stageOrigin.x+i%stageWidth;
                    _position.y=stageOrigin.y+(i/stageWidth)*-1;

                    _obj.transform.position=_position;

                    instantiatedCount++;
                    //10個生成ごとに1f待つ
                    if(instantiatedCount%1==0) yield return null;
                }
            }
        }
    }

    /// <summary>
    /// 引数の数字の座標にオブジェクトがあるかどうか。
    /// true = ある
    /// </summary>
    /// <param name="_currentPos"></param>
    /// <returns></returns>
    bool GetStageObject(Vector2 _currentPos)
    {
        if (stageStatus[(int)_currentPos.x + stageWidth * ((int)_currentPos.y * -1)] == null) return false;
        else return true;
    }

    /// <summary>
    /// 現在位置と方向から次の場所へ移動できるかを返す。
    /// int 0 = up, 1 = right, 2 = down, 3 = left
    /// </summary>
    /// <param name="_currentPos"></param>
    /// <param name="_direction"></param>
    /// <returns></returns>
    public bool GetStageObjectNext(Vector2 _currentPos, int _direction)
    {
        _currentPos = CoordRound(_currentPos);

        switch (_direction)
        {
            case 0: // up
                _currentPos.y += 1;
                if(_currentPos.y > 0) return true;
                break;
            case 1: // right
                if (_currentPos.x % stageWidth == 12) return true;
                _currentPos.x += 1;
                break;
            case 2: // down
                _currentPos.y -= 1;
                if (_currentPos.y <= -stageHeight) return true;
                break;
            case 3: // left
                if (_currentPos.x % stageWidth == 0) return true;
                _currentPos.x -= 1;
                break;
            default:
                return true;
        }

        return GetStageObject(_currentPos);
    }

    /// <summary>
    /// 第1引数の座標に爆弾オブジェクトを設置
    /// </summary>
    /// <param name="_currentPos"></param>
    /// <param name="_obj"></param>
    public void BombSet(Vector2 _currentPos,GameObject _obj,Player.Direction d)
    {
        //プレイヤの向いている方向dによる、逆の値
        var temp = new[]
        {
            -Vector2Int.up,-Vector2Int.down,
            -Vector2Int.left,-Vector2Int.right
        };
        
        _currentPos = CoordRound(_currentPos);
        
        //Debugger.Log("currentpos"+_currentPos);

        //stageStatusのインデックス
        var i=(int)_currentPos.x+stageWidth*((int)_currentPos.y*-1);
        
        //Debugger.Log("i"+i);

        //タイルの範囲外に置こうとしたら
        if(_currentPos.x < 0 || _currentPos.x >= stageWidth || 
            _currentPos.y > 0 || _currentPos.y <= -stageHeight)
        {
            _obj.transform.position = _currentPos+temp[(int)d];
            //Debug.Log(i + temp[(int)d].x + temp[(int)d].y * stageWidth * -1);
            stageStatus[i + temp[(int)d].x + temp[(int)d].y * stageWidth * -1] = _obj;
        }
        //そのタイルに何も無ければボムを置く
        else if(!stageStatus[i]){
            //Debugger.Log("通常");
            _obj.transform.position = _currentPos;
            stageStatus[i]= _obj;
        }
        else{
            _obj.transform.position = _currentPos+temp[(int)d];
            stageStatus[i+temp[(int)d].x+temp[(int)d].y*stageWidth*-1]=_obj;
        }

        if (!GameManager.Instance.BombSearch(_obj.transform.position))
        {
            _obj.SetActive(true);
        }
        else _obj.SetActive(false);
    }

    /// <summary>
    /// 引数の座標のオブジェクトを非アクティブ化して配列から削除する
    /// 破壊不可ブロックは対象外
    /// </summary>
    /// <param name="_coords"></param>
    public void StageObjectRemove(Vector2 _coords)
    {
        _coords = CoordRound(_coords);

        if (_coords.y > 0 || _coords.y <= -stageHeight) return;
        if (_coords.x < 0 || _coords.x >= stageWidth) return;

        if (stageStatus[(int)_coords.x + stageWidth * ((int)_coords.y * -1)].gameObject.CompareTag("Player")) return;

        stageStatus[(int)_coords.x + stageWidth * ((int)_coords.y * -1)].gameObject.SetActive(false);
        stageStatus[(int)_coords.x + stageWidth * ((int)_coords.y * -1)] = null;
    }

    /// <summary>
    /// 座標の小数第一位を四捨五入して整数にする
    /// </summary>
    /// <param name="_coords"></param>
    /// <returns></returns>
    Vector2 CoordRound(Vector2 _coords)
    {
        if (_coords.x < (int)_coords.x + 0.5f)
        {
            _coords.x = (int)_coords.x;
        }
        else _coords.x = (int)_coords.x + 1;

        if (_coords.y > (int)_coords.y - 0.5f)
        {
            _coords.y = (int)_coords.y;
        }
        else _coords.y = (int)_coords.y - 1;

        return _coords;
    }

    public Vector2 GetStageOrigin(){
        return stageOrigin;
    }

    public int GetStageHeight(){
        return stageHeight;
    }

    public int GetStageWidth(){
        return stageWidth;
    }

    public float GetTileSize(){
        return _tileSize;
    }
}
