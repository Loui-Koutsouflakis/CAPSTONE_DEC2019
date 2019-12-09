using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossV2 : MonoBehaviour
{
    public Transform[] body;
    public Transform[] bodyTargets;
    public Animator[] bodyAnims;
    public Transform[] targetInits;

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
    public Animator leftHandAnim;
    public Animator rightHandAnim;

    public bool steering;
    public float steerSpeed = 100f;

    bool steerAdjusted;

    float leftHandBlocking;
    float rightHandBlocking;

    float handLerpRate;
    float horizontalInput;
    float verticalInput;
    float steerLerpRate = 2.6f;

    readonly float bossMaxY = 15f;
    readonly float bossMinY = -60f;
    readonly float bossMaxX = 90f;
    readonly float bossMinX = -90f;

    readonly float dragonInputThreshold = 0.14f;
    readonly float steerRotLerp = 2.75f;

    Vector3 leftHandBlockingPoint;
    Vector3 rightHandBlockingPoint;
    Vector3 steer;
    Vector3 steerRot;

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
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            CueSteer();
        }

        if (rightHandBlocking < 0f)
        {
            rightHand.position = Vector3.Lerp(rightHand.position, rightHandBlockingPoint, handLerpRate * Time.deltaTime);
        }

        if(leftHandBlocking < 0f)
        {
            leftHand.position = Vector3.Lerp(leftHand.position, leftHandBlockingPoint, handLerpRate * Time.deltaTime);
        }

        if (steering)
        {
            bossCam.gameObject.transform.position = Vector3.Lerp(bossCam.gameObject.transform.position, bossCamTarget.position, handLerpRate * Time.deltaTime);
            
            horizontalInput = Input.GetAxis("Horizontal") + Input.GetAxis("HorizontalJoy");
            verticalInput = Input.GetAxis("Vertical") + Input.GetAxis("VerticalJoy");

            for (int i = 0; i < body.Length; i++)
            {
                body[i].position = Vector3.Lerp(body[i].position, bodyTargets[i].position, steerLerpRate * Time.deltaTime);

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
                steerAdjusted = false;
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

    public IEnumerator SteerRoutine(Vector3 steerDirection)
    {
        Vector3 holdSteerDirection = steerDirection;

        for (int i = 0; i < bodyTargets.Length; i++)
        {
            if (i == 0)
            {
                if (bodyTargets[0].localPosition.y > bossMaxY && steerDirection.y > 0f)
                {
                    steerDirection.y = 0f;
                }
                else if (bodyTargets[0].localPosition.y < bossMinY && steerDirection.y < 0f)
                {
                    steerDirection.y = 0f;
                }
                else
                {
                    steerDirection.y = holdSteerDirection.y;
                }

                if (bodyTargets[0].localPosition.x > bossMaxX && steerDirection.x > 0f)
                {
                    steerDirection.x = 0f;
                }
                else if (bodyTargets[0].localPosition.x < bossMinX && steerDirection.x < 0f)
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

    // Cue Steer

    public void CueSteer()
    {
        bossCam.enabled = true;
        playerCam.enabled = false;
        steerVolume.canSteer = false;
        steerVolume.enabled = false;  //don't forget to re-enable this
        steering = true;
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

        yield return new WaitForSeconds(1f);

        //Fade Out

        yield return new WaitForSeconds(1f);

        foreach (GameObject go in pathOne) 
        {
            go.SetActive(false);
            yield return new WaitForSeconds(0.06f);
        }

        foreach(GameObject go in pathTwo)
        {
            go.SetActive(true);
            yield return new WaitForSeconds(0.06f);
        }

        //Fade In

        yield return new WaitForSeconds(1f);

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
