using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterFSMContext 
{
    private Transform transform;
    private Animator animator;
    private NavMeshAgent agent;
    private FieldOfView fov;
    private StateMachineCoroutineController coroutineController;
    private List<Vector3Int> roomPosition;
    private MonsterAudioSource audioSource;


    public Transform Target;

    private Transform attackDetect;

    public MonsterFSMContext(Transform _transform, Animator _animator, NavMeshAgent navMeshAgent,Transform _attackDetect ,FieldOfView _fov,
        StateMachineCoroutineController _coroutineController, MonsterAudioSource _audioSource)
    {
        transform = _transform;
        animator = _animator;
        agent = navMeshAgent;
        attackDetect = _attackDetect;
        fov = _fov;
        coroutineController = _coroutineController;
        audioSource = _audioSource;
    }
    public void SetRoomPosition(List<Vector3Int> _roomPosition)
    {
        roomPosition = new List<Vector3Int>(_roomPosition);
    }
    public Transform Transform => transform;
    public Animator Animator => animator;
    public NavMeshAgent Agent => agent;
    public Transform AttackDetect => attackDetect;
    public FieldOfView Fov => fov;
    public StateMachineCoroutineController CoroutineController => coroutineController;
    public List<Vector3Int> RoomPosition => roomPosition;
    public MonsterAudioSource AudioSource => audioSource;
}
