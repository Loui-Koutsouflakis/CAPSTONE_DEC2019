//Written By Michael Elkin 05/06/2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Mike's Scripts/Kickable_Object", 11)]
public class Kickable_Object : MonoBehaviour
{
    Rigidbody rb;

    [SerializeField, Range(1,10)]
    float forwardForce = 1.0f;
    [SerializeField, Range(1, 10)]
    float upwardForce = 1.0f;
    [SerializeField, Range(0, 5)]
    float drag = 0.0f;
    [SerializeField, Range(0, 5)]
    float frictionMod = 0.0f;
    float friction;
    MeshRenderer rend;
    float maxDistance = 100.0f; // Used To Store Max Distance for Raycasts
    RaycastHit hit; // Used to store Raycast Hit info
    bool grounded;

    bool activated; 
    [Header("Either turn off script to prevent kicking or turn off object once it stopped. Only have one active")]
    public bool canBeTurnedOff; //if we want to prevent kicking an object after the first kick
    public bool canBeDestroyed; //if we want to destroy the oject after it is kicked


    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rend = GetComponent<MeshRenderer>();
        rb.drag = drag;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(activated)
        {
            if(rb.velocity.magnitude < 0.2f)
            {
                if(canBeDestroyed)
                {
                    StartCoroutine(SpriteFlash());
                }
            }
        }

    }

    

    private void OnTriggerEnter(Collider o)
    {
        if(o.gameObject.layer == 14)
        {            
            Vector3 kickVector = (transform.position - o.transform.position - transform.position).normalized;
                rb.AddForce((kickVector * forwardForce) + (o.transform.up * upwardForce), ForceMode.Impulse);

            StartCoroutine(DestroyWait());
            if(canBeTurnedOff)
            {
                this.enabled = false;
            }
        }
    }

    void AddFriction()
    {
        friction = frictionMod * rb.mass * Physics.gravity.magnitude;
        rb.AddForce(-friction * rb.velocity.normalized);
    }

    void IsGrounded()
    {
        Ray groundedRay = new Ray(transform.position, Vector3.down);
        Physics.Raycast(groundedRay, out hit, maxDistance);
        if (hit.distance <= 1.125f)
        {
            grounded = true;
        }
        else
        {
            grounded = false;
        }
    }

    IEnumerator SpriteFlash()
    {
        for (var n = 0; n < 5; n++)
        {
            rend.enabled = true;
            yield return new WaitForSeconds(.1f);
            rend.enabled = false;
            yield return new WaitForSeconds(.1f);
            rend.enabled = true;
            if (n == 4) gameObject.SetActive(false);
        }
    }
    IEnumerator DestroyWait()
    {
        yield return new WaitForSeconds(1f);
        activated = true;
    }

}
