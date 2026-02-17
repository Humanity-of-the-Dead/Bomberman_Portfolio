using System;
using UnityEngine;

public class Player:MonoBehaviour{
    public enum Direction{
        Up,
        Down,
        Left,
        Right
    };

    public Action<int> PlayerDeath;
    
    [SerializeField]bool _isP1=true;//Player1か
    [SerializeField]float _speed=1f;
    
    MyJoyConManager.IJoyCon _joyCon;
    
    Rigidbody2D _rb2;
    Transform _transform;
    SpriteRenderer mySpriteRenderer;
    
    Direction _direction;//現在の向いている方向

    BombPool _bombPool;
    StageGenerator _stageGenerator;
    Animator myAnimator;
    
    //左上(0,0),正の方向が右上
    Vector2 _currentTilePos;//現在のタイルの場所
    Vector2 _bombSpawnTilePos;//ボムを生成するタイルの位置

    string[] playerAnimName =
    {
        "player_idle",          // 0
        "player_idle_back",     // 1
        "player_idle_side",     // 2
        "player_walk",          // 3
        "player_walk_back",     // 4
        "player_walk_side",     // 5
        "player_put_bomb",      // 6
        "player_put_bomb_back", // 7
        "player_put_bomb_side", // 8
        "player_death",         // 9
        "player_victory",       // 10
    };

    bool isPlayable = false;

    void Start(){
        if (MyJoyConManager.Ins != null)
        {
            _joyCon
                = _isP1 ? MyJoyConManager.Ins.joycon1 : MyJoyConManager.Ins.joycon2;
        }
        
        _rb2=GetComponent<Rigidbody2D>();
        _transform=GetComponent<Transform>();
        myAnimator = GetComponent<Animator>();
        mySpriteRenderer = GetComponent<SpriteRenderer>();
        
        _bombPool=GameManager.Instance.GetBombPool();
        _stageGenerator=GameManager.Instance.GetStageGenerator();
        GameManager.Instance.GameStarting += ChangePlayable;
        
        _direction=Direction.Down;
        UpdateTilePos();
        _bombSpawnTilePos = _currentTilePos + Vector2.down;

        isPlayable = SceneLoader.Ins.CanControl;
    }
    
    void FixedUpdate(){
        if (!isPlayable) return;
        Move();
        PlaceBomb();
    }

    /// <summary>
    /// プレイヤがどのタイルに居るか更新
    /// </summary>
    void UpdateTilePos(){
        Vector2 pos = _transform.position;//現在のx,y座標
        var tileSize=_stageGenerator.GetTileSize();//タイルの大きさ
        //タイルの左下の座標
        var so=_stageGenerator.GetStageOrigin();
        
        var relativePos=pos-so;//プレイヤの相対座標
        
        //タイルの場所を計算
        float x=relativePos.x/tileSize;
        float y=relativePos.y/tileSize;
        
        // Debugger.Log("relativePos.y"+relativePos.y);
        
        _currentTilePos = new Vector2(x,y);
    }

    /// <summary>
    /// 移動
    /// </summary>
    void Move(){
        if (_joyCon != null)
        {
            switch (_joyCon.StickDirection)
            {
                case MyJoyConManager.StickDirection.Neutral:
                    _rb2.linearVelocity = Vector3.zero;
                    // プレイヤーアニメ
                    switch (_direction)
                    {
                        case Direction.Up:
                            mySpriteRenderer.flipX = false;
                            myAnimator.Play(playerAnimName[1]);
                            break;
                        case Direction.Down:
                            mySpriteRenderer.flipX = false;
                            myAnimator.Play(playerAnimName[0]);
                            break;
                        case Direction.Left:
                            mySpriteRenderer.flipX = false;
                            myAnimator.Play(playerAnimName[2]);
                            break;
                        case Direction.Right:
                            mySpriteRenderer.flipX = true;
                            myAnimator.Play(playerAnimName[2]);
                            break;
                        default:
                            mySpriteRenderer.flipX = false;
                            myAnimator.Play(playerAnimName[0]);
                            break;
                    }
                    break;

                case MyJoyConManager.StickDirection.Up:
                    _rb2.linearVelocity = Vector3.up * _speed;
                    _direction = Direction.Up;

                    if (AudioManager.Ins) AudioManager.Ins.PlayOneShotSE(1);
                    mySpriteRenderer.flipX = false;
                    myAnimator.Play(playerAnimName[4]);
                    break;

                case MyJoyConManager.StickDirection.Down:
                    _rb2.linearVelocity = Vector3.down * _speed;
                    _direction = Direction.Down;

                    if (AudioManager.Ins) AudioManager.Ins.PlayOneShotSE(1);
                    mySpriteRenderer.flipX = false;
                    myAnimator.Play(playerAnimName[3]);
                    break;

                case MyJoyConManager.StickDirection.Left:
                    _rb2.linearVelocity = Vector3.left * _speed;
                    _direction = Direction.Left;

                    if (AudioManager.Ins) AudioManager.Ins.PlayOneShotSE(1);
                    mySpriteRenderer.flipX = false;
                    myAnimator.Play(playerAnimName[5]);
                    break;

                case MyJoyConManager.StickDirection.Right:
                    _rb2.linearVelocity = Vector3.right * _speed;
                    _direction = Direction.Right;

                    if (AudioManager.Ins) AudioManager.Ins.PlayOneShotSE(1);
                    mySpriteRenderer.flipX = true;
                    myAnimator.Play(playerAnimName[5]);
                    break;
            }
        }
        else // テスト用
        {
            if (Input.GetKey(KeyCode.W))
            {
                _rb2.linearVelocity = Vector3.up * _speed;
                _direction = Direction.Up;

                if (AudioManager.Ins) AudioManager.Ins.PlayOneShotSE(1);
                mySpriteRenderer.flipX = false;
                myAnimator.Play(playerAnimName[4]);
            }
            else if (Input.GetKey(KeyCode.A))
            {
                _rb2.linearVelocity = Vector3.left * _speed;
                _direction = Direction.Left;

                if (AudioManager.Ins) AudioManager.Ins.PlayOneShotSE(1);
                mySpriteRenderer.flipX = false;
                myAnimator.Play(playerAnimName[5]);
            }
            else if (Input.GetKey(KeyCode.S))
            {
                _rb2.linearVelocity = Vector3.down * _speed;
                _direction = Direction.Down;

                if (AudioManager.Ins) AudioManager.Ins.PlayOneShotSE(1);
                mySpriteRenderer.flipX = false;
                myAnimator.Play(playerAnimName[3]);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                _rb2.linearVelocity = Vector3.right * _speed;
                _direction = Direction.Right;

                if (AudioManager.Ins) AudioManager.Ins.PlayOneShotSE(1);
                mySpriteRenderer.flipX = true;
                myAnimator.Play(playerAnimName[5]);
            }
            else
            {
                _rb2.linearVelocity = Vector2.zero;

                mySpriteRenderer.flipX = false;
                // プレイヤーアニメ
                switch (_direction)
                {
                    case Direction.Up:
                        mySpriteRenderer.flipX = false;
                        myAnimator.Play(playerAnimName[1]);
                        break;
                    case Direction.Down:
                        mySpriteRenderer.flipX = false;
                        myAnimator.Play(playerAnimName[0]);
                        break;
                    case Direction.Left:
                        mySpriteRenderer.flipX = false;
                        myAnimator.Play(playerAnimName[2]);
                        break;
                    case Direction.Right:
                        mySpriteRenderer.flipX = true;
                        myAnimator.Play(playerAnimName[2]);
                        break;
                    default:
                        mySpriteRenderer.flipX = false;
                        myAnimator.Play(playerAnimName[0]);
                        break;
                }
            }
        }
    }
    
    /// <summary>
    /// 爆弾を設置する
    /// </summary>
    void PlaceBomb(){
        if (_joyCon != null)
        {
            if (_joyCon.J.isLeft)
            {
                //Joy-Conの右ボタンが押されたとき
                if (_joyCon.GetButtonsDown(Joycon.Button.DPAD_LEFT))
                {
                    UpdateTilePos();
                    //Playerの向きによって爆弾の設置場所を変える
                    switch (_direction)
                    {
                        case Direction.Up:
                            _bombSpawnTilePos = _currentTilePos + Vector2.up;

                            mySpriteRenderer.flipX = false;
                            myAnimator.Play(playerAnimName[7]);
                            break;

                        case Direction.Down:
                            _bombSpawnTilePos = _currentTilePos + Vector2.down;

                            mySpriteRenderer.flipX = false;
                            myAnimator.Play(playerAnimName[6]);
                            break;

                        case Direction.Left:
                            _bombSpawnTilePos = _currentTilePos + Vector2.left;

                            mySpriteRenderer.flipX = false;
                            myAnimator.Play(playerAnimName[8]);
                            break;

                        case Direction.Right:
                            _bombSpawnTilePos = _currentTilePos + Vector2.right;

                            mySpriteRenderer.flipX = true;
                            myAnimator.Play(playerAnimName[8]);
                            break;
                    }

                    if (AudioManager.Ins) AudioManager.Ins.PlayOneShotSE(2);
                    //ボムの設置
                    _stageGenerator.BombSet(
                        _bombSpawnTilePos, _bombPool.GetPooledBomb(), _direction
                    );
                }
            }
            else
            {
                //Joy-Conの右ボタンが押されたとき
                if (_joyCon.GetButtonsDown(Joycon.Button.DPAD_RIGHT))
                {
                    UpdateTilePos();
                    //Playerの向きによって爆弾の設置場所を変える
                    switch (_direction)
                    {
                        case Direction.Up:
                            _bombSpawnTilePos = _currentTilePos + Vector2.up;

                            mySpriteRenderer.flipX = false;
                            myAnimator.Play(playerAnimName[7]);
                            break;

                        case Direction.Down:
                            _bombSpawnTilePos = _currentTilePos + Vector2.down;

                            mySpriteRenderer.flipX = false;
                            myAnimator.Play(playerAnimName[6]);
                            break;

                        case Direction.Left:
                            _bombSpawnTilePos = _currentTilePos + Vector2.left;

                            mySpriteRenderer.flipX = false;
                            myAnimator.Play(playerAnimName[8]);
                            break;

                        case Direction.Right:
                            _bombSpawnTilePos = _currentTilePos + Vector2.right;

                            mySpriteRenderer.flipX = true;
                            myAnimator.Play(playerAnimName[8]);
                            break;
                    }

                    if (AudioManager.Ins) AudioManager.Ins.PlayOneShotSE(2);
                    //ボムの設置
                    _stageGenerator.BombSet(
                        _bombSpawnTilePos, _bombPool.GetPooledBomb(), _direction
                    );
                }
            }
        }
        else if(Input.GetKeyDown(KeyCode.F))
        {
            UpdateTilePos();

            //Playerの向きによって爆弾の設置場所を変える
            switch (_direction)
            {
                case Direction.Up:
                    _bombSpawnTilePos = _currentTilePos + Vector2.up;

                    mySpriteRenderer.flipX = false;
                    myAnimator.Play(playerAnimName[7]);
                    break;

                case Direction.Down:
                    _bombSpawnTilePos = _currentTilePos + Vector2.down;

                    mySpriteRenderer.flipX = false;
                    myAnimator.Play(playerAnimName[6]);
                    break;

                case Direction.Left:
                    _bombSpawnTilePos = _currentTilePos + Vector2.left;

                    mySpriteRenderer.flipX = false;
                    myAnimator.Play(playerAnimName[8]);
                    break;

                case Direction.Right:
                    _bombSpawnTilePos = _currentTilePos + Vector2.right;

                    mySpriteRenderer.flipX = true;
                    myAnimator.Play(playerAnimName[8]);
                    break;
            }

            if (AudioManager.Ins) AudioManager.Ins.PlayOneShotSE(2);
            //ボムの設置
            _stageGenerator.BombSet(
                _bombSpawnTilePos, _bombPool.GetPooledBomb(), _direction
            );
        }
    }

/// <summary>
/// プレイヤーを操作可能にする
/// </summary>
    void ChangePlayable(bool _value)
    {
        if(isPlayable)
        {
            myAnimator.Play(playerAnimName[10]);
        }
        isPlayable = _value;
    }

    void OnTriggerEnter2D(Collider2D other){
        if(other.CompareTag("Die")){
            int winner = 0;
            if(_isP1) winner = 1;
            GameManager.Instance.GameSet(winner);
            mySpriteRenderer.flipX = false;
            myAnimator.Play(playerAnimName[9]);
            //Debug.Log("爆風にあたった"+gameObject.name);
        }
    }
}
