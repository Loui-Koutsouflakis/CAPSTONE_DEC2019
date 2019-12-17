using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HudManager : MonoBehaviour
{
    public static int shardsCollected;
    public static int moonsCollected;
    public MeshRenderer plateRenderer;
    public MeshRenderer[] healthRenderers;
    public MeshRenderer[] shardOnes;
    public MeshRenderer[] shardTens;
    public MeshRenderer[] shardHundreds;
    public MeshRenderer[] moonOnes;
    public MeshRenderer[] moonTens;
    public Animator[] healthAnims;
    public Animator shardsAnim;
    public Animator moonsAnim;
    public Transform pieTf;
    public Vector3 localHide;
    public Vector3 localVisible;
    bool canShow;
    bool showing;
    bool waiting;
    bool hiding;
    public static int health = 3;
    int maxHealth = 3;
    const float showTime = 3f;
    const float hudLerp = 8.6f;

    string shardString = "000";
    string moonString = "000";

    //Tie in to Player health
    PlayerClass p_Class;
    void Start()
    {
        

        canShow = true;
        pieTf.localPosition = localHide;
        //p_Class = GameObject.FindObjectOfType<PlayerClass>();

        foreach(MeshRenderer mr in shardOnes)
        {
            mr.enabled = false;
        }
        foreach (MeshRenderer mr in shardTens)
        {
            mr.enabled = false;
        }
        foreach (MeshRenderer mr in shardHundreds)
        {
            mr.enabled = false;
        }

        foreach(MeshRenderer mr in moonOnes)
        {
            mr.enabled = false;
        }
        foreach(MeshRenderer mr in moonTens)
        {
            mr.enabled = false;
        }

        shardOnes[0].enabled = true;
        shardTens[0].enabled = true;
        shardHundreds[0].enabled = true;
        moonOnes[0].enabled = true;
        moonTens[0].enabled = true;
    }

    private void Update()
    {
        if(showing)
            pieTf.localPosition = Vector3.Lerp(pieTf.localPosition, localVisible, hudLerp * Time.deltaTime);

        else if(hiding)
            pieTf.localPosition = Vector3.Lerp(pieTf.localPosition, localHide, hudLerp * Time.deltaTime);
        
        //if (Input.GetKeyDown(KeyCode.L))
        //    HealthUp();

        //if(Input.GetKeyDown(KeyCode.K))
        //    HealthDown();

        //if(Input.GetKeyDown(KeyCode.N))
        //    ShardsUp();

        //if (Input.GetKeyDown(KeyCode.M))
        //    MoonsUp();

        //health = p_Class.GetHealth();
    }

    public void HudButtonDown()
    {
        if (canShow)
        {
            StartCoroutine(ShowHud());
        }
        else if (waiting)
        {
            StopAllCoroutines();
            StartCoroutine(HideHud());
        }
    }

    public void ShardsUp()
    {
        StopAllCoroutines();
        StartCoroutine(ShowHud());

        shardsCollected++;

        shardString = shardsCollected.ToString();

        foreach (MeshRenderer mr in shardOnes)
        {
            mr.enabled = false;
        }
        foreach (MeshRenderer mr in shardTens)
        {
            mr.enabled = false;
        }
        foreach (MeshRenderer mr in shardHundreds)
        {
            mr.enabled = false;
        }

        if (shardString.Length == 3)
        {
            shardHundreds[shardString[0] - 48].enabled = true;
            shardTens[shardString[1] - 48].enabled = true;
            shardOnes[shardString[2] - 48].enabled = true;
        }
        else if (shardString.Length == 2)
        {
            shardString = "0" + shardString;
            shardHundreds[0].enabled = true;
            shardTens[shardString[1] - 48].enabled = true;
            shardOnes[shardString[2] - 48].enabled = true;
        }
        else if (shardString.Length == 1)
        {
            shardString = "00" + shardString;
            shardHundreds[0].enabled = true;
            shardTens[0].enabled = true;
            shardOnes[shardsCollected].enabled = true;
        }
    }

    public void SetShards()
    {
        shardString = shardsCollected.ToString();

        foreach (MeshRenderer mr in shardOnes)
        {
            mr.enabled = false;
        }
        foreach (MeshRenderer mr in shardTens)
        {
            mr.enabled = false;
        }
        foreach (MeshRenderer mr in shardHundreds)
        {
            mr.enabled = false;
        }

        if (shardString.Length == 3)
        {
            shardHundreds[shardString[0] - 48].enabled = true;
            shardTens[shardString[1] - 48].enabled = true;
            shardOnes[shardString[2] - 48].enabled = true;
        }
        else if (shardString.Length == 2)
        {
            shardString = "0" + shardString;
            shardHundreds[0].enabled = true;
            shardTens[shardString[1] - 48].enabled = true;
            shardOnes[shardString[2] - 48].enabled = true;
        }
        else if (shardString.Length == 1)
        {
            shardString = "00" + shardString;
            shardHundreds[0].enabled = true;
            shardTens[0].enabled = true;
            shardOnes[shardsCollected].enabled = true;
        }
    }

    public static void ResetShards()
    {
        shardsCollected = 0;      
    }

    public static void ResetMoons()
    {
        moonsCollected = 0;      
    }

    public int GetShards()
    {
        return shardsCollected;
    }

    public void MoonsUp()
    {
        StopAllCoroutines();
        StartCoroutine(ShowHud());
        
        moonsAnim.SetTrigger("Bounce");

        if (moonsCollected < 99)
        {
            moonsCollected++;

            moonString = moonsCollected.ToString();

            foreach (MeshRenderer mr in moonOnes)
            {
                mr.enabled = false;
            }
            foreach (MeshRenderer mr in moonTens)
            {
                mr.enabled = false;
            }

            if (moonString.Length == 1)
            {
                moonString = "0" + moonString;
                moonTens[0].enabled = true;
                moonOnes[moonsCollected].enabled = true;
            }
            else if (moonString.Length == 2)
            {
                moonTens[moonString[0] - 48].enabled = true;
                moonOnes[moonString[1] - 48].enabled = true;
            }
        }
    }

    public void HealthDown()
    {
        StopAllCoroutines();
        StartCoroutine(ShowHud());

        if (health > 0)
        {
            //ANIM HERE
            //healthRenderers[health - 1].enabled = false;
            healthAnims[health - 1].SetTrigger("Lose");
            health--;
            //p_Class.SetHealth(-1);
        }
    }

    public void HealthUp()
    {
        StopAllCoroutines();
        StartCoroutine(ShowHud());

        if (health < 3)
        {
            //ANIM HERE
            //healthRenderers[health].enabled = true;
            healthAnims[health].SetTrigger("Gain");
            health++;
            //p_Class.SetHealth(1);
        }
    }

    public void HealthZero()
    {
        StopAllCoroutines();
        StartCoroutine(ShowHud());

        health = 0;

        //foreach(MeshRenderer mr in healthRenderers)
        //{
        //    mr.enabled = false;
        //}

        foreach (Animator anim in healthAnims)
        {
            anim.SetTrigger("Lose");
        }
    }

    public void HealthFull()
    {
        StopAllCoroutines();
        StartCoroutine(ShowHud());

        health = 3;

        //foreach (MeshRenderer mr in healthRenderers)
        //{
        //    mr.enabled = true;
        //}

        foreach(Animator anim in healthAnims)
        {
            anim.SetTrigger("Gain");
        }
    }

    public IEnumerator ShowHud()
    {
        //plateRenderer.enabled = true;

        //foreach(MeshRenderer mr in healthRenderers)
        //{
        //    mr.enabled = true;
        //}

        canShow = false;
        showing = true;

        yield return new WaitForSeconds(1.5f);

        showing = false;
        waiting = true;

        yield return new WaitForSeconds(1.6f);

        if (waiting)
        {
            StartCoroutine(HideHud());
        }
    }

    public IEnumerator HideHud()
    {
        waiting = false;
        hiding = true;

        yield return new WaitForSeconds(0.8f);

        //plateRenderer.enabled = false;

        //foreach (MeshRenderer mr in healthRenderers)
        //{
        //    mr.enabled = false;
        //}

        hiding = false;
        canShow = true;
    }
}
