using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChaseTargetState : MonsterState
{
    const string Walk = "root|Anim_monster_scavenger_walk";
    const float AnimationSpeed = 2;

    private Vector3 lastSeenTargetPosition;
    private float footstepTimer;

    public ChaseTargetState(MonsterFSMContext _context) : base(_context)
    {
    }

    public override void EnterState()
    {
        footstepTimer = 0;
        context.Animator.speed = AnimationSpeed;
        context.Animator.Play(Walk);
        context.Agent.speed = 10;
        context.Agent.acceleration = 13;
        context.CoroutineController.StartStateCoroutine(AttackRangeDetect());
        context.CoroutineController.StartStateCoroutine(FOVDetect());
        context.AudioSource.PlayRoarSound();
    }

    public override void StateUpdateWork()
    {
        if (context.Target!=null)
        {
            FaceTo();
        }
        if (context.Agent.velocity != Vector3.zero)
        {
            SprintFootstep();
        }
    }

    public override void ExitState()
    {
        context.Animator.speed = 1;//back to original animation speed;
        context.Agent.speed = 5;
        context.Agent.acceleration = 6;
        context.Agent.SetDestination(context.Transform.position);
        context.CoroutineController.StopAllCoroutines();
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
    private IEnumerator FOVDetect()
    {
        WaitForSeconds wait = new WaitForSeconds(0.1f);
        while (true)
        {
            if (context.Fov.IsTargetInRange())
            {
                context.Target = context.Fov.Target;
                lastSeenTargetPosition = context.Target.position;
                context.Agent.SetDestination(context.Target.position);
            }
            else
            {
                if (Vector3.Distance(context.Transform.position, lastSeenTargetPosition) <1)
                {
                    context.Target = null;
                    ChangeStateTo(Monster.MonsterState.Idle);
                    break;
                }
            }
            yield return wait;
        }
    }
    private void SprintFootstep()
    {
        footstepTimer -= Time.deltaTime;
        if (footstepTimer <= 0)
        {
            context.AudioSource.PlaySprintFootstep();
            footstepTimer = 0.5f;
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
    private void FaceTo()
    {
        float rotateSpeed = 10;
        Vector3 relativePos = context.Target.position - context.Transform.position;
        Quaternion rotation = Quaternion.LookRotation(relativePos);
        context.Transform.rotation = Quaternion.Lerp(context.Transform.rotation, rotation, Time.deltaTime * rotateSpeed);
    }
}
