using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Player : MonoBehaviour
{
    // プレイヤー状態定義
    public enum PLAYER_STATE
    {
        WALK_OUT,   // 走り出し
        RUN,        // 走る
        SHIFT,      // 横移動
        STOP,       // 停止
        BURST,      // 爆発
        DEAD        // やられ
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

    // 横移動にかける時間
    [SerializeField]
    private float shiftTime = 0.05f;

    // 横移動状態を保つフレーム数
    // 値が小さいと敵への攻撃タイミングがシビアになる。
    // 値が大きすぎると攻撃状態が長く続いてしまうため
    // 簡単になるので注意。
    [SerializeField]
    private int shiftFrameOffset = 10;

    // 現在走っているエリアラインのインデックス
    // 左端のラインを１として、右に行くたびに1加算した値にする
    private int currentAreaLineIdx = 1;

    // 横移動方向
    private SHIFT_DIR shiftDir = SHIFT_DIR.NONE;

    // エリア制御
    private AreaController areaController;

    // 状態制御
    private StateMachine<Player, PLAYER_STATE> stateMachine = new StateMachine<Player, PLAYER_STATE>();

    // 走行軌跡
    private TrailRenderer trail;

    // やられ時のエフェクト
    [SerializeField]
    private GameObject deadEffectPrefab;
        
    private void Awake()
    {
        trail = GetComponent<TrailRenderer>();
        stateMachine.AddState(PLAYER_STATE.WALK_OUT, new WalkOutState(this));
        stateMachine.AddState(PLAYER_STATE.RUN, new RunState(this));
        stateMachine.AddState(PLAYER_STATE.SHIFT, new ShiftState(this));
        stateMachine.AddState(PLAYER_STATE.STOP, new StopState(this));
        stateMachine.AddState(PLAYER_STATE.BURST, new BurstState(this));
        stateMachine.AddState(PLAYER_STATE.DEAD, new DeadState(this));
    }


    // Update is called once per frame
    void Update()
    {
        if(Pauser.Instance.GetState() == Pauser.STATE.RESUME)
        {
            stateMachine.Update();
        }
    }

        /// <summary>
    /// 前進
    /// </summary>
    private void Front()
    {
        transform.position += new Vector3(0, 0, speed * Time.deltaTime);
    }

    /// <summary>
    /// やられた
    /// </summary>
    private void Dead()
    {
        stateMachine.ChangeState(PLAYER_STATE.BURST);
        //stateMachine.ChangeState(PLAYER_STATE.DEAD);
    }

    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            // スタートエリア出口通過
            case Define.TagStartAreaExit:
                stateMachine.ChangeState(PLAYER_STATE.RUN);
                break;

            // エリア入り口通過
            case Define.TagAreaEntrance:
                areaController.RemoveLeadArea();
                areaController.AddArea();
                areaController.CountUpRunningArea();

                break;
            case Define.TagEnemy:
                if(stateMachine.IsCurrentState(PLAYER_STATE.SHIFT))
                {
                    Enemy enemy = other.gameObject.GetComponent<Enemy>();
                    enemy.Dead();
                    ScorePool.Instance.AddDefeatedEnemy(enemy.GetEnemyType());
                    AudioManager.Instance.PlaySE(Define.SE.PLAYER_ATTACK_HIT);
                }
                else if (stateMachine.IsCurrentState(PLAYER_STATE.RUN))
                {
                    Dead();
                }
                
                break;
        }
    }

    /// <summary>
    /// 横移動終了時コールバック
    /// </summary>
    private void ShiftEnd()
    {
        this.Delay(shiftFrameOffset, () => stateMachine.ChangeState(PLAYER_STATE.RUN));
    }

    /// <summary>
    /// スコア計算
    /// </summary>
    /// <returns></returns>
    private float CalcScore(Enemy.ENEMY_TYPE enemyType)
    {
        float result = 0;
        switch (enemyType)
        {
            case Enemy.ENEMY_TYPE.PAWN:result = Define.pawnScore;  break;
            case Enemy.ENEMY_TYPE.ROOK: result = Define.rookScore; break;
            case Enemy.ENEMY_TYPE.BISHOP: result = Define.bishopScore; break;
        }

        return result;
    }

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        stateMachine.ChangeState(PLAYER_STATE.STOP);
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
        currentAreaLineIdx = index;
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

    /// <summary>
    /// やられたか
    /// </summary>
    /// <returns></returns>
    public bool IsDead()
    {
        return stateMachine.IsCurrentState(PLAYER_STATE.DEAD);
    }

    /// <summary>
    /// 軌跡の色を設定
    /// </summary>
    /// <param name="color"></param>
    public void SetTrailColor(Color color)
    {
        trail.material.color = color;
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
            MyDebug.Log("Player WalkOutState Enter");
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
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            // 前進
            owner.Front();

            // UIクリック判定
            if (EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }

            // 横移動方向の決定
            if (Input.GetMouseButtonDown(Define.leftButton))
            {
                owner.shiftDir = SHIFT_DIR.LEFT;
            }
            else if (Input.GetMouseButtonDown(Define.rightButton))
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

                    if (owner.currentAreaLineIdx > 1)
                    {
                        result = true;
                    }
                    break;

                case SHIFT_DIR.RIGHT:
                    if (owner.currentAreaLineIdx < owner.areaController.GetMaxAreaWidth())
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
            Shift(owner.shiftDir);
            AudioManager.Instance.PlaySE(Define.SE.PLAYER_SHIFT);
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
            moveHash.Add("time", owner.shiftTime);
            moveHash.Add("easeType", iTween.EaseType.easeInBack);
            moveHash.Add("oncomplete", "ShiftEnd");
            moveHash.Add("oncompletetarget", owner.gameObject);

            switch (dir)
            {
                case SHIFT_DIR.LEFT:

                    if (owner.currentAreaLineIdx > 1)
                    {
                        owner.currentAreaLineIdx -= 1;
                        moveHash.Add("x", -owner.shiftValue);
                    }
                    break;
                case SHIFT_DIR.RIGHT:
                    if (owner.currentAreaLineIdx < owner.areaController.GetMaxAreaWidth())
                    {
                        owner.currentAreaLineIdx += 1;
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


    /// <summary>
    /// 爆発状態
    /// </summary>
    private class BurstState : State<Player>
    {
        GameObject effectBurst;

        // やられ状態に繊維するまでの時間
        private const float stateChangeTime = 1.0f;
        private float currentStateChangeTime = 0.0f;

        public BurstState(Player owner) : base(owner)
        {

        }

        /// <summary>
        /// 状態開始
        /// </summary>
        public override void Enter()
        {
            currentStateChangeTime = 0.0f;

            // やられエフェクト作成
            effectBurst = Instantiate(owner.deadEffectPrefab);
            effectBurst.transform.position = owner.transform.position;
            effectBurst.transform.parent = owner.transform;

            owner.GetComponent<Renderer>().enabled = false;

            AudioManager.Instance.PlaySE(Define.SE.PLAYER_BURST);
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            currentStateChangeTime += Time.deltaTime;
            if(currentStateChangeTime >= stateChangeTime)
            {
                Destroy(effectBurst);
                owner.stateMachine.ChangeState(PLAYER_STATE.DEAD);
            }
        }

        /// <summary>
        /// 状態終了
        /// </summary>
        public override void Exit()
        {
        }
    }

    /// <summary>
    /// やられ状態
    /// </summary>
    private class DeadState : State<Player>
    {
        public DeadState(Player owner) : base(owner)
        {

        }

        /// <summary>
        /// 状態開始
        /// </summary>
        public override void Enter()
        {
            var components = owner.gameObject.GetComponents<MonoBehaviour>();
            foreach(var comp in components)
            {
                comp.enabled = false;
            }
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
