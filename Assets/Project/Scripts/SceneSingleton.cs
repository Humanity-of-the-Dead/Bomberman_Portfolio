using UnityEngine;

/// <summary>
/// 現在のシーン内でのみ有効なシングルトンクラスの汎用的な実装
/// </summary>
/// <typeparam name="T">シングルトンとして実装するクラスの型</typeparam>
public class SceneSingleton<T>:MonoBehaviour where T : SceneSingleton<T>{
    // このシーン内の唯一のインスタンスを保持する静的フィールド
    private static T _ins;

    /// <summary>
    /// このシングルトンの唯一のインスタンスにアクセスする
    /// インスタンスが存在しない場合、シーン内から自動で検索
    /// </summary>
    public static T Ins{
        get{
            //まだインスタンスがキャッスされてないとき
            if(!_ins){
                //シーン内からT型のっコンポーネントを検索して取得
                _ins=FindAnyObjectByType<T>();
                
                //見つからないとき
                if(!_ins){
                    Debugger.LogError(
                        $"[SceneSingleton]{typeof(T)}のインスタンスが"+
                        "シーン内に存在しない");
                }
            }
            return _ins;
        }
    }

    // 派生クラスで初期化処理をオーバーライド可能
    protected virtual void Awake(){
        // 既にインスタンスが存在する場合、重複するゲームオブジェクトを破棄
        if(_ins&&_ins!=this){
            Destroy(gameObject);
        }
        else{
            // 現在のインスタンスを唯一のインスタンスとして設定
            _ins=(T)this;
        }
    }
}