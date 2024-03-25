

using System.Collections;
using UnityEngine;

public class IdleState : MonsterState
{
    const string Idle = "root|Anim_monster_scavenger_Idle1";
    private float timer;
    private float IdleTime=10;
    private bool TurnLeft=false;
    private Vector3 startRotation;


    public IdleState(MonsterFSMContext _context) : base(_context)
    {
    }

    public override void EnterState()
    {
        TurnLeft = false;
        startRotation = context.Transform.eulerAngles;
        context.Animator.Play(Idle);
        context.CoroutineController.StartStateCoroutine(AttackRangeDetect());
        context.CoroutineController.StartStateCoroutine(FOVDetect());
        timer = 0;
        context.AudioSource.PlayLurkSound();
    }

    public override void ExitState()
    {
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
    public override void StateUpdateWork()
    {
        timer += Time.deltaTime;
        if (timer>IdleTime)
        {
            ChangeStateTo(Monster.MonsterState.Patrol);
        }
        if (TurnLeft)
        {
            context.Transform.rotation = Quaternion.Slerp(context.Transform.rotation, Quaternion.Euler(0, startRotation.y - 60, 0) , Time.deltaTime);
        }
        else
        {
            context.Transform.rotation = Quaternion.Slerp(context.Transform.rotation, Quaternion.Euler(0, startRotation.y + 60, 0), Time.deltaTime);
        }

        if (Mathf.Abs(Mathf.DeltaAngle(context.Transform.eulerAngles.y, startRotation.y)) > 55)
        {
            TurnLeft = !TurnLeft;
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
