using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum EnemyStates { GUARD, PATROL, CHASE, DEAD }
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyController : MonoBehaviour, IEndGameObserver
{
    private EnemyStates enemyStates;
    private NavMeshAgent agent;
    private Animator anim;
    private CharacterStats characterStats;
    private Collider coll;

    [Header("Base Settings")]
    public float sightRadius;//视角范围，查找player用
    public bool isGuard;//是否是站桩敌人
    private float speed;
    private GameObject attackTarget;
    public float lookAtTime;//巡逻到目的地后停顿时间
    private float remainLookAtTime;//用来减减的
    private float lastAttackTime;//cd时间 用来记录上次攻击
    private Quaternion guardRotation;//本身得角度 朝向

    [Header("Patrol State")]
    public float patrolRange;
    private Vector3 wayPoint;//自动巡逻点
    private Vector3 guardPos;//站桩点

    //bool 配合动画
    bool isWalk; //是否行走
    bool isChase; //是否追击
    bool isFollow; //是否跟随player
    bool isDead; //是否死亡
    bool playerDead; //玩家是否死了
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        characterStats = GetComponent<CharacterStats>();
        coll = GetComponent<Collider>();

        speed = agent.speed;
        guardPos = transform.position;
        guardRotation = transform.rotation;
        remainLookAtTime = lookAtTime;
    }

    void Start()
    {
        if (isGuard)
        {
            enemyStates = EnemyStates.GUARD;
        }
        else
        {
            enemyStates = EnemyStates.PATROL;
            GetNewWayPoint();
        }
    }

    void OnEnable()
    {
        GameManager.Instance.AddObservers(this);
    }

    void OnDisable()
    {
        GameManager.Instance.RemoveObservers(this);
    }

    void Update()
    {
        //自己死了吗？
        isDead = characterStats.CurrentHealth == 0;

        if (!playerDead)
        {
            SwitchStates();
            SwitchAnimation();
            lastAttackTime -= Time.deltaTime;
        }
    }

    public bool IsDeath()
    {
        return isDead;
    }

    void SwitchAnimation()
    {
        anim.SetBool("Walk", isWalk);
        anim.SetBool("Chase", isChase);
        anim.SetBool("Follow", isFollow);
        anim.SetBool("Critical", characterStats.isCritical);
        anim.SetBool("Death", isDead);
    }

    void CheckStates()
    {
        if (isDead)
        {
            enemyStates = EnemyStates.DEAD;
        }
        // 如果发现player 切换case
        else if (FoundPlayer())
        {
            enemyStates = EnemyStates.CHASE;
        }
    }

    void SwitchStates()
    {
        CheckStates();

        switch (enemyStates)
        {
            case EnemyStates.GUARD: //站桩
                isChase = false;

                if (transform.position != guardPos)
                {
                    isWalk = true;
                    agent.isStopped = false;
                    agent.destination = guardPos;

                    if (Vector3.SqrMagnitude(guardPos-transform.position) <= agent.stoppingDistance)
                    {
                        isWalk = false;
                        transform.rotation = Quaternion.Lerp(transform.rotation,guardRotation,0.01f);
                    }
                }
                break;
            case EnemyStates.PATROL: //巡逻

                isChase = false;
                agent.speed = speed * 0.5f;

                //判断是否走到了随机巡逻点
                if (Vector3.Distance( wayPoint, transform.position ) <= agent.stoppingDistance)
                {
                    isWalk = false;
                    anim.SetBool("Search", true);
                    if (remainLookAtTime > 0)
                    {
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else
                    {
                        GetNewWayPoint();
                    }
                }
                else
                {
                    isWalk = true;
                    anim.SetBool("Search", false);
                    agent.destination = wayPoint;
                }

                break;
            case EnemyStates.CHASE: //追击
                isWalk = false;
                isChase = true;

                agent.speed = speed;

                if (!FoundPlayer())
                {
                    //拉脱回到上一个状态
                    isFollow = false;
                    anim.SetBool("Search", true);
                    if (remainLookAtTime > 0)
                    {
                        // agent.destination = transform.position;
                        remainLookAtTime -= Time.deltaTime;
                    }
                    else if(isGuard)
                    {
                        anim.SetBool("Search", false);
                        remainLookAtTime = lookAtTime;
                        enemyStates = EnemyStates.GUARD;
                    }
                    else
                    {
                        anim.SetBool("Search", false);
                        enemyStates = EnemyStates.PATROL;
                        GetNewWayPoint();
                    }
                }
                else
                {
                    //追Player
                    isFollow = true;
                    agent.isStopped = false;
                    anim.SetBool("Search", false);
                    agent.destination = attackTarget.transform.position;
                }

                //在攻击范围内则攻击
                if (TargetInAttackRange() || TargetInSkillRange())
                {
                    isFollow = false;
                    agent.isStopped = true;
                    anim.SetBool("Search", false);

                    if (lastAttackTime < 0)
                    {
                        lastAttackTime = characterStats.CoolDown;

                        //暴击判断
                        characterStats.isCritical = Random.value <= characterStats.CriticalChance;
                        //执行攻击
                        Attack();
                    }
                }


                break;
            case EnemyStates.DEAD: //死亡
                coll.enabled = false;
                agent.enabled = false;
                Destroy(gameObject, 2f);
                break;
        }
    }

    void Attack()
    {
        transform.LookAt(attackTarget.transform);
        if (TargetInAttackRange())
        {
            //近身攻击
            anim.SetTrigger("Attack");
        }
        if (TargetInSkillRange())
        {
            //技能或者远战攻击距离
            anim.SetTrigger("Skill");
        }
    }

    bool TargetInAttackRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position,transform.position) <= characterStats.AttackRange;
        return false;
    }

    bool TargetInSkillRange()
    {
        if (attackTarget != null)
            return Vector3.Distance(attackTarget.transform.position,transform.position) <= characterStats.SkillRange;
        return false;
    }

    bool FoundPlayer()
    {
        var colliders = Physics.OverlapSphere(transform.position,sightRadius);

        foreach (var target in colliders)
        {
            if (target.CompareTag("Player"))
            {
                attackTarget = target.gameObject;
                return true;
            }
        }

        attackTarget = null;
        return false;
    }

    void GetNewWayPoint()
    {
        remainLookAtTime = lookAtTime;

        float randomX = Random.Range(-patrolRange,patrolRange);
        float randomZ = Random.Range(-patrolRange,patrolRange);

        Vector3 randomPoint = new Vector3( guardPos.x + randomX, transform.position.y, guardPos.z + randomZ);

        NavMeshHit hit;
        wayPoint = NavMesh.SamplePosition(randomPoint, out hit, patrolRange, 1) ? hit.position : guardPos;
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position,sightRadius);
    }

    //Animation Event
    void Hit()
    {
        if (attackTarget != null)
        {
            var targetStats = attackTarget.GetComponent<CharacterStats>();
            characterStats.TakeDamage(characterStats,targetStats);
        }
    }

    public void EndNotify()
    {
        //获胜动画
        //停止所有移动
        //停止Agent
        anim.SetBool("Win", true);
        isChase = false;
        isWalk = false;
        attackTarget = null;
        playerDead = true;
    }
}
