using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PassKillable : MonoBehaviour, IKillable
{
    [SerializeField]
    GameObject killableObj;
    IKillable killable;

    private void Start()
    {
        killable = killableObj.GetComponent<IKillable>();
    }

    public IEnumerator CheckHit(bool isGroundPound)
    {
        yield return 0f;
        StartCoroutine(killable.CheckHit(isGroundPound));
    }

    public IEnumerator TakeDamage()
    {
        yield return 0f;
        StartCoroutine(killable.TakeDamage());
    }

    public IEnumerator Die()
    {
        yield return 0f;
        StartCoroutine(killable.Die());
    }
}
