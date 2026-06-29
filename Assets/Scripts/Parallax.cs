using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 背景をスクロールさせてパララックス（視差）効果を実現するクラス。
/// MeshRenderer のマテリアルに設定されたテクスチャのオフセットを
/// 毎フレーム少しずつずらすことで、背景が流れているように見せる。
/// </summary>
public class Parallax : MonoBehaviour
{
    #region コンポーネント

    /// <summary>
    /// このオブジェクトにアタッチされた MeshRenderer。
    /// テクスチャのオフセット（ずれ量）を操作するために使用する。
    /// </summary>
    private MeshRenderer meshRenderer;

    #endregion

    #region 設定

    /// <summary>
    /// 背景スクロールの速さ。
    /// 値が大きいほど背景が速く流れる。
    /// Inspector から自由に変更可能。
    /// </summary>
    public float animationSpeed = 1f;

    #endregion

    #region Unity コールバック

    /// <summary>
    /// オブジェクト生成時に一度だけ呼ばれる初期化メソッド。
    /// 自分自身にアタッチされた MeshRenderer を取得しておく。
    /// </summary>
    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// 毎フレーム呼ばれる更新メソッド。
    /// テクスチャのオフセットを X 方向にずらし続けることで
    /// 背景が横方向に流れているように見せる。
    /// </summary>
    private void Update()
    {
        // 現在のテクスチャオフセット（UV 座標のずれ量）を取得する
        Vector2 offset = meshRenderer.material.mainTextureOffset;

        // スクロール速度 × 経過時間（秒） を X 方向に加算する。
        // Time.deltaTime を掛けることで、フレームレートに依存しない
        // 一定速度のスクロールを実現できる。
        offset += new Vector2(animationSpeed * Time.deltaTime, 0);

        // 計算した新しいオフセットをマテリアルに書き戻す。
        // これによって次フレームの描画に反映される。
        meshRenderer.material.mainTextureOffset = offset;
    }

    #endregion
}