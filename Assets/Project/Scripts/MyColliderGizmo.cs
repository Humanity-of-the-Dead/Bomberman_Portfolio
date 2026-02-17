#if UNITY_EDITOR
using UnityEngine;

public class MyColliderGizmo : MonoBehaviour
{
    void OnDrawGizmos()
    {
        // ギズモの色を任意の色に設定
        Gizmos.color = Color.green;

        // Gizmos.matrixにオブジェクトのローカル座標系を設定
        Gizmos.matrix = transform.localToWorldMatrix;

        Collider col = GetComponent<Collider>();
        if (col != null)
        {
            if (col is BoxCollider box)
            {
                // ボックスコライダーのローカル座標での中心とサイズを取得
                // box.centerはローカル座標での中心、box.sizeはローカル座標でのサイズ
                Gizmos.DrawWireCube(box.center, box.size);
            }
            else if (col is SphereCollider sphere)
            {
                // スフィアコライダーのローカル座標での中心と半径を取得
                Gizmos.DrawWireSphere(sphere.center, sphere.radius);
            }
        }
    }
}
#endif