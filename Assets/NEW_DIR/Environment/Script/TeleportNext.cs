using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class TeleportNext : MonoBehaviour
{
    public TransitionManager t_Manager;

    public PlayerClass player;

    public Transform teleportPsParent;
    public ParticleSystem[] teleport;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if(collision.gameObject.layer == 14)
        {
            if(gameObject.name == "TeleportNext")
            {
                t_Manager.StartCoroutine(t_Manager.SceneTransition(1));
            }
            else if(gameObject.name == "TeleportPrevious")
            {
                t_Manager.StartCoroutine(t_Manager.SceneTransition(-1));
            }

            teleportPsParent.position = player.transform.position;

            foreach(ParticleSystem ps in teleport)
            {
                ps.Play();
            }

            player.gameObject.SetActive(false);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
