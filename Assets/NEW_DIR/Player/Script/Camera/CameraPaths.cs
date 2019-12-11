using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("Camera/Camera Path", 1)]
public class CameraPaths : MonoBehaviour
{
    public bool playOnStart = false;
    public bool active = false;
    public CPC_CameraPath path;
    public bool hasEndCam = false;
    public CPC_CameraPath endCam;
    public float endCamStayTime;
    public CPC_CameraPath[] paths;
    public float time = 10;
    public float stayTime;
    bool hasPlayed = false;
    public ParticleSystem[] particles;
    public bool hasParticles;
    [Header("If the object is collection based")]
    [Tooltip("Crystal threshold")]
    public float c_Threshold;
    public bool playAnimWithoutActive = false;

    [Header("Animations?")]
    [Tooltip("If your path requires an animation")]
    public Animation anims;

    //SaveGameManager variable
    SaveGameManager saveGameManager;

    // Start is called before the first frame update
    void Start()
    {
        saveGameManager = GameObject.FindGameObjectWithTag("SaveGameManager").GetComponent<SaveGameManager>(); 
        paths = FindObjectsOfType<CPC_CameraPath>();
        if (anims == null)
            Debug.Log("No big deal");
        else
            anims.Stop();
        if (playOnStart)
            active = true;
        if(saveGameManager.triggeredMonoliths.Contains(gameObject.name + SceneManager.GetActiveScene()))
        {
            playAnimWithoutActive = true;
        }
        if (playAnimWithoutActive) { hasPlayed = true;  if(anims != null) anims.Play(); }

    }

    // Update is called once per frame
    void Update()
    {
        

        if (active)
        {
            if(hasParticles)
            {
                foreach (var item in particles)
                {
                    item.Play();
                }
            }

            foreach (var item in paths)
            {
                item.gameObject.SetActive(false);
            }
            path.gameObject.SetActive(true);
            if (playOnStart)
            {
                if (anims != null)
                    anims.Play();
            }
          
            StartCoroutine(goTime());
        }
    }

    public void StartMeUp()
    {
        if (!hasPlayed)
        {
            active = true;
            
            hasPlayed = true;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == 14 && SceneManager.GetActiveScene().buildIndex != 3)
        {
            if (!hasPlayed)
            {
                if (HudManager.shardsCollected >= c_Threshold)
                {
                    active = true;
                    hasPlayed = true;
                    if (saveGameManager)
                    {
                        saveGameManager.AddToListOfTriggeredMonoliths(gameObject.name + SceneManager.GetActiveScene());
                        saveGameManager.SaveTriggeredMonoliths();
                    }
                    if (anims != null)
                        anims.Play();
                }
            }
        }
    }

    IEnumerator goTime()
    {
        if (hasParticles)
        {
            StartCoroutine(StopParticles(3));
        }
        yield return new WaitForEndOfFrame();
        active = false;
        if (hasEndCam) path.PlayPath(time, stayTime, endCamStayTime,endCam);
        else
        path.PlayPath(time,stayTime, endCamStayTime, null);
    }

    IEnumerator StopParticles(float t)
    {
        yield return new WaitForSeconds(t);
        particles[1].Stop();

    }
}
