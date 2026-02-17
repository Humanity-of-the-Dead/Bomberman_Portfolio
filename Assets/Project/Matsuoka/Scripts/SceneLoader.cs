using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// シーンの非同期読み込みを管理するシングルトン
/// </summary>
public class SceneLoader:PersistentSingleton<SceneLoader>{
    public Action SceneChangeEnd;
    AsyncOperation _asyncLoad;
    SceneCurtain sceneCurtain;
    bool canControl;
    public bool CanControl
    {
        get { return canControl; }
        set { canControl = value;}
    }

    bool _isSceneReady=false;//シーンの準備が完了したか

    protected override void Awake()
    {
        base.Awake();
        sceneCurtain = this.GetComponentInChildren<SceneCurtain>();
        sceneCurtain.Init();
        canControl = true;

        //SceneManagerに「シーンがロード完了したらOnSceneLoaded関数を呼んで」
        //と登録
        SceneManager.sceneLoaded+=OnSceneLoaded;
    }

    /// <summary>
    /// シーンが完全にロードされた時に実行
    /// シーン準備完了フラグを立てる
    /// </summary>
    async void OnSceneLoaded(Scene scene,LoadSceneMode mode){
        //全てのStart()が実行されるのを保障するため1f待つ
        await Awaitable.NextFrameAsync();
        _isSceneReady=true;
    }

    /// <summary>
    /// 指定されたシーンを非同期でロードします。
    /// </summary>
    /// <param name="sceneName">ロードするシーン名</param>
    public void LoadSceneAsync(string sceneName){
        // 既存のコルーチンが動いていれば停止（念のため）
        StopAllCoroutines();
        // 新しいロードコルーチンを開始
        StartCoroutine(LoadSceneCoroutine(sceneName));
    }

    /// <summary>
    /// シーンロード用のコルーチン
    /// </summary>
    IEnumerator LoadSceneCoroutine(string sceneName){
        _asyncLoad
            =SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
    
        //読み込みが完了しても自動でシーンをアクティブにしない
        _asyncLoad.allowSceneActivation = false; 

        Debug.Log("シーンのバックグラウンド読み込みを開始しました...");

        // 読み込みの進捗が90%になるまで待つ (0.9 は読み込み完了の目安)
        while (_asyncLoad.progress < 0.9f)
        {
            Debugger.Log("読み込み中: " +(_asyncLoad.progress * 100) + "%");
            yield return null; 
        }
        
        Debugger.Log("ロードおわり");
    }

    public async void ActivateScene(string sceneName){
        if (!_isSceneReady) return;
        _isSceneReady = false;//次のシーンロードに備えてフラグをリセット
        CanControl = false;
        await sceneCurtain.CurtainClose();
        
        _asyncLoad.allowSceneActivation = true;
        
        //次のシーン側から「準備完了」が報告されるまで、ここで待機
        while(!_isSceneReady)
        {
            await Awaitable.NextFrameAsync(); //1f待つ
        }
        await sceneCurtain.CurtainOpen();
        SceneChangeEnd.Invoke();
    }
}