using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : MonoBehaviour
{
    public GameObject player;
    public GameObject enemyGraphic;

    private float yAtDrop;
    private bool aboveGround;


    // Use this for initialization
    void Start ()
    {
        aboveGround = true;
        yAtDrop = gameObject.transform.position.y;        
	}
	
	// Update is called once per frame
	void Update ()
    {

        Moving();
        Hiding();
        if (GetComponent<UnityEngine.AI.NavMeshAgent>().isActiveAndEnabled)
        {
            GetComponent<UnityEngine.AI.NavMeshAgent>().SetDestination(player.transform.position);
        }
	}

    void Moving()
    {
        if(player.GetComponent<InVisual>().GetGhostInSight() == false && aboveGround == true)
        {
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = true;
            enemyGraphic.SetActive(true);
        }

    }

    void Hiding()
    {
        if (player.GetComponent<InVisual>().GetGhostInSight() == true)
        {
            GetComponent<UnityEngine.AI.NavMeshAgent>().enabled = false;
            enemyGraphic.SetActive(false);
        }


    }

    void OnCollisionEnter(Collision collider)
    {
 
        if (collider.gameObject.GetComponent<Health>() == true && collider.gameObject.layer == LayerMask.NameToLayer("Player") && enemyGraphic.activeSelf == true)
        {
            collider.gameObject.GetComponent<Health>().TakeDMG(30);

        }

    }
}
