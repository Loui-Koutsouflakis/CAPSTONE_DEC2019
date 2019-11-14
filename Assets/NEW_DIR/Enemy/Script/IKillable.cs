using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillable
{
    IEnumerator CheckHit();

    IEnumerator TakeDamage();

    IEnumerator Die();
}
