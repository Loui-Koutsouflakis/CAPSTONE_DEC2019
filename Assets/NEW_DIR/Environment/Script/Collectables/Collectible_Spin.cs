using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("Mike's Scripts/Collectible_Spin", 7)]

public class Collectible_Spin : MonoBehaviour
{
    #region Variables
    [SerializeField]
    ParticleSystem myPS;// Collectible Particle System Reference  
    [SerializeField, Range(0, 90)]
    float rateOfOrbRotation = 1.0f;// set rotation speed of orb
    [SerializeField, Range(0, 90)]
    float offset = 1.0f;// sets distance of point to rotate around
    [SerializeField, Range(0, 10)]
    float moveOffset = 0.0f;// sets distance of vertical movement
    [SerializeField, Range(0, 10)]
    float moveMod = 0.0f;// sets vertical movement rate
    [SerializeField, Range(0, 360)]
    float rateOfParticleXRotation = 1.0f;//used to set rotX
    [SerializeField, Range(0, 360)]
    float rateOfParticleYRotation = 1.0f;//used to set rotY
    [SerializeField, Range(0, 360)]
    float rateOfParticleZRotation = 1.0f;//used to set rotZ
    float rotX = 0.0f;// Sets X rotation in rotationVector
    float rotY = 0.0f;// Sets Y rotation in rotationVector
    float rotZ = 0.0f;// Sets Z rotation in rotationVector
    Vector3 rotationVector;// sets rotational movement of Particle System
    float yStart = 0.0f;//used to store the starting Y world position
    bool movingUp;//used to switch direction of vertical movement
    Transform collectionPool;
    HudManager hud;
    [SerializeField]
    PlayerClass player;
    SaveGameManager saveMan;

    HandleSfx sfx;
    public bool isCollected = false;
    public bool isMoon = false;
    Animator anim;

    private SphereCollider colli;
    private GameObject collectableObject;

    #endregion
    void Awake()
    {
        myPS = GetComponentInChildren<ParticleSystem>();
        yStart = transform.position.y;
        anim = GetComponentInParent<Animator>();
        //collectionPool = GameObject.FindGameObjectWithTag("CollectionPool").transform;
        hud = GameObject.FindObjectOfType<HudManager>();
        player = FindObjectOfType<PlayerClass>();
        sfx = GetComponent<HandleSfx>();
        colli = GetComponentInParent<SphereCollider>();
        collectableObject = GetComponentInParent<Animator>().gameObject;
    }

    private void Start()
    {
        //Debug.Log(gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.name);
        saveMan = FindObjectOfType<SaveGameManager>();

        if(saveMan)
            if (saveMan.ReturnListOfCollectedShards().Contains(gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.name + SceneManager.GetActiveScene().name)) { isCollected = true; }
    }
    // Update is called once per frame
    void Update()
    {
        //PSRotation();
        //if (isCollected)
        //{
        //    GetComponentInParent<SphereCollider>().enabled = false;

        //    gameObject.SetActive(false);
        //}
        //else
        //{
            RotateAround();
        //}
    }

    private void OnTriggerEnter(Collider c)//Pool them later
    {
        if (c.gameObject.layer == 14 && anim.GetCurrentAnimatorStateInfo(0).IsName("Meteor_Spin_Animation"))
        {
            if (!isMoon)
            {
                sfx.PlayOneShotByName("Collect");
                hud.ShardsUp();
                player.SetShards(1);
                isCollected = true;
                if (saveMan)
                {
                    saveMan.AddToListOfCollectedShards(gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.name + SceneManager.GetActiveScene().name);
                    //print(gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.name);
                    saveMan.SaveCollectedShardsID();
                }
            }
            else
            {
                sfx.PlayOneShotByName("Collect");
                hud.MoonsUp();
                //player.MoonsUp(1);
                isCollected = true;
                if (saveMan)
                {
                    saveMan.AddToListOfCollectedShards(gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.name + SceneManager.GetActiveScene().name);
                    //print(gameObject.transform.parent.gameObject.transform.parent.gameObject.transform.parent.gameObject.name);
                    saveMan.SaveCollectedShardsID();
                }
            }

            ///////////////////////////////////////////////////////
            //is there any reason we need the top level gameObject active after its been collected?
            collectableObject.SetActive(false);

        }
    }


    void PSRotation()//Rotates the Particle System
    {
        rotX += rateOfParticleXRotation * Time.deltaTime;
        rotY += rateOfParticleYRotation * Time.deltaTime;
        rotZ += rateOfParticleZRotation * Time.deltaTime;
        rotationVector.x = rotX;
        rotationVector.y = rotY;
        rotationVector.z = rotZ;
        //myPS.transform.eulerAngles = rotationVector;
    }

    void RotateAround()//Rotates Orb
    {
        transform.RotateAround((transform.position + transform.forward * offset), Vector3.up, rateOfOrbRotation);
        VerticalMovement();
    }

    void VerticalMovement()//Vertical Orb Movement
    {
        if (movingUp)
        {
            transform.position += Vector3.up * Time.deltaTime * moveMod;
            if (transform.position.y >= yStart + moveOffset)
            {
                movingUp = false;
            }
        }
        else
        {
            transform.position += Vector3.down * Time.deltaTime * moveMod;
            if (transform.position.y < yStart - moveOffset)
            {
                movingUp = true;
            }
        }
    }

}