//Luke Fentress 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerClass : MonoBehaviour
{


    [Header("RigidBody")]
    [SerializeField]
    public Rigidbody rb;

    [Header("Animation Controller")]
    [SerializeField]
    Animator anim;
    public Animator GetAnimator()
    {
        return anim;
    }


    
    //this is where I put the player movement scripts. They're nested in game objects, which I did so that I could keep them all
    //in an array playerMovementArray, which makes things simpler to cycle through using the SetMovementType() function

    #region player movement components

    //Grapple
    GameObject grapple; 
    GrappleComponent grappleComponent;
    public GrappleComponent GetGrappleComponent()
    {
        return grappleComponent;
    }

    //Crouch
    GameObject crouch; 
    Crouch crouchComponent;
    public Crouch GetCrouchComponent()
    {
        return crouchComponent;
    }


    //Alek's ground script
    GameObject move;
    PlayerMovementv3 moveComponent;
    public PlayerMovementv3 GetMoveComponent()
    {
        return moveComponent;
    }

    GameObject[] playerMovementArray = new GameObject[3];
    #endregion


    
    //this is where I put the player's dynamic variables such as health, grounded, canFlutter, etc...
    #region dynamic player variables
    //HEALTH
    [Header("Health")]
    [SerializeField]
    int health;

    public int GetHealth()
    {
        return health; 
    }

    public void SetHealth(int healthChange)
    {
        health += healthChange; 
    }


    //IS GROUNDED
    [Header("Is Grounded")]
    [SerializeField]
    bool grounded;

    public bool IsGrounded()
    {
        return grounded; 

    }

    public void SetGrounded(bool value)
    {
        grounded = value; 
    }

    //CAN FLUTTER
    [Header("Can Flutter")]
    [SerializeField]
    bool flutter;

    public bool CanFlutter()
    {
        return flutter;

    }

    public void SetFlutter(bool value)
    {
        flutter = value;
    }

    private RaycastHit footHit;
    public RaycastHit GetFootHit()
    {
        return footHit; 
    }



    //ON WALL

    bool onWall;

    public bool IsOnWall()
    {
        return onWall;

    }

    public void SetOnWall(bool value)
    {
        onWall = value;
    }

    //TETHER

    bool tether;

    public bool IsTethered()
    {
        return grappleComponent.tether;

    }

    public void SetTethered(bool value)
    {
        tether = value;
    }



    #endregion




    
    //using a string to activate which script is use. The strings are "grapple", "move", "crouch" 
    public void SetMovementType(string moveType)
    {
        for (int i = 0; i < playerMovementArray.Length; i++)
        {

            if (playerMovementArray[i].name != moveType)
                playerMovementArray[i].SetActive(false);

            else
                playerMovementArray[i].SetActive(true);

        }
    }

    //handy way to make sure there's nothing wrong
    public void InitializePlayer()
    {
        try
        {
            rb = GetComponent<Rigidbody>();
        }
        catch
        {
            rb = this.gameObject.AddComponent<Rigidbody>();
        }

        try
        {
            moveComponent = move.GetComponent<PlayerMovementv3>();
        }
        catch
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            go.transform.position = transform.position;
            move = go;
            move.AddComponent<PlayerMovementv3>();
            moveComponent = move.GetComponent<PlayerMovementv3>();
            move.name = "move";

        }

        playerMovementArray[0] = move;

        try
        {
            grappleComponent = grapple.GetComponent<GrappleComponent>();
        }
        catch
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            go.transform.position = transform.position;
            grapple = go;
            grapple.AddComponent<GrappleComponent>();
            grappleComponent = grapple.GetComponent<GrappleComponent>();
            grapple.name = "grapple";

        }

        grappleComponent.Initialize();
        playerMovementArray[1] = grapple;

        try
        {
            crouchComponent = crouch.GetComponent<Crouch>();
        }
        catch
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            go.transform.position = transform.position;
            crouch = go;
            crouch.AddComponent<Crouch>();
            crouchComponent = crouch.GetComponent<Crouch>();
            crouch.name = "crouch";


        }

        playerMovementArray[2] = crouch;

        for (int i = 0; i < playerMovementArray.Length; i++)
        {
            Debug.Log(playerMovementArray[i].name + " is in the player movement array");
        }

       
        SetMovementType("move");
    }





}
