using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PullablePlatform : MonoBehaviour
{
    public Transform startPosition;
    public Transform finalLocation;

    [SerializeField]
    private float moveSpeed = 0;

    private bool move;

    // Start is called before the first frame update
    void Start()
    {
        move = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (move && Vector3.Distance(transform.position, finalLocation.position) > 0.1f)
        {
            Move();
        }
        else if (!move)
        {
            ResetPosition();
        }
    }

    public void MovePlatform()
    {
        move = true;
        StartCoroutine(ResetTimer());
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, finalLocation.transform.position, moveSpeed * Time.deltaTime);
    }

    private void ResetPosition()
    {
        transform.position = Vector3.MoveTowards(transform.position, startPosition.transform.position, moveSpeed * Time.deltaTime);
    }

    IEnumerator ResetTimer()
    {
        yield return new WaitForSeconds(3);
        move = false;
    }

}
