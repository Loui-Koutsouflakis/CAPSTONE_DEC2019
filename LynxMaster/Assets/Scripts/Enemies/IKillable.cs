using System.Collections;

public interface IKillable
{
    IEnumerator CheckHit();

    IEnumerator TakeDamage(int damage);

    IEnumerator Die();
}
