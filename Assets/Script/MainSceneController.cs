using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System;

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

    [SerializeField]
    private GameObject pauseResumeButton;

    [SerializeField]
    private GameObject pauseTitleBackButton;

    // スコアテキスト
    [SerializeField]
    private ScoreView scoreView;

    // 一定時間経過毎に加算される点数
    [SerializeField]
    private float timeScoreValue = 1;

    // スコアを加算する間隔
    [SerializeField]
    private float timeScoreInterval = 1;

    // リザルトスコアUI
    [SerializeField]
    private ResultScoreView resultScoreView;

    [SerializeField]
    private GameObject resultRetryButton;

    [SerializeField]
    private GameObject resultTitleBackButton;

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
        UnityEngine.Random.InitState((int)DateTime.Now.Ticks);
    }

    private void Start()
    {
        stateMachine.ChangeState(MAIN_SCENE_STATE.AWAKE);
        AudioManager.Instance.PlayBGM(Define.BGM.MAIN);
    }

    private void Update()
    {
        stateMachine.Update();

        if (InputPauseButton())
        {
            if (stateMachine.IsCurrentState(MAIN_SCENE_STATE.PAUSE))
            {
                Resume();
            }
            else
            {
                if (stateMachine.IsCurrentState(MAIN_SCENE_STATE.STANBY) || stateMachine.IsCurrentState(MAIN_SCENE_STATE.RUNNING))
                {
                    Pause();
                }
            }
        }
    }

    private bool InputPauseButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            return true;
        }
        return false;
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
            if (currentTime >= owner.timeScoreInterval)
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
        private int selectButtonIdx = 0;

        public ResultState(MainSceneController owner) : base(owner)
        {
            owner.resultScoreView.SetCompleteHandle(OnSelected);
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            // コンフィグボタン非表示
            owner.configButton.enabled = false;

            // スコア表示
            owner.resultScoreView.gameObject.SetActive(true);
            owner.resultScoreView.DisplayScore();

            selectButtonIdx = 0;
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            if (Define.InputLeftButton())
            {
                selectButtonIdx = 0;
                OnSelected();
            }
            else if (Define.InputRightButton())
            {
                selectButtonIdx = 1;
                OnSelected();
            }
            else if (Define.InputEnterButton())
            {
                OnDecided();
            }
        }

        /// <summary>
        /// 状態終了時
        /// </summary>
        public override void Exit()
        {
            // コンフィグボタン非表示
            owner.configButton.enabled = true;
        }

        private void OnSelected()
        {
            ButtonHighlightEvent retry = owner.resultRetryButton.GetComponent<ButtonHighlightEvent>();
            ButtonHighlightEvent back = owner.resultTitleBackButton.GetComponent<ButtonHighlightEvent>();
            switch (selectButtonIdx)
            {
                case 0:
                    back.OnPointerEnter();
                    retry.OnPointerExit();
                    break;
                case 1:
                    back.OnPointerExit();
                    retry.OnPointerEnter();
                    break;
            }
        }

        private void OnDecided()
        {
            switch (selectButtonIdx)
            {
                case 0:
                    ButtonClickEvent back = owner.resultTitleBackButton.GetComponent<ButtonClickEvent>();
                    back.OnPointerClick();
                    owner.TitleBack();
                    break;
                case 1:
                    ButtonClickEvent retry = owner.resultRetryButton.GetComponent<ButtonClickEvent>();
                    retry.OnPointerClick();
                    owner.Retry();
                    break;
            }
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
            FadeController.Instance.FadeOut(0.5f, () => owner.Delay(0.5f, () => owner.stateMachine.ChangeState(MAIN_SCENE_STATE.AWAKE)));
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
        private int selectButtonIdx = 0;

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

            selectButtonIdx = 0;

            OnSelected();
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            if (Define.InputUpButton())
            {
                selectButtonIdx = 0;
                OnSelected();
            }
            else if (Define.InputDownButton())
            {
                selectButtonIdx = 1;
                OnSelected();
            }
            else if (Define.InputEnterButton())
            {
                OnDecided();
            }
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

        private void OnSelected()
        {
            ButtonHighlightEvent resume = owner.pauseResumeButton.GetComponent<ButtonHighlightEvent>();
            ButtonHighlightEvent back = owner.pauseTitleBackButton.GetComponent<ButtonHighlightEvent>();
            switch (selectButtonIdx)
            {
                case 0:
                    resume.OnPointerEnter();
                    back.OnPointerExit();
                    break;
                case 1:
                    resume.OnPointerExit();
                    back.OnPointerEnter();
                    break;
            }
        }

        private void OnDecided()
        {
            switch (selectButtonIdx)
            {
                case 0:
                    ButtonClickEvent resume = owner.pauseResumeButton.GetComponent<ButtonClickEvent>();
                    resume.OnPointerClick();
                    owner.Resume();
                    break;
                case 1:
                    ButtonClickEvent back = owner.pauseTitleBackButton.GetComponent<ButtonClickEvent>();
                    back.OnPointerClick();
                    owner.TitleBack();
                    break;
            }
        }
    }

}
