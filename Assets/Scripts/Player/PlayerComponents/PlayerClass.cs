//Luke Fentress
//edited 19-08-09 by AT - updated to include air component
//19-10-16 - added generic add force fuction
//19/10/19 - modified by KE - cleaned redundant code

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AddComponentMenu("Player Scripts/Player Class", 2)]

public enum MovementType
{
    move, grapple, jump, air, crouch
}

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

    public MovementType playerCurrentMove;

    public LayerMask airMask;


    public HudManager h_Manager;

    //this is where I put the player movement scripts. They're nested in game objects, which I did so that I could keep them all
    //in an array playerMovementArray, which makes things simpler to cycle through using the SetMovementType() function

    #region player movement components

    //Grapple
    public GameObject grapple;
    GrappleComponent grappleComponent;
    public GrappleComponent GetGrappleComponent()
    {
        return grappleComponent;
    }

    public TetherPoint tetherPoint;
    public void SetTetherPoint(Collider tP)
    {
        if (tP != null)
            tetherPoint = tP.GetComponent<TetherPoint>();
        else
            tetherPoint = null;
    }

    //Crouch
    public GameObject crouch;
    Crouch crouchComponent;
    public Crouch GetCrouchComponent()
    {
        return crouchComponent;
    }

    //Water Movement
    public GameObject swim;
    WaterMovement SwimComponent;
    public WaterMovement GetSwimComponent()
    {
        return SwimComponent;
    }


    //Ground script
    public GameObject move;

    PlayerGroundMovement moveComponent;

    public PlayerGroundMovement GetMoveComponent()
    {
        return moveComponent;
    }

    //air script
    public GameObject air;
    PlayerAirMovement airComponent;
    public PlayerAirMovement GetAirComponent()
    {
        return airComponent;
    }

    public GameObject stuck;
    PlayerAirMovement stuckComponnent;
    public PlayerAirMovement GetStuckComponent()
    {
        return stuckComponnent;
    }

    GameObject[] playerMovementArray = new GameObject[4];
    #endregion



    //this is where I put the player's dynamic variables such as health, grounded, canFlutter, etc...
    #region dynamic player variables
    //HEALTH
    [Header("Health")]
    public int health = 3;

    public int GetHealth()
    {
        return health;
    }

    public void SetHealth(int healthChange)
    {
        health += healthChange;
    }

    public int shards;
    private int healthShards;

    public int GetShards()
    {
        return shards;
    }

    public void SetShards(int value)
    {
        shards += value;
        healthShards = shards;
        if(healthShards >= 20 && health < 3)
        {
            SetHealth(1);
            h_Manager.HealthUp();
            healthShards = 0;
        }
        
    }


    public Vector3 vel;

    private bool controlsEnabled;

     


    bool crouching;

    //gets crouching from player controller to allow air controller to properly limit air movement
    public void SetCrouching(bool crouch)
    {
        crouching = crouch;
    }
    public bool GetCrouching()
    {
        return crouching;
    }

    bool groundPounding;

    public void SetGroundPounding(bool value)
    {
        groundPounding = value;
    }

    public bool GetGroundPounding()
    {
        return groundPounding;
    }

    //Recently added swimming test
    bool swimming;
    public void SetSwimming(bool swim)
    {
        swimming = swim;
    }
    public bool GetSwimming()
    {
        return swimming;
    }
    // for jump bug fix
    [SerializeField]
    private bool groundCheckEnabled = true;

    public bool GetGroundCheck()
    {
        return groundCheckEnabled;
    }

    public void SetGroundCheck(bool value)
    {
        groundCheckEnabled = value;
    }

    public IEnumerator GroundCheckStop()
    {
        yield return new WaitForSeconds(0.3f);
        SetGroundCheck(true);
        //Debug.Log("ground check stopped");
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


    //CAN FLUTTER
    [Header("falling")]
    public bool isFalling;

    //ISGRAPPLING
    [Header("grapplinig")]
    public bool isGrappling;    

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

    public Transform attachedGrapplePoint;

    //Long and high jumps
    [SerializeField]
    private bool isHighJump;
    [SerializeField]
    private bool isLongJump;
    [SerializeField]
    private bool isDoubleJump;

    public bool GetHighJump()
    {
        return isHighJump;
    }

    public void SetHighJump(bool value)
    {
        isHighJump = value;
    }

    public bool GetLongJump()
    {
        return isLongJump;
    }

    public void SetLongJump(bool value)
    {
        isLongJump = value;
    }

    public bool GetDoubleJump()
    {
        return isDoubleJump;
    }

    public void SetDoubleJump(bool value)
    {
        isDoubleJump = value;
    }

    #endregion




    public void SetMovementType(MovementType mT)
    {
        switch (mT)
        {
            case MovementType.air:
                playerCurrentMove = MovementType.air;
                SetMovementComponent("air");
                break;
            case MovementType.grapple:
                playerCurrentMove = MovementType.grapple;
                SetMovementComponent("grapple");
                break;
            case MovementType.move:
                playerCurrentMove = MovementType.move;
                SetMovementComponent("move");
                break;
            case MovementType.crouch:
                playerCurrentMove = MovementType.crouch;
                SetMovementComponent("crouch");
                break;
            //case MovementType.swim:
            //    playerCurrentMove = MovementType.swim;
            //    SetMovementComponent("swim");
            //    break;

        }
    }


    //using a string to activate which script is use. The strings are "grapple", "move", "crouch"
    public void SetMovementComponent(string moveType)
    {
        for (int i = 0; i < playerMovementArray.Length; i++)
        {

            if (playerMovementArray[i].name != moveType)
                playerMovementArray[i].SetActive(false);


            else
            {
                playerMovementArray[i].SetActive(true);
            }

        }
    }


    //NO LONGER IN USE
    //public GameObject GetMovementType()
    //{
    //    for (int i = 0; i < playerMovementArray.Length; i++)
    //    {
    //        if(playerMovementArray[i].activeSelf)
    //        {
    //            return playerMovementArray[i];
    //        }
    //    }

    //    return null;
    //}

    //handy way to make sure there's nothing wrong
    public void InitializePlayer()
    {
       
        //set up ground component
        try
        {
            moveComponent = move.GetComponent<PlayerGroundMovement>();
        }
        catch
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            go.transform.position = transform.position;
            move = go;
            //move.AddComponent<PlayerMovementv3>();
            move.AddComponent<PlayerGroundMovement>();
            //moveComponent = move.GetComponent<PlayerMovementv3>();
            moveComponent = move.GetComponent<PlayerGroundMovement>();
            move.name = "move";
        }

        playerMovementArray[0] = move;

        //set up grapple component
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
        playerMovementArray[1] = grapple;

        //set up crouch component
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

        ////set up swim component
        //try
        //{
        //    SwimComponent = swim.GetComponent<WaterMovement>();
        //}
        //catch
        //{
        //    GameObject go = new GameObject();
        //    go.transform.parent = transform;
        //    go.transform.position = transform.position;
        //    swim = go;
        //    swim.AddComponent<WaterMovement>();
        //    SwimComponent = swim.GetComponent<WaterMovement>();
        //    swim.name = "swim";
        //}

        //playerMovementArray[3] = swim;

        //set up air component
        try
        {
            airComponent = air.GetComponent<PlayerAirMovement>();
        }
        catch
        {
            GameObject go = new GameObject();
            go.transform.parent = transform;
            go.transform.position = transform.position;
            air = go;
            air.AddComponent<PlayerAirMovement>();
            airComponent = air.GetComponent<PlayerAirMovement>();
            air.name = "air";
        }

        playerMovementArray[3] = air;

        //for (int i = 0; i < playerMovementArray.Length; i++)
        //{
        //    Debug.Log(playerMovementArray[i].name + " is in the player movement array");
        //}


        SetMovementType(MovementType.move);
    }
    
    //DEBUGGING GRAPPLE
    public LineRenderer debugLine;

    //genereric addforce fuctions that can be called by anything
    public void GenericAddForce(Vector3 direction, float force)
    {
        rb.AddForce(direction * force, ForceMode.Impulse);
    }

   
}