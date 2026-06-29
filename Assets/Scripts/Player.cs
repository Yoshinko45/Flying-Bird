using UnityEngine;

/// <summary>
/// プレイヤー（鳥）の入力・物理移動・アニメーション・衝突判定を管理するクラス。
/// スペースキーまたは画面タップでジャンプし、重力で落下する。
/// 障害物に当たればゲームオーバー、スコアゾーンを通過すればスコア加算。
/// </summary>
public class Player : MonoBehaviour
{
    #region スプライトアニメーション

    /// <summary>
    /// パラパラ漫画のように切り替えるスプライト画像の配列。
    /// Inspector から鳥の羽ばたきコマ画像をセットする。
    /// </summary>
    public Sprite[] sprites;

    /// <summary>
    /// スプライトをシーンに描画するコンポーネント。
    /// 実行時に自動取得するので Inspector での設定は不要。
    /// </summary>
    private SpriteRenderer spriteRenderer;

    /// <summary>
    /// 現在表示中のスプライトが sprites 配列の何番目かを示すインデックス。
    /// AnimateSprite() で順番に進められる。
    /// </summary>
    private int spriteIndex;

    #endregion

    #region 移動関連

    /// <summary>
    /// ジャンプ時に Y 方向へ加える速度の大きさ。
    /// 値が大きいほど高く跳び上がる。Inspector から調整可能。
    /// </summary>
    public float strength = 5f;

    /// <summary>
    /// 毎秒どれだけ下方向に加速するかを表す重力加速度。
    /// 負の値にすることで下向き（Y マイナス方向）に引っ張られる。
    /// Inspector から調整可能。
    /// </summary>
    public float gravity = -9.8f;

    /// <summary>
    /// 上下の速度に応じて鳥を傾ける量（度）の係数。
    /// 値が大きいほど傾きが急になる。Inspector から調整可能。
    /// </summary>
    public float tilt = 5f;

    /// <summary>
    /// 現在フレームのプレイヤーの移動方向・速度を表すベクトル。
    /// X は使用せず、Y だけで上昇・落下を管理する。
    /// </summary>
    private Vector3 direction;

    #endregion

    #region Unity コールバック

    /// <summary>
    /// オブジェクト生成時に一度だけ呼ばれる初期化メソッド。
    /// アニメーション再生に必要な SpriteRenderer を取得する。
    /// </summary>
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    /// <summary>
    /// 最初のフレームの直前に一度だけ呼ばれる。
    /// InvokeRepeating でスプライトアニメーションを一定間隔でループ再生する。
    /// 第 2 引数：開始までの待機時間（秒）、第 3 引数：繰り返し間隔（秒）。
    /// </summary>
    private void Start()
    {
        InvokeRepeating(nameof(AnimateSprite), 0.15f, 0.15f);
    }

    /// <summary>
    /// 毎フレーム呼ばれる更新メソッド。
    /// 入力の受け取り・重力の適用・位置の更新・傾き補正をここで行う。
    /// </summary>
    private void Update()
    {
        // ── 入力判定 ──────────────────────────────────────────────────
        // スペースキー押下 または マウス左ボタン（タップ）でジャンプする。
        // Input.GetKeyDown / GetMouseButtonDown は押した瞬間の 1 フレームだけ true になる。
        if (Input.GetKeyDown(KeyCode.Space) || Input.GetMouseButtonDown(0))
        {
            // direction.y を strength に上書きすることで、
            // 落下中でも即座に上昇方向へリセットされる（ジャンプ感の演出）。
            direction = Vector3.up * strength;

            // ジャンプ効果音を再生する。
            SoundManager.Instance.PlaySE(SoundManager.Instance.jumpClip);
        }

        // ── 重力の適用 ────────────────────────────────────────────────
        // 毎フレーム gravity（負の値）× 経過時間 を Y 速度に加算する。
        // Time.deltaTime を掛けることでフレームレートに依存しない一定の重力になる。
        direction.y += gravity * Time.deltaTime;

        // ── 位置の更新 ────────────────────────────────────────────────
        // direction（速度）× Time.deltaTime を現在位置に加算して動かす。
        transform.position += direction * Time.deltaTime;

        // ── 傾き補正 ──────────────────────────────────────────────────
        // 上昇中（direction.y が正）は上を向き、落下中（負）は下を向くように
        // Z 軸回転を direction.y × tilt の角度にセットする。
        Vector3 rotation = transform.eulerAngles;
        rotation.z = direction.y * tilt;
        transform.eulerAngles = rotation;
    }

    #endregion

    #region スプライトアニメーション

    /// <summary>
    /// InvokeRepeating から 0.15 秒ごとに呼ばれ、スプライトを次のコマに切り替える。
    /// 配列の末尾まで来たら先頭に戻してループさせる。
    /// </summary>
    private void AnimateSprite()
    {
        // インデックスを 1 進め、配列の長さを超えたら 0 に戻す（ループ）
        spriteIndex++;
        if (spriteIndex >= sprites.Length) spriteIndex = 0;

        // 現在インデックスのスプライトを SpriteRenderer に反映する
        spriteRenderer.sprite = sprites[spriteIndex];
    }

    #endregion

    #region 衝突処理

    /// <summary>
    /// 2D トリガーコライダーに何かが入ったときに Unity が自動で呼ぶメソッド。
    /// タグで相手を判別し、障害物ならゲームオーバー・スコアゾーンなら加点する。
    /// </summary>
    /// <param name="other">衝突してきた相手のコライダー</param>
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Obstacle"))
        {
            // BGM を止めて、ヒット音とゲームオーバー音を重ねて再生する
            SoundManager.Instance.StopBGM();
            SoundManager.Instance.PlaySE(SoundManager.Instance.hitClip);
            SoundManager.Instance.PlaySE(SoundManager.Instance.gameOverClip);

            // GameManager にゲームオーバー処理を依頼する
            GameManager.Instance.GameOver();
        }
        else if (other.CompareTag("Scoring"))
        {
            // スコアゾーンを通過したのでスコアを 1 加算する
            GameManager.Instance.IncreaseScore();
        }
    }

    #endregion

    #region ゲーム開始処理

    /// <summary>
    /// ゲーム開始（リスタート）時に GameManager から呼ばれるメソッド。
    /// プレイヤーの位置・速度をリセットし、スクリプトを有効化して操作できる状態に戻す。
    /// </summary>
    public void StartGame()
    {
        // Y 座標だけ 0（中央）に戻す。X・Z はそのまま維持する。
        Vector3 position = transform.position;
        position.y = 0f;
        transform.position = position;

        // 前の移動量をクリアして静止状態にする
        direction = Vector3.zero;

        // enabled = true にするとこのスクリプトの Update() が再び呼ばれるようになる。
        // ゲームオーバー時に enabled = false で操作を止めていた場合の復帰処理。
        enabled = true;
    }

    #endregion
}