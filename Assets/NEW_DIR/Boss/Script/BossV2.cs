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

    //private readonly Vector3[] targetInitOffset =
    //{
    //    new Vector3(0f, 0f, 0f),
    //    new Vector3(-0.5688758f, -2.2f, -18.96224f),
    //    new Vector3(-4.401353f, -3.9f, -46.54705f),
    //    new Vector3(-7.734359f, -2.73f, -73.86629f),
    //    new Vector3(-17.17431f, -3.9f, -98.58646f),
    //    new Vector3(-27.90802f, -2.73f, -123.434f),
    //    new Vector3(-40.29121f, -3.9f, -146.836f),
    //    new Vector3(-57.83416f, -5.46f, -175.1439f)
    //};

    public GameObject[] pathOne;
    public GameObject[] pathTwo;

    public Transform leftHand;
    public Transform rightHand;
    public Transform leftHandGrabPoint;
    public Animator leftHandAnim;
    public Animator rightHandAnim;

    public bool steering;
    public int health = 10;
    public float steerSpeed = 100f;

    bool steerAdjusted;
    bool grabIsAnimating;

    public float leftHandBlocking;
    public float rightHandBlocking;

    float handLerpRate = 8f;
    float horizontalInput;
    float verticalInput;
    float steerLerpRate = 8.6f;

    readonly float bossMaxY = -5f;
    readonly float bossMinY = -80f;
    readonly float bossMaxX = 110f;
    readonly float bossMinX = -120f;

    readonly float dragonInputThreshold = 0.14f;
    readonly float steerRotLerp = 12.75f;

    public Vector3 leftHandBlockingPoint;
    public Vector3 rightHandBlockingPoint;
    Vector3 leftHandInitPos;
    Vector3 rightHandInitPos;
    Vector3 leftHandInitRot;
    Vector3 rightHandInitRot;
    Vector3 steer;
    Vector3 steerRot;
    Vector3 rightHandRot = new Vector3(90f, 270f, 0f);
    Vector3 leftHandRot = new Vector3(90f, 90f, 0f);

    public Transform bossCamTarget;
    public Camera playerCam;
    public Camera bossCam;
    public BossSteerVolume steerVolume;

    const string bumperName = "RightBumper";

    private void Start()
    {
        if(bossCam.enabled)
        {
            bossCam.enabled = false;
        }

        rightHandInitPos = rightHand.position;
        leftHandInitPos = leftHand.position;
        rightHandInitRot = rightHand.rotation.eulerAngles;
        leftHandInitRot = leftHand.rotation.eulerAngles;

    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            CueSteer();
        }

        if (rightHandBlocking > 0f)
        {
            LerpPositionRotation(rightHand, rightHandBlockingPoint, rightHandRot);
            rightHandBlocking -= Time.deltaTime;
        }
        else if(!grabIsAnimating)
        {
            LerpPositionRotation(rightHand, rightHandInitPos, rightHandInitRot);
        }
        else
        {

        }

        if(leftHandBlocking > 0f)
        {
            LerpPositionRotation(leftHand, leftHandBlockingPoint, leftHandRot);
            leftHandBlocking -= Time.deltaTime;
        }
        else if(!grabIsAnimating)
        {
            LerpPositionRotation(leftHand, leftHandInitPos, leftHandInitRot);
        }
        else
        {
            LerpPositionRotation(leftHand, leftHandGrabPoint.position, leftHandInitRot);
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
                    body[i].rotation = Quaternion.Lerp(body[i].rotation, Quaternion.LookRotation(body[i - 1].position - body[i].position), steerRotLerp * Time.deltaTime);
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

                //steerAdjusted = false;
            }
            else
            {
                steer = Vector3.zero;

                //if(!steerAdjusted)
                //{
                //    for (int i = 1; i < bodyTargets.Length; i++)
                //    {
                //        bodyTargets[i].position = targetInits[i].position;
                //    }
                //
                //    steerAdjusted = true;
                //}

                body[0].rotation = Quaternion.Lerp(body[0].rotation, Quaternion.Euler(0f, -17f, 0f), steerLerpRate * Time.deltaTime);
            }
        }

        if ((Input.GetButtonDown(bumperName)) && steerVolume.canSteer)
        {
            CueSteer();
        }
    }

    public void LerpPositionRotation(Transform hand, Vector3 blockingPoint, Vector3 euler)
    {
        hand.position = Vector3.Lerp(hand.position, blockingPoint, handLerpRate * Time.deltaTime);

        hand.rotation = Quaternion.Lerp(hand.rotation, Quaternion.Euler(euler), steerRotLerp * Time.deltaTime);
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

            //bodyTargets[i].localPosition += steerDirection * steerSpeed * Time.deltaTime; 
        }
    }

    // Cue Steer

    public void CueSteer()
    {
        Time.timeScale = 1.25f;
        playerTf.gameObject.SetActive(false);
        bossCam.enabled = true;
        playerCam.enabled = false;
        steerVolume.canSteer = false;
        steerVolume.enabled = false;  //don't forget to re-enable this
        steering = true;
    }

    public void EndSteer()
    {
        Time.timeScale = 1f;

        playerCam.enabled = true;
        bossCam.enabled = false;
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
        steering = false;

        //Animations for boss being hurt
        //Code for readjusting body back to default position

        yield return new WaitForSeconds(1f);

        //Readjustment finished

        //Cue Animations for Hands Grabbing Lumi
        grabIsAnimating = true;

        yield return new WaitForSeconds(1f);

        leftHandAnim.SetTrigger("Grab");

        //Fade Out

        yield return new WaitForSeconds(1f);

        // Anim here
        foreach (GameObject go in pathOne) 
        {
            go.SetActive(false);
            yield return new WaitForSeconds(0.06f);
        }

        // Anim Here
        foreach(GameObject go in pathTwo)
        {
            go.SetActive(true);
            yield return new WaitForSeconds(0.06f);
        }

        steerVolume.canSteer = false;
        //steerVolume.enabled = true;

        //Fade In

        leftHandAnim.SetTrigger("ReverseGrab");

        yield return new WaitForSeconds(1f);

        grabIsAnimating = false;
        //Hand Animations Finish, bringing Lumi back to tail

        bossCam.enabled = false;
        playerCam.enabled = true;

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
