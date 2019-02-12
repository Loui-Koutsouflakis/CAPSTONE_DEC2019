using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crow : MonoBehaviour
{
    public float eatingRange;
    //public float angle;

    //public Crop food; 

    Crop currentCrop;
    bool canFindCrop;

    public Material crowColor;
    public Animator AIControl;


    //public Animator AniCrow; 


    bool hitPlayer; 
    public GameObject player;


    private Vector3 tagVector; 

    private bool playerNear;
    private bool flyAway;
    private float circleRandom; 
    private bool isGrounded;
    public bool canCircle;
    public bool canHeadToCrop; 


    //circle
    public float circeSpeed;
    private float timer; 
    float timeC;
    float radious; 


    // Start is called before the first frame update
    void Start()
    {
        canHeadToCrop = false; 
        timer = 6; 
        circleRandom = UnityEngine.Random.Range(0, 5);
        Debug.Log(circleRandom);
        canCircle = true; 
        hitPlayer = false; 
        timeC = 0; 
        flyAway = false; 
        isGrounded = false; 
        //finds all the crops



        if(GetComponent<Material>())
        {
            crowColor = GetComponent<Material>();
        }
        else
        {
            Debug.Log("Crow is colorless");
        }

        
        if(GetComponent<Animator>())
        {
            AIControl = GetComponent<Animator>();
        }
        else
        {
            Debug.Log("No Animator on Crow");
        }

        if (currentCrop == null)
        {
            FindCrop();
        }


        //AniCrow.SetBool("flying", true);
    }

    // Update is called once per frame
    void Update()
    {
    
        if(canFindCrop)
        {
            AIControl.SetBool("CanFindCrop", true);
            AIControl.SetBool("canCircle", false);
            Circle();
            //HeadingToCrop();
        }

        if (canHeadToCrop)
        {
            HeadingToCrop();
        }

        //player range
        
        Vector3 toPlayer = transform.position - player.transform.position;
        playerNear = toPlayer.magnitude < eatingRange; 

        if(playerNear)
        {
            flyAway = true; 

        }


        //eating range
        if(currentCrop != null)
        {
        Vector3 toCrop = transform.position - currentCrop.transform.position;
        bool nearCrop = toCrop.magnitude < eatingRange;
        

        if (nearCrop)
        {
            if(isGrounded)
            {
                   
                canHeadToCrop = false; 
            //Debug.Log("Crow is eating");
            EatingCrop();
            }
        }
        }
        else
        {
            timer = 10; 
            FindCrop();
        }

        FightOrFlight();
    }


    public void FindCrop()
    {
        if (currentCrop == null)
        {
            GameObject[] allCrops = GameObject.FindGameObjectsWithTag("Crop");
            if (allCrops.Length > 0)
            {
                int random = UnityEngine.Random.Range(0, allCrops.Length);
                Crop startCrop = allCrops[random].GetComponent<Crop>();
                if (startCrop != null)
                {
                    currentCrop = startCrop;

                    if(currentCrop.cropArray.Length > 0)
                    {
                        int random2 = UnityEngine.Random.Range(0, currentCrop.cropArray.Length);
                    tagVector = currentCrop.cropArray[random2].transform.position;
                    }
                    else
                    {
                        Debug.Log("Crow cannot find target on crop");
                    }


                    canFindCrop = true; 
                   
                }
            }
            else
            {
                Debug.Log("There are no Crops left");
                Destroy(gameObject);
            }
        }
        else
        {
            Circle();
        }
    }


    public void Circle()
    {

        
        // timeC += Time.deltaTime * circeSpeed;
        // Vector3 origin = new Vector3(0, 0, 0) - transform.position;
        //radious = origin.magnitude;
        //float rx = Mathf.Cos(timeC);
        //float rz = Mathf.Sin(timeC);
        //transform.position = new Vector3(rx, 20, rz);
        Vector3 start = transform.position;
        Vector3 headingAway = new Vector3(0, 20, transform.forward.z + 50);
        transform.position = Vector3.Lerp(start, headingAway, 0.1f);
         
       
            timer = timer - Time.deltaTime;
            //Debug.Log(timer);
            if(timer < circleRandom)
                {
                    canHeadToCrop = true; 
                }
            



    }

    public void HeadingToCrop()
    {
        //AniCrow.SetBool("flying", false);
        Vector3 start = transform.position;
        if (currentCrop != null)
        {
            canFindCrop = false; 
            AIControl.SetBool("CanFindCrop", true);
            AIControl.SetBool("canCircle", false);



            //Vector3 headingToCrop = currentCrop.transform.position - new Vector3(2, 0, 2);
            transform.position = Vector3.Lerp(start, tagVector, 0.01f);
            
        }
        else
        {
            Debug.LogWarning("Can't find crop");
            FindCrop();
        }
    }

    public void EatingCrop()
    {

        if (currentCrop == null)
        {

            FindCrop();
        }

        else
        {
            //AniCrow.SetBool("Pecking", true);
            currentCrop.crowEating = true;
        }

    }

    void ResetTimer()
    {
        circleRandom = UnityEngine.Random.Range(0, 5);
        timer = 10; 
    }


    private void FightOrFlight()
    {
        if(flyAway)
        {
        if(isGrounded)
        {
                currentCrop.crowEating = false; 
            canFindCrop = false;
                Vector3 start = transform.forward;
            Vector3 headingAway = new Vector3(0, 20, transform.forward.z + 50);
            transform.position = Vector3.Lerp(start, headingAway, 0.01f);
            currentCrop.crowEating = false;
            Vector3 toDestruc = transform.position - headingAway;
            bool canDestroy = toDestruc.magnitude < 3;
            if(canDestroy)
            {
                Destroy(gameObject);
            }
           


        }
        else
        {
                if(hitPlayer)
                {
                    canFindCrop = true; 
                    Vector3 start = transform.forward;
                    Vector3 headingTowards = player.transform.position; 
                    transform.position = Vector3.Lerp(start, headingTowards, 0.1f);
                    Debug.Log("Attacking player");
                    hitPlayer = false; 
                }

            }
        }

    }

   

   

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            isGrounded = true;
            Debug.Log("Crow has landed on the ground");

        }

        if(collision.gameObject.tag == "Player")
        {
            if(!isGrounded)
            {
                Debug.Log("Hit player");
                hitPlayer = true;
                canFindCrop = false; 
                Circle();
            }
        }
    }
   





}
