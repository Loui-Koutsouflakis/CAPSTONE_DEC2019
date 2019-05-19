using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    LevelObjectPools levelObjectPools;

    PFS_Trigger _currentSavePoint;
    public PFS_Trigger currenSavePoint
    {
        get { return _currentSavePoint; }
        private set { _currentSavePoint = value; }
    }

    [SerializeField]
    GameObject playerRef;// Player Object

    




    //[SerializeField]
    //LayerMask interActObjLayer;


    void Awake()
    {
        playerRef = GameObject.FindGameObjectWithTag("Player");// Insert Player Class
        levelObjectPools = GameObject.FindObjectOfType<LevelObjectPools>();
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCurrentSpawnPoint(PFS_Trigger newSavePoint)
    {
        if (currenSavePoint != newSavePoint)
        {
            currenSavePoint = newSavePoint;
        }
    }
    void RepositionPlayer(Vector3 newPosition)
    {
        playerRef.GetComponent<HitBox>().TakeDamage(1);
        playerRef.transform.position = newPosition;

    }

    public GameObject DeactivateObject(GameObject objToTurnOff)
    {
        if (objToTurnOff.activeSelf == true)
        {
            objToTurnOff.SetActive(false);
        }
        return objToTurnOff;
    }

}
