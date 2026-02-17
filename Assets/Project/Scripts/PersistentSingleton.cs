using UnityEngine;

/// <summary>
/// シーンをまたいで存在するシングルトンクラスの汎用敵な実装
/// </summary>
/// <typeparam name="T">シングルトンとして実装するクラスの型</typeparam>
public class PersistentSingleton<T>:MonoBehaviour where T:PersistentSingleton<T>{
    static T _ins;//このクラスの唯一のインスタンスを保持する静的フィ―ルド

    /// <summary>
    /// このシングルトンの唯一のインスタンスにアクセスする
    /// </summary>
    public static T Ins{
        get{
            if(!_ins){
                Debugger.LogError(
                    $"[PersistentSingleton]{typeof(T)}の"+
                    "インスタンスがシーンないに存在しない"
                );
            }
            return _ins;
        }
    }

    protected virtual void Awake(){
        //インスタンスが存在しているとき
        if (_ins){
            //現在のゲームオブジェクトを破棄(重複を防ぐため)
            Destroy(gameObject);
        }
        else{
            _ins=(T)this;//現在のインスタンスを設定
            //このゲームオブジェクトがをシーン遷移時に破棄されないようにする
            DontDestroyOnLoad(gameObject);
        }
    }
}