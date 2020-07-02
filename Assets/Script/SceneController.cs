using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneController : MonoBehaviour
{
    // シーン状態定義
    public enum MAIN_SCENE_STATE
    {
        AWAKE,
        STANBY,
        RUNNING,
        GOAL,
        FAIL
    }

    // エリア制御
    [SerializeField]
    private AreaController areaController;

    // 状態制御
    public StateMachine<SceneController, MAIN_SCENE_STATE> stateMachine = new StateMachine<SceneController, MAIN_SCENE_STATE>();

    // プレイヤープレハブ
    [SerializeField]
    private GameObject playerPrefab;

    // プレイヤー制御
    private Player player;

    // 追従カメラ制御
    [SerializeField]
    private FollowCamera followCamera;

    private void Awake()
    {
        stateMachine.AddState(MAIN_SCENE_STATE.AWAKE, new AwakeState(this));
        stateMachine.AddState(MAIN_SCENE_STATE.STANBY, new StanbyState(this));
    }

    private void Start()
    {
        stateMachine.ChangeState(MAIN_SCENE_STATE.AWAKE);
    }

    /// <summary>
    /// 起動状態
    /// </summary>
    private class AwakeState : State<SceneController>
    {
        public AwakeState(SceneController owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
            // プレイヤーをスタート位置に作成
            owner.player = owner.areaController.CreatePlayer(owner.playerPrefab);

            // カメラでプレイヤーの追従開始
            owner.followCamera.SetTarget(owner.player.transform);
            owner.followCamera.EnableFollow();

            // エリア初期化
            owner.areaController.Initialize();

            owner.stateMachine.ChangeState(MAIN_SCENE_STATE.STANBY);
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
    private class StanbyState : State<SceneController>
    {
        public StanbyState(SceneController owner) : base(owner)
        {

        }

        /// <summary>
        /// 状態開始時
        /// </summary>
        public override void Enter()
        {
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

}
