using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("UI関連")]
    public Text scoreText;        // スコア表示用
    public Text titleText;        // タイトルテキスト
    public GameObject playButton; // プレイ開始ボタン
    public GameObject gameOver;   // ゲームオーバー表示

    [Header("プレイヤー関連")]
    public Player player;         // プレイヤースクリプト参照

    private int score;            // 現在のスコア

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        Application.targetFrameRate = 60;

        // タイトル画面表示
        Title();
    }

    // プレイ開始処理
    public void Play()
    {
        // 再スタート時にゲームオーバー音を止める
        SoundManager.Instance.StopGameOverSE();

        // クリック音再生
        SoundManager.Instance.PlaySE(SoundManager.Instance.clickClip);

        // UI非表示
        titleText.gameObject.SetActive(false);
        playButton.SetActive(false);
        gameOver.SetActive(false);

        // スコア初期化
        score = 0;
        scoreText.text = score.ToString();

        Time.timeScale = 1f;

        // プレイヤー開始
        player.StartGame();

        // 画面上のパイプを全削除
        Pipes[] pipes = FindObjectsOfType<Pipes>();
        for (int i = 0; pipes != null && i < pipes.Length; i++)
        {
            Destroy(pipes[i].gameObject);
        }
    }

    // ゲーム一時停止
    public void Pause()
    {
        Time.timeScale = 0f;
        player.enabled = false;
    }

    // タイトル表示
    public void Title()
    {
        titleText.gameObject.SetActive(true);
        playButton.SetActive(true);
        gameOver.SetActive(false);

        player.enabled = false;
        Time.timeScale = 0f;
    }

    // ゲームオーバー処理
    public void GameOver()
    {
        gameOver.SetActive(true);
        playButton.SetActive(true);

        // ゲームオーバー音を再生
        SoundManager.Instance.PlayGameOverSE();

        Pause();
    }

    // スコア加算
    public void IncreaseScore()
    {
        score++;
        scoreText.text = score.ToString();
    }
}