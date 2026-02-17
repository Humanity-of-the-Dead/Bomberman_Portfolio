#if UNITY_EDITOR
    using UnityEngine;

    /// <summary>
    /// エディタ上でギズモを描画するためのユーティリティクラス
    /// </summary>
    public static class EditorGizmos{
        /// <summary>
        /// 最小座標と最大座標から直方体のワイヤーフレームギズモを描画
        /// </summary>
        /// <param name="min">直方体の最小座標(Min)</param>
        /// <param name="max">直方体の最大座標(Max)</param>
        /// <param name="color">ギズモの色。指定しない場合は緑色</param>
        public static void DrawWireCube(
            Vector3 min,Vector3 max,Color?color=null
        ){
            // 2つのベクトルからBoundsを生成
            var bounds=new Bounds();
            bounds.SetMinMax(min,max);

            // Boundsを使って描画するメソッドを呼び出す
            DrawWireCube(bounds,color);
        }

        /// <summary>
        /// Bounds構造体から直方体のワイヤーフレームギズモを描画します。
        /// </summary>
        /// <param name="bounds">描画するBounds</param>
        /// <param name="color">ギズモの色。指定しない場合は緑色になります。</param>
        public static void DrawWireCube(Bounds bounds,Color?color=null){
            // 元の色を一時的に保存
            Color originalColor=Gizmos.color;

            // 色が指定されていればその色に、指定がなければ緑色に設定
            Gizmos.color=color??Color.green;

            // ギズモを描画
            Gizmos.DrawWireCube(bounds.center,bounds.size);

            // ギズモの色を元に戻す
            Gizmos.color=originalColor;
        }
    }
#endif