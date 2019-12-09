using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossV2 : MonoBehaviour
{
    public Transform[] body;
    public Transform[] bodyTargets;
    public Animator[] bodyAnims;

    public GameObject[] pathOne;
    public GameObject[] pathTwo;

    public Transform leftHand;
    public Transform rightHand;
    public Animator leftHandAnim;
    public Animator rightHandAnim;

    public bool steering;
    public float steerSpeed = 75f;

    float leftHandBlocking;
    float rightHandBlocking;

    float handLerpRate;
    readonly float dragonInputThreshold = 0.14f;
    readonly float steerLerpRate = 4f;

    Vector3 leftHandBlockingPoint;
    Vector3 rightHandBlockingPoint;
    Vector3 steer;

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
        //
        //if(Input.GetKeyDown(KeyCode.B))
        //{
        //    steering = true;
        //}
        //

        if(rightHandBlocking < 0f)
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

            for(int i = 0; i < body.Length; i++)
            {
                body[i].position = Vector3.Lerp(body[i].position, bodyTargets[i].position, steerLerpRate * Time.deltaTime);

                if (i > 0)
                {
                    body[i].rotation = Quaternion.Lerp(body[i].rotation, Quaternion.LookRotation(body[i - 1].position - body[i].position), steerLerpRate * Time.deltaTime);
                }
                else
                {
                    body[0].rotation = Quaternion.Lerp(body[0].rotation, Quaternion.LookRotation( body[0].position + (Vector3.forward*8f) + steer), steerLerpRate * Time.deltaTime);
                }
            }

            if (Mathf.Abs(Input.GetAxis("Horizontal")) > 0.14f || Mathf.Abs(Input.GetAxis("Vertical")) > 0.14f)
            {
                steer.x = Input.GetAxis("Horizontal");
                steer.y = Input.GetAxis("Vertical");
                StartCoroutine(SteerRoutine(steer));
            }
            else
            {
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
        foreach (Transform target in bodyTargets)
        {
            target.localPosition += steerDirection * steerSpeed * Time.deltaTime;
            yield return new WaitForSeconds(0.1f);
        }
    }

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
