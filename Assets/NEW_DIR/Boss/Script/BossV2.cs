using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.SceneManagement;

public class BossV2 : MonoBehaviour
{
    public Transform[] body;
    public Transform[] bodyTargets;
    public Animator[] bodyAnims;
    public Animator[] dragonPlats1;
    public Animator[] dragonPlats2;
    public Animator[] dragonFloatPlats;
    public Animator dragonHeadAnim;
    public Transform[] targetInits;
    public Transform playerTf;
    public Transform tailSpawnPoint;
    public Transform lumiPlayPoint;
    public TransitionManager transition;
    public Material skybox;
    public AudioSource musicSource;
    public AudioClip endCinemaSong;
    public AudioClip creditsSong;

    public GameObject hudManager;
    public PlayerClass playerClass;
    public HandleSfx playerSfx;
    public HandleSfx bossSfx;
    public GameObject[] pathOne;
    public GameObject[] pathTwo;
    public GameObject pauseMenu;
    public Tumble worldTumble;
    public Tumble obeliskTumble;
    public Tumble innerParallaxTumble;
    public Tumble outerParallaxTumble;
    public Transform leftHand;
    public Transform rightHand;
    public Transform leftHandGrabPoint;
    public Transform leftHandInitPoint;
    public Transform rightHandInitPoint;
    //public Animator leftHandAnim;
    public Animator rightHandAnim;
    public VideoPlayer endCinematic;

    public int health = 10;
    public float steerSpeed = 100f;

    public static bool steering;
    public static bool steerAdjusting;
    public static bool grabIsAnimating;
    public static bool dropIsAnimating;
    public static bool canDie;
    public static bool endingIsPlaying;

    public float leftHandBlocking;
    public float rightHandBlocking;

    float handLerpRate = 8f;
    float horizontalInput;
    float verticalInput;
    float steerLerpRate = 8.6f;
    float skyboxRotation = 0f;

    readonly float bossMaxY = -5f;
    readonly float bossMinY = -80f;
    readonly float bossMaxX = 110f;
    readonly float bossMinX = -120f;

    readonly float dragonInputThreshold = 0.14f;
    readonly float steerRotLerp = 12.75f;

    public Vector3 leftHandBlockingPoint;
    public Vector3 rightHandBlockingPoint;
    Vector3 leftHandInitRot;
    Vector3 rightHandInitRot;
    Vector3 steer;
    Vector3 steerRot;
    Vector3 rightHandRot = new Vector3(90f, 270f, 0f);
    Vector3 leftHandRot = new Vector3(90f, 90f, 0f);

    public Transform bossCamTarget;
    public Camera playerCam;
    public Camera bossCam;
    public Camera grabCam;
    public Camera dropCam;
    public Camera cinemaCam;
    public Camera crystalCam;
    public Camera videoCam;
    public Animator leftHandParentAnim;
    public Animator grabCamAnim;
    public Animator controlCrystalAnim;
    //public Animator dropCamAnim;
    public Animator cinematicAnim;
    public BossSteerVolume steerVolume;

    const string bumperName = "RightBumper";

    private void Start()
    {
        if(bossCam.enabled)
        {
            bossCam.enabled = false;
        }

        rightHandInitRot = rightHand.rotation.eulerAngles;
        leftHandInitRot = leftHand.rotation.eulerAngles;
        skyboxRotation = 0f;
        canDie = true;

        StartCoroutine(IntroSequence());
    }

    private void Update()
    {
        //if (Input.GetKeyDown(KeyCode.B))
        //{
        //    CueSteer();
        //}

        if (rightHandBlocking > 0f)
        {
            LerpPositionRotation(rightHand, rightHandBlockingPoint, rightHandRot, handLerpRate, steerLerpRate/4f);
            rightHandBlocking -= Time.deltaTime;
        }
        else if(!grabIsAnimating)
        {
            LerpPositionRotation(rightHand, rightHandInitPoint.position, rightHandInitRot, handLerpRate, steerLerpRate/4f);
        }

        if(leftHandBlocking > 0f)
        {
            LerpPositionRotation(leftHand, leftHandBlockingPoint, leftHandRot, handLerpRate, steerLerpRate);

            if(leftHandBlocking - Time.deltaTime <= 0f)
            {
                leftHandParentAnim.enabled = true;
            }

            leftHandBlocking -= Time.deltaTime;
        }
        else if(!grabIsAnimating)
        {
            LerpPositionRotation(leftHand, leftHandInitPoint.position, leftHandInitRot, handLerpRate, steerLerpRate);
        }
        else
        {
            if (dropIsAnimating)
            {
                LerpPositionRotation(leftHand, tailSpawnPoint.position, leftHandInitRot, handLerpRate, steerLerpRate/3f);
            }
            else
            {
                LerpPositionRotation(leftHand, leftHandGrabPoint.position, leftHandInitRot, handLerpRate, steerLerpRate/3f);
            }
        }

        if (steering)
        {
            bossCam.gameObject.transform.position = Vector3.Lerp(bossCam.gameObject.transform.position, bossCamTarget.position, handLerpRate * Time.deltaTime);
            
            horizontalInput = Input.GetAxis("Horizontal") + Input.GetAxis("HorizontalJoy");
            verticalInput = Input.GetAxis("Vertical") + Input.GetAxis("VerticalJoy");

            for (int i = 0; i < body.Length; i++)
            {
                body[i].position = Vector3.Lerp(body[i].position, bodyTargets[i].position, steerLerpRate/i * Time.deltaTime);

                if (i > 0)
                {
                    //body[i].rotation = Quaternion.Lerp(body[i].rotation, Quaternion.LookRotation(body[i - 1].position - body[i].position), steerRotLerp * Time.deltaTime);

                    body[i].rotation = Quaternion.Lerp(body[i].rotation, targetInits[i].rotation, steerLerpRate / 6f * Time.deltaTime);
                }
                else
                {
                    steerRot.x = -steer.y * 30f;
                    steerRot.y = (steer.x * 30f) - 17f;
                    body[0].rotation = Quaternion.Lerp(body[0].rotation, Quaternion.Euler(steerRot), steerRotLerp * Time.deltaTime);
                }
            }

            steer.x = horizontalInput;
            steer.y = verticalInput;

            if (Mathf.Abs(horizontalInput) > 0.03f || Mathf.Abs(verticalInput) > 0.03f)
            {
                StartCoroutine(SteerRoutine(steer.normalized));
            }
            else
            {
                steer = Vector3.zero;

                body[0].rotation = Quaternion.Lerp(body[0].rotation, Quaternion.Euler(0f, -17f, 0f), steerLerpRate * Time.deltaTime);
            }
        }

        if(steerAdjusting)
        {
            for (int i = 0; i < bodyTargets.Length; i++)
            {
                body[i].position = Vector3.Lerp(body[i].position, targetInits[i].position, steerLerpRate/4f * Time.deltaTime);
                body[i].rotation = Quaternion.Lerp(body[i].rotation, targetInits[i].rotation, steerLerpRate/6f * Time.deltaTime);
                bodyTargets[i].position = Vector3.Lerp(bodyTargets[i].position, targetInits[i].position, steerLerpRate/4f * Time.deltaTime);
            }
        }

        if (steerVolume.canSteer)
        {
            StartCoroutine(CueSteerRoutine());
        }

        skyboxRotation -= Time.deltaTime;
        skybox.SetFloat("_Rotation", skyboxRotation % 360f);

        if ((Input.GetButtonDown("Start") || Input.GetButtonDown("AButton") || Input.GetKeyDown(KeyCode.Space) || Input.GetKeyDown(KeyCode.Return)) && endingIsPlaying)
        {
            StopAllCoroutines();
            StartCoroutine(CutEndSequence());
        }
    }

    public static void LerpPositionRotation(Transform tf, Vector3 destination, Vector3 eulers, float positionLerp, float rotationLerp)
    {
        tf.position = Vector3.Lerp(tf.position, destination, positionLerp * Time.deltaTime);

        tf.rotation = Quaternion.Lerp(tf.rotation, Quaternion.Euler(eulers), rotationLerp * Time.deltaTime);
    }

    public IEnumerator SteerRoutine(Vector3 steerDirection)
    {
        Vector3 holdSteerDirection = steerDirection;

        for (int i = 0; i < bodyTargets.Length; i++)
        {
            if (i == 0)
            {
                if (bodyTargets[i].localPosition.y > bossMaxY && steerDirection.y > 0f)
                {
                    steerDirection.y = 0f;
                }
                else if (bodyTargets[i].localPosition.y < bossMinY && steerDirection.y < 0f)
                {
                    steerDirection.y = 0f;
                }
                else
                {
                    steerDirection.y = holdSteerDirection.y;
                }

                if (bodyTargets[i].localPosition.x > bossMaxX && steerDirection.x > 0f)
                {
                    steerDirection.x = 0f;
                }
                else if (bodyTargets[i].localPosition.x < bossMinX && steerDirection.x < 0f)
                {
                    steerDirection.x = 0f;
                }
                else
                {
                    steerDirection.x = holdSteerDirection.x;
                }
            }

            bodyTargets[i].localPosition += steerDirection * steerSpeed * Time.deltaTime;

            yield return new WaitForSeconds(0.116f);
        }
    }

    public IEnumerator CueSteerRoutine()
    {
        steerVolume.canSteer = false;
        steerVolume.enabled = false;  //don't forget to re-enable this

        crystalCam.enabled = true;
        playerCam.enabled = false;
        playerClass.DisableControls();
        playerSfx.enabled = false;

        controlCrystalAnim.SetTrigger("Cue");

        yield return new WaitForSeconds(1.2f);

        CueSteer();
    }

    public void CueSteer()
    {
        Time.timeScale = 1.2f;

        //playerTf.gameObject.SetActive(false);

        bossCam.enabled = true;
        crystalCam.enabled = false;
        playerCam.enabled = false;
        steerVolume.enabled = false;
        steerVolume.canSteer = false;
        
        steering = true;
    }

    public void EndSteer()
    {
        Time.timeScale = 1f;
        steering = false;
    }

    public void ResetStaticVariables()
    {
        steering = false;
        steerAdjusting = false;
        grabIsAnimating = false;
        dropIsAnimating = false;
        canDie = true;
    }

    public IEnumerator IntroSequence()
    {
        pauseMenu.gameObject.SetActive(false);
        playerClass.DisableControls();
        playerSfx.enabled = false;
        hudManager.SetActive(false);

        cinemaCam.enabled = true;
        playerCam.enabled = false;
        worldTumble.enabled = false;
        obeliskTumble.enabled = false;
        innerParallaxTumble.enabled = false;
        outerParallaxTumble.enabled = false;

        foreach(Animator anim in dragonFloatPlats)
        {
            anim.SetTrigger("Float");
            yield return new WaitForSeconds(0.3f);
        }

        yield return new WaitForSeconds(7f);

        bossSfx.PlayOneShotByIndex(1);

        yield return new WaitForSeconds(0.6f);

        playerTf.position = lumiPlayPoint.position;

        yield return new WaitForSeconds(2);
        
        StartCoroutine(transition.BlinkSequence(2.5f, 0.2f, 2.5f, 1, false));

        yield return new WaitForSeconds(2.4f);
        
        worldTumble.enabled = true;
        obeliskTumble.enabled = true;
        innerParallaxTumble.enabled = true;
        outerParallaxTumble.enabled = true;
        playerCam.enabled = true;
        cinemaCam.enabled = false;

        pauseMenu.gameObject.SetActive(true);
        playerClass.EnableControls();
        playerSfx.enabled = true;
        hudManager.SetActive(true);
    }

    public IEnumerator DamageSequence()
    {
        EndSteer();
        
        hudManager.SetActive(false);

        steerAdjusting = true;

        //Animations for boss being hurt

        yield return new WaitForSeconds(1.6f);

        bossSfx.PlayOneShotByIndex(4);

        steerAdjusting = false;
        //bodyAnims[0].enabled = true;

        grabCam.enabled = true;
        bossCam.enabled = false;

        yield return new WaitForSeconds(0.6f);

        grabCamAnim.SetTrigger("Pan");
        leftHandParentAnim.SetTrigger("Cue");

        yield return new WaitForSeconds(1.1f);

        grabIsAnimating = true;

        yield return new WaitForSeconds(0.8f);

        //leftHandAnim.SetTrigger("Grab");

        //Fade Out

        //StartCoroutine(transition.BlinkSequence(3.2f, 0.5f, 3.2f, 1f, false));

        yield return new WaitForSeconds(2f);

        steerVolume.canSteer = false;
        //steerVolume.enabled = true;

        //dropIsAnimating = true;

        //Fade In

        yield return new WaitForSeconds(0.6f);

        //grabCam.enabled = false;
        //dropCam.enabled = true;

        //dropCamAnim.SetTrigger("Pan");

        yield return new WaitForSeconds(0.6f);

        //leftHandAnim.SetTrigger("ReverseGrab");

        for (int i = 1; i < bodyAnims.Length; i++)
        {
            bodyAnims[i].SetTrigger("Swap");

            yield return new WaitForSeconds(0.25f);
        }
        
        playerTf.position = tailSpawnPoint.position;

        yield return new WaitForSeconds(6f);

        playerClass.EnableControls();
        playerSfx.enabled = true;
        playerCam.enabled = true;
        grabCam.enabled = false;
        //dropCam.enabled = false;
        //dropIsAnimating = false;
        grabIsAnimating = false;

        //Hand Animations Finish, bringing Lumi back to tail

        //Player is given back control over the dragon


        hudManager.SetActive(true);
    }

    public IEnumerator DeathSequence()
    {
        canDie = false;

        Time.timeScale = 1f;
        
        cinematicAnim.SetTrigger("Death");

        steering = false;

        worldTumble.enabled = false;
        obeliskTumble.enabled = false;

        //Disable Pausing for Cinematic

        musicSource.Stop();

        yield return new WaitForSeconds(0.5f);

        cinemaCam.enabled = true;
        bossCam.enabled = false;
        bossSfx.PlayOneShotByIndex(2);

        yield return new WaitForSeconds(5f);

        StartCoroutine(transition.BlinkSequence(2f, 1f, 2f, 1f, false));

        musicSource.clip = endCinemaSong;
        musicSource.volume = 1f;
        musicSource.Play();

        yield return new WaitForSeconds(2f);

        videoCam.enabled = true;
        cinemaCam.enabled = false;

        endCinematic.Play();

        yield return new WaitForSeconds(12f);

        //musicSource.Stop();
        //musicSource.clip = creditsSong;
        //musicSource.Play();

        yield return new WaitForSeconds(2f);

        endingIsPlaying = true;

        yield return new WaitForSeconds(60f);

        StartCoroutine(transition.BlinkSequence(8f, 5f, 0f, 0.25f, false));

        yield return new WaitForSeconds(8f);

        SceneManager.LoadScene(0);
    }

    public IEnumerator CutEndSequence()
    {
        StartCoroutine(transition.BlinkSequence(5f, 1f, 0f, 1f, false));

        yield return new WaitForSeconds(2.5f);

        SceneManager.LoadScene(0);
    }

    public IEnumerator CueHitAnims()
    {
        dragonHeadAnim.SetTrigger("Damage");
        yield return new WaitForSeconds(0.05f);

        foreach(Animator anim in dragonPlats1)
        {
            anim.SetTrigger("Hurt");
            yield return new WaitForSeconds(0.05f);
        }

        foreach(Animator anim in dragonPlats2)
        {
            anim.SetTrigger("Hurt2");
            yield return new WaitForSeconds(0.05f);
        }
    }
}
