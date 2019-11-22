using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimateOnCollision : MonoBehaviour
{
    public Animator animator;
    public string triggerName;
    public bool useTrigger;
    public bool usePlayerLayer;


    [Header("Hidden Pickups")]
    [SerializeField]
    private bool hasPickups;
    [SerializeField]
    private GameObject[] hiddenPickups;
    private Animator[] pickupAnims;
    const string showPickupsAnimName = "ShowPickups";

    const string kbodyName = "KBody";

    private void Awake()
    {
        if(hasPickups)
        {
            foreach(GameObject go in hiddenPickups)
            {
                go.SetActive(false);
            }

            pickupAnims = new Animator[hiddenPickups.Length];

            for(int i = 0; i < hiddenPickups.Length; i++)
            {
                pickupAnims[i] = hiddenPickups[i].GetComponent<Animator>();
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!useTrigger)
        {
            if (collision.gameObject.name == kbodyName)
            {
                animator.SetTrigger(triggerName);
            }
            else if(usePlayerLayer && collision.gameObject.layer == 14)
            {
                animator.SetTrigger(triggerName);
            }

            CheckPickups();
        
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (useTrigger)
        {
            if (other.gameObject.layer == 14)
            {
                Debug.Log("hit player");
                animator.SetTrigger(triggerName);
            }
            else if (usePlayerLayer && other.gameObject.layer == 9)
            {
                animator.SetTrigger(triggerName);
            }
        }

        CheckPickups();

    }

    public void CheckPickups()
    {
        if(hasPickups)
        {
            for(int i = 0; i < hiddenPickups.Length; i++) 
            { 
                hiddenPickups[i].SetActive(true);
                hiddenPickups[i].transform.LookAt(hiddenPickups[i].transform.position + new Vector3(Random.Range(0f, 359f), 0f, Random.Range(0f, 359f)));
                pickupAnims[i].SetTrigger(showPickupsAnimName);
                // give exit time to this animation and allow it to transition back to its spinning/floating
            }
        }
    }
}
