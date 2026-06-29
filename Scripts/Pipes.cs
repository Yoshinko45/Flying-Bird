using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 画面を左方向へ流れてくる土管（パイプ）の動きを管理するクラス。
/// 土管が画面左端を超えたら自動的に削除することで、
/// 不要なオブジェクトがシーンに残り続けるのを防ぐ。
/// </summary>
public class Pipes : MonoBehaviour
{
    #region 設定

    /// <summary>
    /// 土管が左方向へ移動する速さ（単位：Unity ワールド座標 / 秒）。
    /// Inspector から調整可能。値が大きいほど速く流れる。
    /// </summary>
    public float speed = 5f;

    #endregion

    #region 内部変数

    /// <summary>
    /// 画面左端の X ワールド座標。
    /// この座標より左に出た土管を削除する境界線として使用する。
    /// 余裕を持たせるため、実際の左端より 1 単位外側に設定する。
    /// </summary>
    private float LeftEdge;

    #endregion

    #region Unity コールバック

    /// <summary>
    /// オブジェクトが有効になった最初のフレームに一度だけ呼ばれる。
    /// メインカメラの左端座標を計算し、削除ラインとして保存する。
    /// </summary>
    private void Start()
    {
        // スクリーン座標の (0, 0, 0) = 画面左下の角をワールド座標に変換する。
        // そこから X 座標だけ取り出し、さらに 1f 引くことで
        // 画面外に完全に出てから削除されるようにしている。
        LeftEdge = Camera.main.ScreenToWorldPoint(Vector3.zero).x - 1f;
    }

    /// <summary>
    /// 毎フレーム呼ばれる更新メソッド。
    /// 土管を左方向へ移動させ、画面外に出たら自分自身を削除する。
    /// </summary>
    private void Update()
    {
        // Vector3.left は (-1, 0, 0) なので、speed と Time.deltaTime を掛けると
        // 「1秒間に speed 分だけ左へ移動する量」が求まる。
        // Time.deltaTime を掛けることでフレームレートが変わっても速度が一定になる。
        transform.position += Vector3.left * speed * Time.deltaTime;

        // 土管の X 座標が削除ラインより左に出たら、このゲームオブジェクトを削除する。
        // Destroy(gameObject) はシーンからオブジェクトを完全に取り除き、
        // メモリも解放されるので、使い終わったパイプの後始末に適している。
        if (transform.position.x < LeftEdge)
        {
            Destroy(gameObject);
        }
    }

    #endregion
}
