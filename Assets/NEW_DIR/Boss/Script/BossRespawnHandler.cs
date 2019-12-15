using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BossRespawnHandler : MonoBehaviour
{
    public Transform playerTf;
    public TransitionManager transition;
    public Vector3 respawn;

    public static bool respawning;

    public IEnumerator Respawn()
    {
        respawning = true;

        StartCoroutine(transition.BlinkSequence(1.5f, 0.2f, 1.5f, 1f, false));

        yield return new WaitForSeconds(1.55f);

        if (HudManager.health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        else
        {
            playerTf.position = respawn;
        }

        yield return new WaitForSeconds(1.55f);

        respawning = false;
    }
}
