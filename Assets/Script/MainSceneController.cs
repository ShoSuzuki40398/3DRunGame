using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;

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
        RESULT,
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
    
    // 一定時間経過毎に加算される点数
    [SerializeField]
    private float timeScoreValue = 1;

    // リザルトスコアUI
    [SerializeField]
    private ResultScoreView resultScoreView;

    // ポーズ前の状態
    private MAIN_SCENE_STATE latestState = MAIN_SCENE_STATE.NONE;

    [SerializeField]
    private GameData gameData;

    private void Awake()
    {
        stateMachine.AddState(MAIN_SCENE_STATE.AWAKE, new AwakeState(this));
        stateMachine.AddState(MAIN_SCENE_STATE.STANBY, new StanbyState(this));
        stateMachine.AddState(MAIN_SCENE_STATE.RUNNING, new RunningState(this));
        stateMachine.AddState(MAIN_SCENE_STATE.FAIL, new FailState(this));
        stateMachine.AddState(MAIN_SCENE_STATE.RESULT, new ResultState(this));
        stateMachine.AddState(MAIN_SCENE_STATE.RESET, new ResetState(this));
        stateMachine.AddState(MAIN_SCENE_STATE.PAUSE, new PauseState(this));

        Pauser.Instance.Resume();
        configButton.gameObject.SetActive(true);
        pauseUI.SetActive(false);
        resultScoreView.gameObject.SetActive(false);
    }

    private void Start()
    {
        stateMachine.ChangeState(MAIN_SCENE_STATE.AWAKE);
        AudioManager.Instance.PlayBGM(Define.BGM.MAIN);
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

        FadeController.Instance.FadeOut(0.5f, () => { Pauser.Instance.Resume(); SceneManager.LoadScene(Define.GetSceneName(Define.SCENE_NAME.TITLE)); }
            );
    }

    /// <summary>
    /// リトライ
    /// </summary>
    public void Retry()
    {
        stateMachine.ChangeState(MAIN_SCENE_STATE.RESET);
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
            owner.player.Initialize(owner.gameData.playerSkinName);

            // カメラでプレイヤーの追従開始
            owner.followCamera.SetTarget(owner.player.transform);
            owner.followCamera.EnableFollow();

            // エリア初期化
            owner.areaController.Initialize();

            // スコア初期化
            //owner.score.Reset();
            ScorePool.Instance.ResetScore();
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

        private Score score;

        public RunningState(MainSceneController owner) : base(owner)
        {

        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            score = ScorePool.Instance.score;
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
                score.Add(owner.timeScoreValue);
            }

            // スコア更新
            owner.scoreView.UpdateScore(score.GetScore());

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
            owner.stateMachine.ChangeState(MAIN_SCENE_STATE.RESULT);
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
    /// 最終スコア表示状態
    /// </summary>
    private class ResultState : State<MainSceneController>
    {
        public ResultState(MainSceneController owner) : base(owner)
        {

        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            // コンフィグボタン非表示
            owner.configButton.enabled = false;

            // ポーズ
            //Pauser.Instance.Pause();

            // スコア表示
            owner.resultScoreView.gameObject.SetActive(true);
            owner.resultScoreView.DisplayScore();
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

            // ポーズ解除
            //Pauser.Instance.Resume();
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

            owner.resultScoreView.gameObject.SetActive(false);

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
