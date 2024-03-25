using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item
{
    private Transform firePoint;
    public override void Use()
    {
        Shoot();
    }
    private void Shoot()
    {

    }
}
