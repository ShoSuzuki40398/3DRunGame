using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // 敵タイプ定義
    public enum ENEMY_TYPE
    {
        PAWN,
        BISHOP,
        ROOK
    }

    // 敵状態定義
    enum ENEMY_STATE
    {
        ALIVE,
        DEAD
    }

    // 敵タイプ
    [SerializeField]
    private ENEMY_TYPE type = ENEMY_TYPE.PAWN;

    // やられ時のエフェクト
    [SerializeField]
    private GameObject deadEffect;

    // アニメーター
    private Animator animator;

    // 状態制御
    private StateMachine<Enemy, ENEMY_STATE> stateMachine = new StateMachine<Enemy, ENEMY_STATE>();

    // 巡回先
    private List<Transform> navPoint = new List<Transform>();

    // 移動速度
    private float moveSpeed = 3.0f;

    // プレイヤーの横移動方向
    private Player.SHIFT_DIR playerShiftDir = Player.SHIFT_DIR.NONE;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        stateMachine.AddState(ENEMY_STATE.ALIVE, new AliveState(this));
        stateMachine.AddState(ENEMY_STATE.DEAD, new DeadState(this));
        stateMachine.ChangeState(ENEMY_STATE.ALIVE);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.Update();
    }

    /// <summary>
    /// 巡回先設定
    /// </summary>
    public void SetNavPoint(List<Transform> points)
    {
        navPoint = points;
    }

    /// <summary>
    /// やられた
    /// </summary>
    public void Dead(Player.SHIFT_DIR dir)
    {
        playerShiftDir = dir;
        stateMachine.ChangeState(ENEMY_STATE.DEAD);
    }

    /// <summary>
    /// 敵種類取得
    /// </summary>
    /// <returns></returns>
    public ENEMY_TYPE GetEnemyType()
    {
        return type;
    }

    //----------------------------------------------------------------------------------
    //  ↓状態クラス↓
    //----------------------------------------------------------------------------------

    /// <summary>
    /// 生存状態
    /// </summary>
    private class AliveState : State<Enemy>
    {
        // 巡回地点まで近づく距離
        private float remainingDistance = 0.1f;

        // 次の移動先
        private Vector3 destination;

        // 現在の巡回地点インデックス
        private int destPointIndex = 0;

        public AliveState(Enemy owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始
        /// </summary>
        public override void Enter()
        {
            destination = GetNextPoint();
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            // 移動先が初期位置しかない場合は移動しない
            if (owner.navPoint.Count <= 1)
            {
                return;
            }

            float step = owner.moveSpeed * Time.unscaledDeltaTime;
            owner.transform.position = Vector3.MoveTowards(owner.transform.position, destination, step);

            if (Vector3.Distance(owner.transform.position, destination) < remainingDistance)
            {
                destination = GetNextPoint();
            }
        }

        /// <summary>
        /// 状態終了
        /// </summary>
        public override void Exit()
        {
        }

        /// <summary>
        /// 次の移動先を取得
        /// </summary>
        /// <returns></returns>
        private Vector3 GetNextPoint()
        {
            Vector3 result = owner.navPoint[destPointIndex].position;

            destPointIndex = (destPointIndex + 1) % owner.navPoint.Count;
            
            var enemyHeight = owner.GetComponent<Renderer>().bounds.size.y;
            result += new Vector3(0, result.y + 0.5f, 0);
            return result;
        }
    }

    /// <summary>
    /// やられ状態
    /// </summary>
    private class DeadState : State<Enemy>
    {
        // やられ状態のアニメーション
        private AnimatorStateInfo animInfo;

        private string animName;

        public DeadState(Enemy owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始
        /// </summary>
        public override void Enter()
        {
            // プレイヤーの横移動の方向に飛ばすアニメーションを設定
            switch (owner.playerShiftDir)
            {
                case Player.SHIFT_DIR.LEFT:
                    owner.animator.SetTrigger("DeadLeft");
                    animName = "Base Layer.EnemyDeadLeft";
                    break;
                case Player.SHIFT_DIR.RIGHT:
                    owner.animator.SetTrigger("DeadRight");
                    animName = "Base Layer.EnemyDeadRight";
                    break;
                default:
                    owner.animator.SetTrigger("DeadLeft");
                    animName = "Base Layer.EnemyDeadLeft";
                    break;
            }

            animInfo = owner.animator.GetCurrentAnimatorStateInfo(0);
            owner.transform.parent = null;
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            if (owner.animator.GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash(animName))
            {
                if (owner.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    GameObject eff = Instantiate(owner.deadEffect);
                    eff.transform.position = owner.transform.position;
                    AudioManager.Instance.PlaySE(Define.SE.ENEMY_BURST);
                    Destroy(owner.gameObject);
                }
            }
        }

        /// <summary>
        /// 状態終了
        /// </summary>
        public override void Exit()
        {
        }
    }
}
