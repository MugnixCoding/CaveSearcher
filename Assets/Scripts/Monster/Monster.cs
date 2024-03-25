
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Monster : MonoBehaviour
{
    public enum MonsterState
    {
        Idle,
        Patrol,
        Chase,
        Attack,
    }

    [SerializeField]
    private Animator animator;
    /*
    [SerializeField]
    private AttackRadius attackRadius;
    */
    [SerializeField]
    private Transform attackDetect;

    private FieldOfView fov;
    private NavMeshAgent agent;
    private FiniteStateMachine<MonsterState> monsterFSM;
    private MonsterFSMContext context;
    private StateMachineCoroutineController coroutineController;
    [SerializeField]
    private MonsterAudioSource audioSource;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        fov = GetComponent<FieldOfView>();
        audioSource.audioSource = GetComponent<AudioSource>();
        coroutineController = GetComponent<StateMachineCoroutineController>();
        monsterFSM = new FiniteStateMachine<MonsterState>();
        context = new MonsterFSMContext(transform, animator, agent, attackDetect, fov, coroutineController,audioSource);
    }

    private void Start()
    {
        InitFSM();
    }
    private void Update()
    {
        monsterFSM.UpdateWork();
    }
    private void InitFSM()
    {
        monsterFSM.AddState(MonsterState.Idle, new IdleState(context));
        monsterFSM.AddState(MonsterState.Patrol, new PatrolState(context));
        monsterFSM.AddState(MonsterState.Chase, new ChaseTargetState(context));
        monsterFSM.AddState(MonsterState.Attack, new AttackTargetState(context));
        monsterFSM.TransitionToState(MonsterState.Idle);
    }
    public void SetRoomPosition(List<Vector3Int> _roomPosition)
    {
        if (context !=null)
        {
            context.SetRoomPosition(_roomPosition);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(attackDetect.position, 3);
    }
}
