using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainSceneController : MonoBehaviour
{
    // シーン状態定義
    public enum MAIN_SCENE_STATE
    {
        NONE,
        AWAKE,
        STANBY,
        RUNNING,
        GOAL,
        FAIL,
        RESET,
        PAUSE
    }

    // エリア制御
    [SerializeField]
    private AreaController areaController;

    // 状態制御
    public StateMachine<MainSceneController, MAIN_SCENE_STATE> stateMachine = new StateMachine<MainSceneController, MAIN_SCENE_STATE>();

    // プレイヤープレハブ
    [SerializeField]
    private GameObject playerPrefab;

    // プレイヤー制御
    private Player player;

    // 追従カメラ制御
    [SerializeField]
    private FollowCamera followCamera;

    // コンフィグボタン
    [SerializeField]
    private Button configButton;

    // ポーズメニュー
    [SerializeField]
    private GameObject pauseUI;

    // スコアテキスト
    [SerializeField]
    private ScoreView scoreView;

    // スコア
    private Score score = new Score();

    // 一定時間経過毎に加算される点数
    [SerializeField]
    private float timeScoreValue = 1;

    // ポーズ前の状態
    private MAIN_SCENE_STATE latestState = MAIN_SCENE_STATE.NONE;

    private void Awake()
    {
        stateMachine.AddState(MAIN_SCENE_STATE.AWAKE, new AwakeState(this));
        stateMachine.AddState(MAIN_SCENE_STATE.STANBY, new StanbyState(this));
        stateMachine.AddState(MAIN_SCENE_STATE.RUNNING, new RunningState(this));
        stateMachine.AddState(MAIN_SCENE_STATE.FAIL, new FailState(this));
        stateMachine.AddState(MAIN_SCENE_STATE.RESET, new ResetState(this));
        stateMachine.AddState(MAIN_SCENE_STATE.PAUSE, new PauseState(this));

        Pauser.Instance.Resume();
        configButton.gameObject.SetActive(true);
        pauseUI.SetActive(false);
    }

    private void Start()
    {
        stateMachine.ChangeState(MAIN_SCENE_STATE.AWAKE);
    }

    private void Update()
    {
        stateMachine.Update();
    }

    /// <summary>
    /// ポーズ
    /// </summary>
    public void Pause()
    {
        latestState = stateMachine.GetCurrentStateKey();
        stateMachine.ChangeState(MAIN_SCENE_STATE.PAUSE);
    }

    /// <summary>
    /// ポーズ解除
    /// </summary>
    public void Resume()
    {
        stateMachine.ChangeState(latestState);
    }

    /// <summary>
    /// タイトルに戻る
    /// </summary>
    public void TitleBack()
    {
        FadeController.Instance.FadeOut(0.5f, () => SceneManager.LoadScene(Define.GetSceneName(Define.SCENE_NAME.TITLE)));
    }

    //----------------------------------------------------------------------------------
    //  ↓状態クラス↓
    //----------------------------------------------------------------------------------

    /// <summary>
    /// 起動状態
    /// </summary>
    private class AwakeState : State<MainSceneController>
    {
        public AwakeState(MainSceneController owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            // プレイヤーをスタート位置に作成
            owner.player = owner.areaController.CreatePlayer(owner.playerPrefab);
            owner.player.Initialize(owner.score);

            // カメラでプレイヤーの追従開始
            owner.followCamera.SetTarget(owner.player.transform);
            owner.followCamera.EnableFollow();

            // エリア初期化
            owner.areaController.Initialize();

            // スコア初期化
            owner.score.Reset();
            owner.scoreView.ResetScore();

            // フェード後に準備状態へ遷移
            FadeController.Instance.FadeIn(1.0f, () => owner.stateMachine.ChangeState(MAIN_SCENE_STATE.STANBY));
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
        }
    }

    /// <summary>
    /// 準備状態
    /// </summary>
    private class StanbyState : State<MainSceneController>
    {
        public StanbyState(MainSceneController owner) : base(owner)
        {

        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            // プレイヤーのラン開始
            if (owner.player != null)
            {
                owner.player.StartWalkOut();
            }
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            // プレイヤーがスタート位置を出たら走行状態へ移行
            if (owner.player != null && owner.player.IsRunning())
            {
                // 走行状態へ遷移
                owner.stateMachine.ChangeState(MAIN_SCENE_STATE.RUNNING);
            }
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
        }
    }

    /// <summary>
    /// 走行状態
    /// </summary>
    private class RunningState : State<MainSceneController>
    {
        // スコア算出用の経過時間
        private float currentTime = 0.0f;
        private const float intervalTime = 1.0f;
        

        public RunningState(MainSceneController owner) : base(owner)
        {

        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            MyDebug.Log("Scene RunningState Enter");
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            currentTime += Time.deltaTime;
            if(currentTime >= intervalTime)
            {
                currentTime = 0;
                owner.score.Add(owner.timeScoreValue);
            }

            // スコア更新
            owner.scoreView.UpdateScore(owner.score.GetScore());

            // プレイヤーを監視
            // やられていたら失敗状態に遷移
            if (owner.player.IsDead())
            {
                owner.stateMachine.ChangeState(MAIN_SCENE_STATE.FAIL);
            }
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
        }
    }


    /// <summary>
    /// 失敗状態
    /// </summary>
    private class FailState : State<MainSceneController>
    {
        public FailState(MainSceneController owner) : base(owner)
        {

        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            // ステージをリセットする
            owner.stateMachine.ChangeState(MAIN_SCENE_STATE.RESET);
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
        }
    }

    /// <summary>
    /// リセット状態
    /// </summary>
    private class ResetState : State<MainSceneController>
    {
        public ResetState(MainSceneController owner) : base(owner)
        {

        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            if (owner.player != null)
            {
                Destroy(owner.player.gameObject);
            }

            // エリア終了処理
            owner.areaController.Finalized();

            // フェード後ステージをリセットする
            // フェード後のコールバックの中にフェード処理が入っているとフェード出来ない不具合があるので、遅延をかけています。
            FadeController.Instance.FadeOut(0.5f, () => owner.Delay(0.5f,()=> owner.stateMachine.ChangeState(MAIN_SCENE_STATE.AWAKE)) );
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
        }
    }

    /// <summary>
    /// ポーズ状態
    /// </summary>
    private class PauseState : State<MainSceneController>
    {
        public PauseState(MainSceneController owner) : base(owner)
        {

        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            // コンフィグボタン非表示
            owner.configButton.enabled = false;
            // ポーズメニュー表示
            owner.pauseUI.SetActive(true);

            // ポーズ
            Pauser.Instance.Pause();
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
            // コンフィグボタン非表示
            owner.configButton.enabled = true;
            // ポーズメニュー表示
            owner.pauseUI.SetActive(false);

            // ポーズ解除
            Pauser.Instance.Resume();
        }
    }

}
