using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : MonoBehaviour
{
    public Transform head;
    public Transform[] body;
    public Transform returnTrig;
    
    public Animator leadAnim;
    public Animator headAnim;
    public Animator[] bodyAnims;

    private bool thrustActive;
    private bool returning;
    public bool steering;
    private int health = 2;
    public float thrustForce = 12f; //24
    private Vector3 steerDirection;
    private readonly float steerThreshold = 0.12f;
    private readonly float followDelay = 0.5f;
    private readonly float rotationLerpRate = 6f;
    private readonly float rateOfRotataion = 100f;

    private void Start()
    {
        thrustActive = true;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Pillar")
        {
            collision.collider.enabled = false;
            collision.gameObject.GetComponent<Explode>().Cue(820f, 60f, Vector3.zero);
            StartCoroutine(TakeDamage());
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(returning && other.gameObject.name == "ReturnTrig")
        {
            returning = false;
            leadAnim.enabled = true;
            leadAnim.SetTrigger("Platformable");
        }
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.B) && !steering && !returning)
        {
            CueSteer();
        }

        if (thrustActive)
        {
            head.position += head.forward * thrustForce * Time.deltaTime;

            //foreach (Transform tf in body)
            //{
            //    tf.position += tf.forward * thrustForce * Time.deltaTime;
            //}

            for (int i = 0; i < body.Length; i++)
            {
                body[i].position += body[i].forward * thrustForce * Time.deltaTime;

                if (i > 0)
                {
                    body[i].rotation = Quaternion.Lerp(body[i].rotation,
                    Quaternion.LookRotation(body[i - 1].position - body[i].position, Vector3.up),
                    rotationLerpRate * Time.deltaTime);
                }
                else
                {
                    body[i].rotation = Quaternion.Lerp(body[i].rotation,
                    Quaternion.LookRotation(head.position - body[i].position, Vector3.up),
                    rotationLerpRate * Time.deltaTime);
                }
            }
        }

        if ((Mathf.Abs(Input.GetAxis("Horizontal")) > steerThreshold || 
            Mathf.Abs(Input.GetAxis("Vertical")) > steerThreshold) && steering)
        {
            steerDirection.y = Input.GetAxis("Horizontal");
            steerDirection.x = Input.GetAxis("Vertical");
            head.rotation = Quaternion.Euler(head.rotation.eulerAngles + (steerDirection * rateOfRotataion * Time.deltaTime));
        }
    }

    public void CueSteer()
    {
        steering = true;
        leadAnim.enabled = false;
        //leadAnim.SetTrigger("Steering");
    }

    public IEnumerator TakeDamage()
    {
        steering = false;
        thrustActive = false;
        headAnim.SetTrigger("TakeDamage");

        for (int i = 0; i < bodyAnims.Length; i++)
        {
            yield return new WaitForSeconds(0.06f);
            bodyAnims[i].SetTrigger("TakeDamage");
        }

        yield return new WaitForSeconds(1f);

        thrustActive = true;
        transform.LookAt(returnTrig);
        returning = true;

        //yield return new WaitForSeconds(1f);

    }

    //public IEnumerator SteerSequence(Vector3 steer)
    //{
    //    head.Rotate(steer);
    //    yield return new WaitForSeconds(followDelay);

    //    for(int i = 0; i < body.Length - 1; i++)
    //    {
    //        body[i].Rotate(steer);
    //        yield return new WaitForSeconds(followDelay);
    //    }

    //}
}
