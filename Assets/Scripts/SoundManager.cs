using UnityEngine;

/// <summary>
/// ゲーム全体の BGM（背景音楽）と SE（効果音）を一元管理するクラス。
/// シングルトンパターンを使い、どのスクリプトからでも
///   SoundManager.Instance.PlaySE(clip);
/// のように簡単に呼び出せる。
/// </summary>
public class SoundManager : MonoBehaviour
{
    // ─────────────────────────────────────────
    // シングルトン
    // ─────────────────────────────────────────

    /// <summary>
    /// このクラスの唯一のインスタンス。
    /// 外部からは読み取り専用（private set）にして、
    /// 誤って上書きされないようにしている。
    /// </summary>
    public static SoundManager Instance { get; private set; }

    // ─────────────────────────────────────────
    // BGM 関連
    // ─────────────────────────────────────────

    [Header("BGM")]

    /// <summary>BGM を再生するための AudioSource コンポーネント。</summary>
    public AudioSource bgmSource;

    /// <summary>実際に流す BGM の音声データ（AudioClip）。</summary>
    public AudioClip bgmClip;

    // ─────────────────────────────────────────
    // SE（効果音）関連
    // ─────────────────────────────────────────

    [Header("SE")]

    /// <summary>SE を再生するための AudioSource コンポーネント。</summary>
    public AudioSource seSource;

    /// <summary>ボタンをクリックしたときの効果音。</summary>
    public AudioClip clickClip;

    /// <summary>キャラクターがジャンプしたときの効果音。</summary>
    public AudioClip jumpClip;

    /// <summary>障害物などに衝突したときの効果音。</summary>
    public AudioClip hitClip;

    /// <summary>
    /// ゲームオーバー時に再生する効果音。
    /// 途中で止める必要があるため、通常の PlaySE とは別メソッドで管理している。
    /// </summary>
    public AudioClip gameOverClip;

    // ─────────────────────────────────────────
    // Unity ライフサイクル
    // ─────────────────────────────────────────

    /// <summary>
    /// オブジェクト生成時に自動で呼ばれる初期化処理。
    /// シングルトンの初期化と、シーンをまたいでも破棄されない設定を行う。
    /// </summary>
    private void Awake()
    {
        if (Instance == null)
        {
            // まだインスタンスが存在しない場合 → 自分自身をインスタンスとして登録する
            Instance = this;

            // シーンが切り替わっても破棄されないようにする
            // （BGM をシームレスに流し続けるために必要）
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // すでに別のインスタンスが存在する場合 → 重複を防ぐため自分自身を削除する
            Destroy(gameObject);
        }
    }

    // ─────────────────────────────────────────
    // BGM 操作
    // ─────────────────────────────────────────

    /// <summary>
    /// BGM をループ再生する。
    /// bgmSource または bgmClip が未設定の場合は何もしない。
    /// </summary>
    public void PlayBGM()
    {
        if (bgmSource != null && bgmClip != null)
        {
            bgmSource.clip = bgmClip; // 再生するクリップをセット
            bgmSource.loop = true;    // ループ再生を有効にする
            bgmSource.Play();
        }
    }

    /// <summary>
    /// BGM を停止する。
    /// bgmSource が未設定の場合は何もしない。
    /// </summary>
    public void StopBGM()
    {
        if (bgmSource != null)
        {
            bgmSource.Stop();
        }
    }

    // ─────────────────────────────────────────
    // SE 操作
    // ─────────────────────────────────────────

    /// <summary>
    /// 指定した効果音を再生する（汎用版）。
    /// PlayOneShot を使うため、同じ SE を重ねて鳴らすことができる。
    /// ただし、途中で止めることはできない。
    /// </summary>
    /// <param name="clip">再生したい AudioClip</param>
    public void PlaySE(AudioClip clip)
    {
        if (seSource != null && clip != null)
        {
            seSource.PlayOneShot(clip);
        }
    }

    /// <summary>
    /// ゲームオーバー時の効果音を再生する。
    /// 途中で止められるよう、PlayOneShot ではなく seSource.clip に直接セットして Play() する。
    /// </summary>
    public void PlayGameOverSE()
    {
        if (seSource != null && gameOverClip != null)
        {
            seSource.clip = gameOverClip; // クリップをセット
            seSource.loop = false;        // ループしない（1回だけ再生）
            seSource.Play();
        }
    }

    /// <summary>
    /// ゲームオーバー効果音を停止する。
    /// ゲームオーバーSE が再生中のときだけ止める（他の SE を誤って止めないための安全チェック）。
    /// </summary>
    public void StopGameOverSE()
    {
        // seSource が再生中、かつ再生中のクリップが gameOverClip のときだけ停止する
        if (seSource != null && seSource.isPlaying && seSource.clip == gameOverClip)
        {
            seSource.Stop();
        }
    }
}