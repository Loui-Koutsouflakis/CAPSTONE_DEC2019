using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pumpkin : Crop
{
    private float pScore;

    private void Awake()
    {
        alert.SetActive(false);
    }
    // Start is called before the first frame update
    void Start()
    {
        health = 100;
        points = 50;

        cropTag.Add(tag1);
        cropTag.Add(tag2);
        cropTag.Add(tag4);
        cropTag.Add(tag3);
        cropArray = cropTag.ToArray();
        Debug.Log(cropArray.Length);
        pScore = player.GetComponent<Score>().playerScore;



    }

    // Update is called once per frame
    void Update()
    {
        if (crowEating)
        {
            alert.SetActive(true);
            if (health > 0)
            {

                health = health - Time.deltaTime;
                //Debug.Log(health);
                pScore = pScore - (health * 0.01f);
                player.GetComponent<Score>().playerScore = pScore;
                //Debug.Log("Player Score is " + pScore);

            }

            else
            {
                alert.SetActive(false);
            }


        }
        if (health <= 0)
        {
            Destroy(gameObject);
        }


    }

}

