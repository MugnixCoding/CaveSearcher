using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class AttackRadius : MonoBehaviour
{
    private List<IDamageable> damageables = new List<IDamageable>();
    public int damage = 1;
    public float attackDelay = 0.5f;
    public delegate void AttackEvent(IDamageable target);
    public AttackEvent OnAttack;
    private Coroutine AttackCoroutine;

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageables.Add(damageable);

            if (AttackCoroutine==null)
            {
                AttackCoroutine = StartCoroutine(Attack());
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageables.Remove(damageable);

            if (damageables.Count==0)
            {
                StopCoroutine(Attack());
                AttackCoroutine = null;
            }
        }
    }
    private IEnumerator Attack()
    {
        WaitForSeconds wait = new WaitForSeconds(attackDelay);
        yield return wait;
        IDamageable closetDamageable=null;
        float closetDistance = float.MaxValue;
        while (damageables.Count>0)
        {
            for (int i = 0;i<damageables.Count;i++)
            {
                Transform damageableTransform = damageables[i].GetTransform();
                float distance = Vector3.Distance(transform.position, damageableTransform.position);

                if (distance<closetDistance)
                {
                    closetDistance = distance;
                    closetDamageable = damageables[i];
                }
            }
            if (closetDamageable!=null)
            {
                OnAttack?.Invoke(closetDamageable);
                closetDamageable.TakeDamage(damage);
            }
            closetDamageable = null;
            closetDistance = float.MaxValue;
            yield return wait;
            damageables.RemoveAll(DisableDamageable); // remove all object that is not active
        }
        AttackCoroutine = null;
    }
    private bool DisableDamageable(IDamageable damageable)
    {
        return damageables != null && !damageable.GetTransform().gameObject.activeSelf;
    }
}
