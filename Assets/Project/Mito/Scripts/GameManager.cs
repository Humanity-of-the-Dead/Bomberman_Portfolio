using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;
    public Action<bool> GameStarting;
    [Header("ゲーム時間(秒)")]
    [SerializeField] float gameTime = 10f;

    [Header("各スクリプトがついたGameObjectをここに")]
    [SerializeField] BombPool bombPool;
    [SerializeField] StageGenerator stageGenerator;
    [SerializeField] Timer timer;

    [Header("演出関連")]
    [SerializeField] GameObject countDownBackObj;
    [SerializeField] Image gameStateTitle;
    [SerializeField] Sprite gameStart;
    [SerializeField] Sprite kabooom;
    [SerializeField] float countDownSpeed = 0.4f;
    [SerializeField] float vanishmentUI = 0.8f;
    [SerializeField] float UIMoveSpeed = 15000f;
    TextMeshProUGUI countDownTextObj;
    Vector2 displayMidPoint = Vector2.zero;
    Vector2 displayOutPoint;
    Transform countDownBackTransform;
    Transform gameStateTitleTransform;
    bool timerActive = false;
    string[] countDownText = { "","1","2","3" };

    bool gameEnd = false;

    void Start()
    {
        if (Instance == null) Instance = this;
        else Destroy(this);
        gameEnd = false;
        timerActive = false;

        if (!countDownTextObj)
        {
            countDownTextObj = countDownBackObj.transform.GetChild(0).GetComponent<TextMeshProUGUI>();
            countDownTextObj.text = countDownText[0];
        }
        gameStateTitleTransform = gameStateTitle.transform;
        displayOutPoint = gameStateTitleTransform.localPosition;
        countDownBackTransform = countDownBackObj.transform;
        countDownBackTransform.localPosition = displayMidPoint;

        bombPool.Init();
        stageGenerator.Init();
        timer.Init(gameTime);

        if (SceneLoader.Ins)
        {
            SceneLoader.Ins.SceneChangeEnd += CountDown;
            SceneLoader.Ins.LoadSceneAsync("ResultScene");
        }

        if(AudioManager.Ins) AudioManager.Ins.PlayBGM(1);
    }

    void Update()
    {
        if(!timerActive) return;
        timer.TimeUpdate();
    }
    
    //void OnDestroy()
    //{
    //    // Instance が自分自身なら、参照をクリアする
    //    if (Instance == this)
    //    {
    //        Instance = null;
    //    }

    //    // SceneLoader のイベントから CountDown メソッドの登録を解除する
    //    if (SceneLoader.Ins != null)
    //    {
    //        SceneLoader.Ins.SceneChangeEnd -= CountDown;
    //    }
    //}
    
    /// <summary>
    /// ゲーム開始時カウントダウン
    /// </summary>
    async void CountDown()
    {
        if (timerActive) return;
        SceneLoader.Ins.CanControl = false;

        countDownTextObj.text = countDownText[3];
        await Awaitable.WaitForSecondsAsync(countDownSpeed);
        countDownTextObj.text = countDownText[2];
        await Awaitable.WaitForSecondsAsync(countDownSpeed);
        countDownTextObj.text = countDownText[1];
        await Awaitable.WaitForSecondsAsync(countDownSpeed);
        countDownTextObj.text = countDownText[0];

        gameStateTitle.sprite = gameStart;
        gameStateTitleTransform.localPosition = displayMidPoint;
        timerActive = true;
        SceneLoader.Ins.CanControl = true;
        GameStarting.Invoke(true);

        await Awaitable.WaitForSecondsAsync(vanishmentUI);

        countDownBackTransform.localPosition = displayOutPoint;
        gameStateTitleTransform.localPosition = displayOutPoint;
    }

    /// <summary>
    /// ゲームオーバー演出
    /// </summary>
    /// <returns></returns>
    async Awaitable GameOverRoutine()
    {
        gameStateTitle.sprite = kabooom;
        countDownBackTransform.localPosition = displayMidPoint;
        Vector2 _animationPos = displayOutPoint;
        while (true)
        {
            _animationPos.x -= UIMoveSpeed * Time.deltaTime;
            gameStateTitleTransform.localPosition = _animationPos;
            await Awaitable.EndOfFrameAsync();

            if (gameStateTitleTransform.localPosition.x <= displayMidPoint.x) break;
        }
        gameStateTitleTransform.localPosition = displayMidPoint;
        await Awaitable.WaitForSecondsAsync(vanishmentUI);
        while (true)
        {
            _animationPos.x -= UIMoveSpeed * Time.deltaTime;
            gameStateTitleTransform.localPosition = _animationPos;
            await Awaitable.EndOfFrameAsync();

            if (gameStateTitleTransform.localPosition.x <= -displayOutPoint.x) break;
        }
    }

    /// <summary>
    /// BombPoolの取得
    /// </summary>
    /// <returns>BombPool</returns>
    public BombPool GetBombPool() => bombPool;

    /// <summary>
    /// StageGeneratorの取得
    /// </summary>
    /// <returns>StageGenerator</returns>
    public StageGenerator GetStageGenerator(){
        return stageGenerator;
    }

    /// <summary>
    /// StageGeneratorから指定した座標にオブジェクトがないか調べる
    /// int 0 = up, 1 = right, 2 = down, 3 = left
    /// </summary>
    /// <param name="_currentPos"></param>
    /// <param name="_direction"></param>
    /// <returns></returns>
    public bool GetStageObject(Vector2 _currentPos, int _direction)
    {
        // _currentPos = 現在の座標
        // _direction = 方向 
        // int 0 = up, 1 = right, 2 = down, 3 = left
        return stageGenerator.GetStageObjectNext(_currentPos, _direction);
    }

    /// <summary>
    /// 指定した座標のオブジェクトを削除
    /// 破壊不可のブロックは対象外
    /// </summary>
    /// <param name="_coords"></param>
    public void StageObjectRemove(Vector2 _coords)
    {
        stageGenerator.StageObjectRemove(_coords);
    }

    public bool BombSearch(Vector2 _coords)
    {
        return bombPool.SearchCoordsDuplicate(_coords);
    }

    /// <summary>
    /// ゲーム終了処理
    /// </summary>
    /// <param name="_winner"></param>
    public async void GameSet(int _winner)
    {
        if (gameEnd) return;
        gameEnd = true;
        GameStarting.Invoke(false);
        ResultManager.Ins.WinnerDicade(_winner);
        timer.TimeStop();
        if (AudioManager.Ins) AudioManager.Ins.PlayOneShotSE(0);
        await GameOverRoutine();
        SceneLoader.Ins.ActivateScene("ResultScene");
    }
}
