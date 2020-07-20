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
    /// やられた
    /// </summary>
    public void Dead()
    {
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
        public AliveState(Enemy owner) : base(owner)
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
    /// やられ状態
    /// </summary>
    private class DeadState : State<Enemy>
    {
        // やられ状態のアニメーション
        private AnimatorStateInfo animInfo;

        public DeadState(Enemy owner) : base(owner)
        {
        }

        /// <summary>
        /// 状態開始
        /// </summary>
        public override void Enter()
        {
            owner.animator.SetTrigger("Dead");
            animInfo = owner.animator.GetCurrentAnimatorStateInfo(0);
        }

        /// <summary>
        /// 状態更新
        /// </summary>
        public override void Execute()
        {
            if(owner.animator.GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Base Layer.EnemyDead"))
            {
                if (owner.animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    MyDebug.Log(owner.animator.GetCurrentAnimatorStateInfo(0).nameHash == Animator.StringToHash("Base Layer.EnemyDead"));
                    GameObject eff = Instantiate(owner.deadEffect);
                    eff.transform.position = owner.transform.position;
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
