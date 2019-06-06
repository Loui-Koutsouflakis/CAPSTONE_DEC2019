//Written By Michael Elkin 04/18/2019
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PFS_Trigger : MonoBehaviour
{
    public enum PlayerTriggerTypes { LevelProgression, Cinematics, Boundry, KillBox }
    [SerializeField]
    PlayerTriggerTypes _triggerType;
    public PlayerTriggerTypes triggerType
    {
        get { return _triggerType; }
        set { _triggerType = value; }
    }

    BoxCollider thisBoxCollider;// Progression and Boundry and KillBox Collider
    SphereCollider thisSpherecollider;// Cinematics Collider
    Bounce_Back bounce_Back;//KillBox Additive Script

    [SerializeField, Range(0, 100), Header("Box Collider Dimensions")]
    float height = 1.0f;
    [SerializeField, Range(0, 100)]
    float width = 1.0f;
    [SerializeField, Range(0, 100)]
    float depth = 1.0f;

    [SerializeField, Range(1, 10), Header("Sphere Collider Radius")]
    float colliderRadius = 1.0f;

    Vector3 colliderSize;

    [SerializeField]
    bool liveEdit;
    [SerializeField]
    bool resetTrigger;

    private void Awake()
    {
        colliderSize = new Vector3(width, height, depth);
        SetTriggerCollider();

    }

    private void Update()
    {
        LiveTiggerEdit();
        ResetTrigger();
    }
    void OnCollisionEnter(Collision c)
    {
        switch (triggerType)
        {
            case PlayerTriggerTypes.LevelProgression:
                // Nothing Happens
                break;
            case PlayerTriggerTypes.Cinematics:
                // Nothing Happens
                break;
            case PlayerTriggerTypes.Boundry:
                if (c.gameObject.tag == "Player")
                {
                    Debug.Log("Player Hit Level Boundry");
                }
                //Player goes no further
                break;
            case PlayerTriggerTypes.KillBox:
                if (c.gameObject.tag == "Player")
                {
                    c.gameObject.GetComponent<Bounce_Back>().Rebound(c.gameObject);
                    Debug.Log("Player Hit Rock Bottom");
                }
                // Bounce back function to be added later
                break;
        }
    }
    void OnTriggerEnter(Collider o)
    {
        switch (triggerType)
        {
            case PlayerTriggerTypes.LevelProgression:
                if(o.gameObject.tag == "Player")
                {
                    // Enter Save Function Here
                    Debug.Log("Player Saved");
                }
                break;
            case PlayerTriggerTypes.Cinematics:
                if (o.gameObject.tag == "Player")
                {
                    // Enter Cinematics Function Here
                    Debug.Log("Cinematic Plays");
                }
                break;
            case PlayerTriggerTypes.Boundry:
                // Nothing Happens
                break;
            case PlayerTriggerTypes.KillBox:
                // Nothing Happens
                break;
        }
    }
    void SetTriggerCollider()// Sets Up Colliders
    {        
        switch (triggerType)
        {
            case PlayerTriggerTypes.LevelProgression:
                thisBoxCollider = gameObject.AddComponent<BoxCollider>();
                thisBoxCollider.size = colliderSize;
                thisBoxCollider.isTrigger = true;
                break;
            case PlayerTriggerTypes.Cinematics:
                thisSpherecollider = gameObject.AddComponent<SphereCollider>();
                thisSpherecollider.radius = colliderRadius;
                thisSpherecollider.isTrigger = true;
                break;
            case PlayerTriggerTypes.Boundry:
                thisBoxCollider = gameObject.AddComponent<BoxCollider>();
                thisBoxCollider.size = colliderSize;
                thisBoxCollider.isTrigger = false;
                break;
            case PlayerTriggerTypes.KillBox:
                thisBoxCollider = gameObject.AddComponent<BoxCollider>();
                //bounce_Back = gameObject.AddComponent<Bounce_Back>();
                thisBoxCollider.size = colliderSize;
                thisBoxCollider.isTrigger = false;
                break;

        }
    }
    void LiveTiggerEdit()
    {
        if (liveEdit)
        {
            switch (triggerType)
            {
                case PlayerTriggerTypes.LevelProgression:
                    if(thisBoxCollider == null)
                    {
                        SetTriggerCollider();
                    }
                    IfDimensionChange();                    
                    break;
                case PlayerTriggerTypes.Cinematics:
                    if(thisSpherecollider == null)
                    {
                        SetTriggerCollider();
                    }
                    IfDimensionChange();
                    break;
                case PlayerTriggerTypes.Boundry:
                    if (thisBoxCollider == null)
                    {
                        SetTriggerCollider();
                    }
                    IfDimensionChange();                    
                    break;
                case PlayerTriggerTypes.KillBox:
                    if (thisBoxCollider == null)
                    {
                        SetTriggerCollider();
                    }
                    IfDimensionChange();
                    break;

            }
        }
    }
    void ResetTrigger()
    {
        if(resetTrigger)
        {
            if (gameObject.GetComponent<BoxCollider>())
            {
                Destroy(thisBoxCollider);
            }
            if (gameObject.GetComponent<SphereCollider>())
            {
                Destroy(thisSpherecollider);
            }
            //SetTriggerCollider();
            resetTrigger = false;
        }
    }
    void IfDimensionChange()
    {
        switch (triggerType)
        {
            case PlayerTriggerTypes.LevelProgression:
                if (colliderSize.x != width || colliderSize.y != height || colliderSize.z != depth)
                {
                    colliderSize.x = width;
                    colliderSize.y = height;
                    colliderSize.z = depth;
                    thisBoxCollider.size = colliderSize;
                }
                break;
            case PlayerTriggerTypes.Cinematics:
                if (colliderRadius != thisSpherecollider.radius)
                {
                    thisSpherecollider.radius = colliderRadius;
                }
                break;
            case PlayerTriggerTypes.Boundry:
                if (colliderSize.x != width || colliderSize.y != height || colliderSize.z != depth)
                {
                    colliderSize.x = width;
                    colliderSize.y = height;
                    colliderSize.z = depth;
                    thisBoxCollider.size = colliderSize;
                }
                break;
            case PlayerTriggerTypes.KillBox:
                if (colliderSize.x != width || colliderSize.y != height || colliderSize.z != depth)
                {
                    colliderSize.x = width;
                    colliderSize.y = height;
                    colliderSize.z = depth;
                    thisBoxCollider.size = colliderSize;
                }
                break;

        }
    }
}
