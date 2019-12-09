using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//If we have a stiff rope, such as a metal wire, then we need a simplified solution
//this is also an accurate solution because a metal wire is not swinging as much as a rope made of a lighter material
public class RopeController : MonoBehaviour
{

    [Range(-1, 1)]
    public float upperControlPointOffset = 0.1f;

    [Range(-1, 1)]
    public float lowerControlPointOffset = -0.5f;

    public PlayerClass player;
    //Objects that will interact with the rope
    public Transform whatTheRopeIsConnectedTo;
    public Transform whatIsHangingFromTheRope;

    //Line renderer used to display the rope
    private LineRenderer lineRenderer;

    //A list with all rope sections
    public List<Vector3> allRopeSections = new List<Vector3>();


    //Mass of what the rope is carrying
    private float loadMass = 100f;
    //How fast we can add more/less rope
    float winchSpeed = 1f;

    float ropeWidth = 0.05f;
    public bool beTwisty = true;

    void Start()
    {

        //Init the line renderer we use to display the rope
        lineRenderer = GetComponent<LineRenderer>();

        //Init the spring we use to approximate the rope from point a to b


        //Add the weight to what the rope is carrying
    }



    void Update()
    {

        if (beTwisty)
            DisplayRope();

        else
        {
            lineRenderer.startWidth = ropeWidth;
            lineRenderer.endWidth = ropeWidth;
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, whatTheRopeIsConnectedTo.transform.position);
            lineRenderer.SetPosition(1, whatIsHangingFromTheRope.position);

        }
    }

    private void DisplayRope()
    {
        //This is not the actual width, but the width use so we can see the rope


        lineRenderer.startWidth = ropeWidth;
        lineRenderer.endWidth = ropeWidth;


        //Update the list with rope sections by approximating the rope with a bezier curve
        //A Bezier curve needs 4 control points
        Vector3 A = whatTheRopeIsConnectedTo.position;
        Vector3 D = whatIsHangingFromTheRope.position;

        //Upper control point
        //To get a little curve at the top than at the bottom
        Vector3 B = A + whatTheRopeIsConnectedTo.up * (-(A - D).magnitude) * upperControlPointOffset;
        //B = A;

        //Lower control point
        Vector3 C = D + whatIsHangingFromTheRope.up * ((A - D).magnitude) * lowerControlPointOffset;

        //Get the positions
        BezierCurve.GetBezierCurve(A, B, C, D, allRopeSections);


        //An array with all rope section positions
        Vector3[] positions = new Vector3[allRopeSections.Count];

        for (int i = 0; i < allRopeSections.Count; i++)
        {
            positions[i] = allRopeSections[i];
        }

        //Just add a line between the start and end position for testing purposes
        //Vector3[] positions = new Vector3[2];

        //positions[0] = whatTheRopeIsConnectedTo.position;
        //positions[1] = whatIsHangingFromTheRope.position;


        //Add the positions to the line renderer
        lineRenderer.positionCount = positions.Length;

        lineRenderer.SetPositions(positions);
    }


}