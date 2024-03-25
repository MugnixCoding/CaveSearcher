using System.Collections;
using System.Collections.Generic;
using Unity.Services.Analytics.Internal;
using UnityEngine;
using UnityEngine.UIElements;

public class PatrolState : MonsterState
{
    const string Walk = "root|Anim_monster_scavenger_walk";
    private Vector3Int currentPatrolDest = Vector3Int.zero;
    private Vector3Int lastPatrolDest = Vector3Int.zero;
    private float footstepTimer;

    public PatrolState(MonsterFSMContext _context) : base(_context)
    {
    }

    public override void EnterState()
    {
        footstepTimer = 0;
        currentPatrolDest = context.RoomPosition[Random.Range(0, context.RoomPosition.Count - 1)];
        while (currentPatrolDest==lastPatrolDest)
        {
            currentPatrolDest = context.RoomPosition[Random.Range(0, context.RoomPosition.Count - 1)];
        }
        context.Agent.SetDestination(currentPatrolDest);
        context.Animator.Play(Walk);
        context.CoroutineController.StartStateCoroutine(AttackRangeDetect());
        context.CoroutineController.StartStateCoroutine(FOVDetect());
        context.CoroutineController.StartStateCoroutine(CheckPatrol());

    }

    public override void ExitState()
    {
        context.CoroutineController.StopAllCoroutines();
        lastPatrolDest = currentPatrolDest;
    }

    public override void OnTriggerEnter(Collider other)
    {
    }

    public override void OnTriggerExit(Collider other)
    {
    }

    public override void OnTriggerStay(Collider other)
    {
    }

    public override void StateUpdateWork()
    {
        if (context.Agent.velocity != Vector3.zero)
        {
            Quaternion desiredRotation = Quaternion.LookRotation(context.Agent.velocity.normalized);
            context.Transform.rotation = Quaternion.Slerp(context.Transform.rotation, desiredRotation, Time.deltaTime * 10f);
            WalkFootstep();
        }
    }
    private void WalkFootstep()
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer<=0)
        {
            context.AudioSource.PlayWalkFootstep();
            footstepTimer = 1;
        }
    }
    public IEnumerator CheckPatrol()
    {
        WaitForSeconds wait = new WaitForSeconds(1f);
        while (true)
        {
            if (Vector3.Distance(context.Transform.position,currentPatrolDest)<1)
            {
                ChangeStateTo(Monster.MonsterState.Idle);
                break;
            }
            yield return wait;
        }
    }
    private IEnumerator AttackRangeDetect()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while (true)
        {
            if (IsPlayerInAttackRange())
            {
                ChangeStateTo(Monster.MonsterState.Attack);
                break;
            }
            yield return wait;
        }
    }
    private IEnumerator FOVDetect()
    {
        WaitForSeconds wait = new WaitForSeconds(0.2f);
        while (true)
        {
            if (context.Fov.IsTargetInRange())
            {
                ChangeStateTo(Monster.MonsterState.Chase);
                break;
            }
            yield return wait;
        }
    }
    private bool IsPlayerInAttackRange()
    {
        Collider[] hit = Physics.OverlapSphere(context.AttackDetect.position, 3, 1 << 6);

        for (int i = 0; i < hit.Length; i++)
        {
            if (hit[i].gameObject.GetComponent<PlayerController>() != null)
            {
                context.Target = hit[i].gameObject.transform;
                return true;
            }
        }
        return false;
    }
}
