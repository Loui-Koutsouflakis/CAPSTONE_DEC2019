using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildQueue : MonoBehaviour {

    public GameObject Marine;
    public GameObject Firebat;

    private GameObject temp;

    float p_speed = 6;
    float p_Accel = 3;
    float p_MS = 6;
    public float timer;
    public float timer2;
    public float BTM = 3;
    public float BTF = 5;

    public bool m = false;
    public bool n = false;
    private Rigidbody body;
    public Queue<GameObject> buildQ;
    // Use this for initialization
    void Start() {
        buildQ = new Queue<GameObject>();
        body = GetComponent<Rigidbody>();
    }

    private void FixedUpdate()
    {
        //movement script
        float m_LR = Input.GetAxis("Horizontal");
        float m_FB = Input.GetAxis("Vertical");

        Vector3 movement = new Vector3(m_LR, 0, m_FB);
        body.AddForce(movement * p_speed * p_Accel);
        if (body.velocity.magnitude > p_MS)
        {
            body.velocity = body.velocity.normalized * p_MS;
        }
    }
    // Update is called once per frame
    void Update()
    {
        {
            if (Input.GetKey(KeyCode.E))
            { 
                body.AddForce(0, 30f, 15f);
            }
            if (Input.GetKey(KeyCode.Q))
            {
                body.AddForce(0, -30f, 0f);
            }
            #region spawn Marine

            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                m = true;
                Debug.Log(gameObject + ("building marine"));
            }
            if (m == true)
            {
                temp = Marine;
                buildQ.Enqueue(temp);
                timer += Time.deltaTime;
                if (timer >= BTM)
                {
                    temp = Instantiate(Marine, transform.position, Quaternion.identity);
                    Debug.Log(gameObject + ("done building marine"));
                    emptyQueue();
                    Debug.Log(buildQ.Count);
                    m = false;
                    timer = 0;
                    GameObject.Destroy(temp, 2);
                }
            }
            #endregion
            #region spawn Firebat

            if (Input.GetKeyDown(KeyCode.Alpha2))
            {
                n = true;
                Debug.Log(gameObject + ("building Firebat"));
            }
            if (n == true)
            {

                temp = Firebat;
                buildQ.Enqueue(temp);
                timer2 += Time.deltaTime;
                if (timer2 > BTF)
                {
                    temp = Instantiate(Firebat, transform.position, Quaternion.identity);
                    Debug.Log(gameObject + ("done building firebat"));
                    Debug.Log(buildQ.Count);
                    emptyQueue();
                    n = false;
                    timer2 = 0;
                    GameObject.Destroy(temp, 3);
                }
            }
            #endregion
        }
    }
        private void emptyQueue()
    {
        GameObject[] tmp = new GameObject[buildQ.Count];
        buildQ.CopyTo(tmp, 0);
    }
}
