using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable
{
    IEnumerator CheckHit(bool x);

    IEnumerator TakeDamage();

    IEnumerator Die();
}
