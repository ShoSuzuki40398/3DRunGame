using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    // プレイヤー状態定義
    public enum PLAYER_STATE
    {
        WALK_OUT,   // 走り出し
        RUN,        // 走る
        SHIFT,      // 横移動
        STOP        // 停止
    }

    // 横移動方向定義
    enum SHIFT_DIR
    {
        NONE,
        LEFT,
        RIGHT
    }

    // 直線移動スピード
    [SerializeField, Range(1,10)]
    public float speed = 1;
    
    // 横移動量
    [SerializeField]
    private float shiftValue = 1;

    // 現在走っているエリアラインのインデックス
    // 左端のラインを１として、右に行くたびに1加算した値にする
    private int currentAreaLIneIdx = 1;

    // 横移動方向
    private SHIFT_DIR shiftDir = SHIFT_DIR.NONE;

    // エリア制御
    private AreaController areaController;

    // 状態制御
    private StateMachine<Player, PLAYER_STATE> stateMachine = new StateMachine<Player, PLAYER_STATE>();
    
    private void Awake()
    {
        stateMachine.AddState(PLAYER_STATE.WALK_OUT, new WalkOutState(this));
        stateMachine.AddState(PLAYER_STATE.RUN, new RunState(this));
        stateMachine.AddState(PLAYER_STATE.SHIFT, new ShiftState(this));
        stateMachine.AddState(PLAYER_STATE.STOP, new StopState(this));
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

        /// <summary>
    /// 前進
    /// </summary>
    private void Front()
    {
        transform.position += new Vector3(0, 0, speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            // スタートエリア出口通過
            case Define.StartAreaExit:
                stateMachine.ChangeState(PLAYER_STATE.RUN);
                break;

            // エリア入り口通過
            case Define.AreaEntrance:
                areaController.RemoveLeadArea();
                areaController.AddArea();
                break;
        }
    }

    /// <summary>
    /// エリア制御設定
    /// </summary>
    public void SetAreaController(AreaController controller)
    {
        areaController = controller;
    }

    /// <summary>
    /// エリアラインのインデックスを設定
    /// </summary>
    public void SetCurrentAreaLineIndex(int index)
    {
        currentAreaLIneIdx = index;
    }

    /// <summary>
    /// 走り出し開始
    /// </summary>
    public void StartWalkOut()
    {
        stateMachine.ChangeState(PLAYER_STATE.WALK_OUT);
    }

    /// <summary>
    /// 走行中か
    /// </summary>
    /// <returns></returns>
    public bool IsRunning()
    {
        return stateMachine.IsCurrentState(PLAYER_STATE.RUN);
    }

    private void ShiftEnd()
    {
        stateMachine.ChangeState(PLAYER_STATE.RUN);
    }

    //----------------------------------------------------------------------------------
    //  ↓状態クラス↓
    //----------------------------------------------------------------------------------

    /// <summary>
    /// 走り出し状態
    /// </summary>
    private class WalkOutState : State<Player>
    {
        public WalkOutState(Player owner) : base(owner)
        {

        }

        /// <summary>
        /// 状態開始
        /// </summary>
        public override void Enter()
        {
            MyDebug.Log("Enter WalkOut");
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            // 前進
            owner.Front();
        }

        /// <summary>
        /// 状態終了
        /// </summary>
        public override void Exit()
        {
        }
    }

    /// <summary>
    /// 走り状態
    /// </summary>
    private class RunState : State<Player>
    {
        public RunState(Player owner) : base(owner)
        {

        }

        /// <summary>
        /// 状態開始
        /// </summary>
        public override void Enter()
        {
            MyDebug.Log("Enter Run");
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            // 前進
            owner.Front();

            // 横移動方向の決定
            if (Input.GetMouseButtonUp(Define.leftButton))
            {
                owner.shiftDir = SHIFT_DIR.LEFT;
            }
            else if (Input.GetMouseButtonUp(Define.rightButton))
            {
                owner.shiftDir = SHIFT_DIR.RIGHT;
            }

            // 横移動状態へ遷移
            if(CheckShift(owner.shiftDir))
            {
                owner.stateMachine.ChangeState(PLAYER_STATE.SHIFT);
            }
        }

        /// <summary>
        /// 状態終了
        /// </summary>
        public override void Exit()
        {
        }

        /// <summary>
        /// 横移動可能か確認
        /// </summary>
        /// <returns></returns>
        private bool CheckShift(SHIFT_DIR　dir)
        {
            bool result = false;

            // 左端か右端にプレイヤーがいた場合はfalseを返す
            switch (dir)
            {
                case SHIFT_DIR.NONE:
                    result = false;
                    break;
                case SHIFT_DIR.LEFT:

                    if (owner.currentAreaLIneIdx > 1)
                    {
                        result = true;
                    }
                    break;

                case SHIFT_DIR.RIGHT:
                    if (owner.currentAreaLIneIdx < owner.areaController.GetMaxAreaWidth())
                    {
                        result = true;
                    }
                    break;
            }

            return result;
        }
    }

    /// <summary>
    /// 横移動状態
    /// </summary>
    private class ShiftState : State<Player>
    {
        public ShiftState(Player owner) : base(owner)
        {

        }

        /// <summary>
        /// 状態開始
        /// </summary>
        public override void Enter()
        {
            MyDebug.Log("Enter Shift");
            Shift(owner.shiftDir);
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            // 前進
            owner.Front();
        }

        /// <summary>
        /// 状態終了
        /// </summary>
        public override void Exit()
        {
            owner.shiftDir = SHIFT_DIR.NONE;
        }

        /// <summary>
        /// 横移動
        /// エリア外に行かないように範囲制限をする
        /// </summary>
        private void Shift(SHIFT_DIR dir)
        {
            var moveHash = new Hashtable();
            moveHash.Add("time", 0.3f);
            moveHash.Add("easeType", iTween.EaseType.linear);
            moveHash.Add("oncomplete", "ShiftEnd");
            moveHash.Add("oncompletetarget", owner.gameObject);

            switch (dir)
            {
                case SHIFT_DIR.LEFT:

                    if (owner.currentAreaLIneIdx > 1)
                    {
                        owner.currentAreaLIneIdx -= 1;
                        moveHash.Add("x", -owner.shiftValue);
                    }
                    break;
                case SHIFT_DIR.RIGHT:
                    if (owner.currentAreaLIneIdx < owner.areaController.GetMaxAreaWidth())
                    {
                        owner.currentAreaLIneIdx += 1;
                        moveHash.Add("x", owner.shiftValue);
                    }
                    break;
            }

            iTween.MoveBy(owner.gameObject, moveHash);
        }
    }

    /// <summary>
    /// 停止状態
    /// </summary>
    private class StopState : State<Player>
    {
        public StopState(Player owner) : base(owner)
        {

        }

        /// <summary>
        /// 状態開始
        /// </summary>
        public override void Enter()
        {
            MyDebug.Log("Enter Stop");
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
        }

        /// <summary>
        /// 状態終了
        /// </summary>
        public override void Exit()
        {
        }
    }


}
