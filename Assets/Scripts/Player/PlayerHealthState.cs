using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthState : MonoBehaviour, IDamageable
{
    [SerializeField]
    private int health = 5;


    public event EventHandler<DamageEventArgs> OnTakeDamage;
    public event EventHandler OnDead;
    public Transform GetTransform()
    {
        return transform;
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health<=0)
        {
            Debug.Log("player die");
            OnDead?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            OnTakeDamage?.Invoke(this, new DamageEventArgs(damage));
        }
    }
    public int GetPlayerHealth()
    {
        return health;
    }
}
public class DamageEventArgs : EventArgs
{
    public int Damage { get; }

    public DamageEventArgs(int damage)
    {
        Damage = damage;
    }
}
