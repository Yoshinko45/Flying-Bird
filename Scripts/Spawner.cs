using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 一定間隔でパイプ（障害物）を自動生成するクラス。
/// このオブジェクトが有効になると自動でスポーンを開始し、
/// 無効になると停止する。
/// </summary>
public class Spawner : MonoBehaviour
{
    // ─────────────────────────────────────────
    // 設定項目（Inspector から変更可能）
    // ─────────────────────────────────────────

    #region 設定

    /// <summary>スポーンするパイプの Prefab（元になるオブジェクト）。</summary>
    public GameObject prefab;

    /// <summary>
    /// パイプをスポーンする間隔（秒）。
    /// 値を小さくするほどパイプの登場頻度が上がり、難しくなる。
    /// </summary>
    public float spawnRate = 1f;

    /// <summary>
    /// パイプが出現する Y 座標の最小値。
    /// この値より下にはパイプが出ない。
    /// </summary>
    public float minHeight = -1f;

    /// <summary>
    /// パイプが出現する Y 座標の最大値。
    /// この値より上にはパイプが出ない。
    /// </summary>
    public float maxHeight = 3.5f;

    #endregion

    // ─────────────────────────────────────────
    // Unity ライフサイクル
    // ─────────────────────────────────────────

    #region Unity コールバック

    /// <summary>
    /// このオブジェクトが有効（アクティブ）になったときに呼ばれる。
    /// InvokeRepeating を使って、spawnRate 秒ごとに Spawn() を繰り返し呼び出す。
    /// 第2引数（spawnRate）は「最初に呼ぶまでの待機時間」、
    /// 第3引数（spawnRate）は「その後の繰り返し間隔」。
    /// </summary>
    private void OnEnable()
    {
        InvokeRepeating(nameof(Spawn), spawnRate, spawnRate);
    }

    /// <summary>
    /// このオブジェクトが無効（非アクティブ）になったときに呼ばれる。
    /// InvokeRepeating を止めて、スポーンを停止する。
    /// OnEnable と対になるようにセットで書くのが基本。
    /// </summary>
    private void OnDisable()
    {
        CancelInvoke(nameof(Spawn));
    }

    #endregion

    // ─────────────────────────────────────────
    // スポーン処理
    // ─────────────────────────────────────────

    #region スポーン処理

    /// <summary>
    /// パイプを1つ生成し、Y 座標をランダムにずらす。
    /// InvokeRepeating によって定期的に自動呼び出される。
    /// </summary>
    private void Spawn()
    {
        // この Spawner オブジェクトの位置に Prefab を生成する
        // Quaternion.identity = 回転なし（デフォルトの向き）
        GameObject pipes = Instantiate(prefab, transform.position, Quaternion.identity);

        // minHeight ～ maxHeight の範囲でランダムな Y 方向のオフセットを加える
        // これにより、毎回異なる高さにパイプが出現してゲームに変化が生まれる
        pipes.transform.position += Vector3.up * Random.Range(minHeight, maxHeight);
    }

    #endregion
}