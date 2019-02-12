using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Build : MonoBehaviour
{
    public GameObject buildingOne;
    public GameObject buildingTwo;

    Vector3 location1;
    Vector3 location2;

    public float buildTimeOne;
    public float buildTimeTwo;

    public int whichBuild;

    private bool isFinishedOne;
    private bool isFinishedTwo;


    private bool hasClicked;
    private bool hasClickedtwo;

    private bool canBuildOne;
    private bool canBuildTwo;

    Queue<GameObject> buildQueue;


    private void Start()
    {
        buildQueue = new Queue<GameObject>();
        whichBuild = 0;
        hasClicked = false;
        hasClickedtwo = false;
        canBuildOne = true;
        canBuildTwo = true; 
    }


    private void Update()
    {

   

        selectBuild();
        build();
        startTimerOne();
        startTimerTwo();


    }

    private void selectBuild()
    {
        if (whichBuild == 1)
        {
            if (hasClicked == false)
            {

                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hitInfo = new RaycastHit();
                    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                    if (hit)
                    {
                        location1 = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                        hasClicked = true;

                    }



                }
            }
        }

        else if (whichBuild == 2)
        {
            if (hasClickedtwo == false)
            {
                if (Input.GetMouseButtonDown(0))
                {
                    RaycastHit hitInfo = new RaycastHit();
                    bool hit = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hitInfo);
                    if (hit)
                    {
                        location2 = new Vector3(hitInfo.point.x, hitInfo.point.y, hitInfo.point.z);
                        hasClickedtwo = true;


                    }


                }

            }
        }
    }

    public void buttonOne()
    {
        whichBuild = 1;
        buildQueue.Enqueue(buildingOne);

        Debug.Log("Q length is " + buildQueue.Count);
    }
    public void buttonTwo()
    {
        whichBuild = 2;
        buildQueue.Enqueue(buildingTwo);

        Debug.Log("Q length is " + buildQueue.Count);

    }

    public void startTimerOne()
    {
        if (hasClicked &&  canBuildOne && buildQueue.Peek() == buildingOne)
        {
            buildTimeOne -= Time.deltaTime;
            if (buildTimeOne < 0)
            {
                isFinishedOne = true;
                buildQueue.Dequeue();
                canBuildOne = false; 

            }
        }

        //else if(hasClicked  && buildQueue.Count == 0)
        //{
        //    Debug.Log("building one is no longer in the que");
        //}
    }
    public void startTimerTwo()
    {
        if (hasClickedtwo && canBuildTwo && buildQueue.Peek() == buildingTwo)
        {
            buildTimeTwo -= Time.deltaTime;
            if (buildTimeTwo < 0)
            {
                isFinishedTwo = true;
                buildQueue.Dequeue();
                canBuildTwo = false;

            }
        }


    }


    void build()
    {
        if (isFinishedOne)
        {

            GameObject newBuild = Instantiate(buildingOne, location1, Quaternion.identity);
            isFinishedOne = false; 
        }

        else if (isFinishedTwo)
        {
            GameObject newBuild = Instantiate(buildingTwo, location2, Quaternion.identity);

            isFinishedTwo = false; 
        }
      
    }
}




