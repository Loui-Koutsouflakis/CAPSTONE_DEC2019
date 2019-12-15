using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossV2 : MonoBehaviour
{
    public Transform[] body;
    public Transform[] bodyTargets;
    public Animator[] bodyAnims;
    public Transform[] targetInits;
    public Transform playerTf;
    public Transform tailSpawnPoint;
    public TransitionManager transition;
    public Material skybox;

    public GameObject[] pathOne;
    public GameObject[] pathTwo;

    public Transform leftHand;
    public Transform rightHand;
    public Transform leftHandGrabPoint;
    public Transform leftHandInitPoint;
    public Transform rightHandInitPoint;
    public Animator leftHandAnim;
    public Animator rightHandAnim;

    public static bool steering;
    public int health = 10;
    public float steerSpeed = 100f;

    public static bool steerAdjusting;
    public static bool grabIsAnimating;
    public static bool dropIsAnimating;

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
    public Animator grabCamAnim;
    public Animator dropCamAnim;
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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            CueSteer();
        }

        if (rightHandBlocking > 0f)
        {
            LerpPositionRotation(rightHand, rightHandBlockingPoint, rightHandRot, handLerpRate, steerLerpRate);
            rightHandBlocking -= Time.deltaTime;
        }
        else if(!grabIsAnimating)
        {
            LerpPositionRotation(rightHand, rightHandInitPoint.position, rightHandInitRot, handLerpRate, steerLerpRate);
        }

        if(leftHandBlocking > 0f)
        {
            LerpPositionRotation(leftHand, leftHandBlockingPoint, leftHandRot, handLerpRate, steerLerpRate);
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

        if ((Input.GetButtonDown(bumperName)) && steerVolume.canSteer)
        {
            CueSteer();
        }

        skyboxRotation -= Time.deltaTime;
        skybox.SetFloat("_Rotation", skyboxRotation % 360f);
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

    public void CueSteer()
    {
        Time.timeScale = 1.25f;

        //playerTf.gameObject.SetActive(false);

        bossCam.enabled = true;
        playerCam.enabled = false;
        steerVolume.canSteer = false;
        steerVolume.enabled = false;  //don't forget to re-enable this
        steering = true;
    }

    public void EndSteer()
    {
        Time.timeScale = 1f;
        steering = false;
    }

    public IEnumerator IntroSequence()
    {
        //Pause is disabled for cinematic
        //Intro cinematic plays

        yield return new WaitForSeconds(1f);

        //Fade to player camera

        yield return new WaitForSeconds(1f);

        //Player has control and can start playing level 
    }

    public IEnumerator DamageSequence()
    {
        EndSteer();

        //bodyAnims[0].enabled = false;
        steerAdjusting = true;

        //Animations for boss being hurt

        //Code for readjusting body back to default position

        yield return new WaitForSeconds(2f);

        steerAdjusting = false;
        //bodyAnims[0].enabled = true;

        yield return new WaitForSeconds(0.5f);

        grabCamAnim.SetTrigger("Pan");

        yield return new WaitForSeconds(0.5f);

        grabCam.enabled = true;
        bossCam.enabled = false;
        grabIsAnimating = true;

        yield return new WaitForSeconds(1f);

        leftHandAnim.SetTrigger("Grab");

        //Fade Out

        StartCoroutine(transition.BlinkSequence(3.2f, 0.5f, 3.2f, 1f, false));

        yield return new WaitForSeconds(3f);

        grabCam.enabled = false;
        dropCam.enabled = true;
        playerTf.position = tailSpawnPoint.position;

        //foreach (GameObject go in pathOne) 
        //{
        //    go.SetActive(false);
        //    yield return new WaitForSeconds(0.06f);
        //}


        yield return new WaitForSeconds(1.2f);


        //foreach(GameObject go in pathTwo)
        //{
        //    go.SetActive(true);
        //    yield return new WaitForSeconds(0.06f);
        //}

        steerVolume.canSteer = false;
        //steerVolume.enabled = true;

        dropIsAnimating = true;

        //Fade In
        yield return new WaitForSeconds(0.9f);

        leftHandAnim.SetTrigger("ReverseGrab");
        dropCamAnim.SetTrigger("Pan");

        for (int i = 1; i < bodyAnims.Length; i++)
        {
            bodyAnims[i].SetTrigger("Swap");

            yield return new WaitForSeconds(0.2f);
        }

        yield return new WaitForSeconds(2.1f);

        playerCam.enabled = true;
        dropCam.enabled = false;
        dropIsAnimating = false;
        grabIsAnimating = false;

        //Hand Animations Finish, bringing Lumi back to tail

        //Player is given back control over the dragon
    }

    public IEnumerator DeathSequence()
    {
        steering = false;

        //Pause is Disabled for Cinematic
        //Cue Ending In-Game Cinematic

        yield return new WaitForSeconds(1f);

        //Fade Transition to prerender of Luke's Ending Cinematic

        yield return new WaitForSeconds(1f);

        //Fade Transition to prerendered Credits / Ending

        yield return new WaitForSeconds(1f);

        //Return to main menu
    }
}
