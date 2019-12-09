using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActivateText : MonoBehaviour
{
    private GameObject gameText;
    private PlayerClass player;
    private PlayerCamera camera;
    public GameObject sign;
    // Start is called before the first frame update
    void Start()
    {
        gameText = GetComponentInChildren<TextMesh>().gameObject;
        gameText.SetActive(false);
        sign.SetActive(false);
        player = FindObjectOfType<PlayerClass>();
        camera = FindObjectOfType<PlayerCamera>();
    }

    // Update is called once per frame
    void Update()
    {
        if(gameText.activeSelf)
        {
            gameText.transform.position = camera.transform.position + camera.transform.forward.normalized * 5 + camera.transform.right.normalized * 2 + camera.transform.up.normalized * 2;
            sign.transform.position = camera.transform.position + camera.transform.forward.normalized * 6 + camera.transform.right.normalized * 2 + camera.transform.up.normalized * 2;
            gameText.transform.forward = Vector3.Lerp(gameText.transform.forward, camera.transform.forward, Time.deltaTime * 10);
            sign.transform.forward = Vector3.Lerp(sign.transform.forward, -camera.transform.forward, Time.deltaTime * 10);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.layer == 14)
        {
            if(!gameText.activeSelf)
            {
                gameText.SetActive(true);
                sign.SetActive(true);
                sign.transform.forward = -camera.transform.forward;
                StartCoroutine(DisableText());
            }
        }
    }

    IEnumerator DisableText()
    {
        yield return new WaitForSeconds(3);
        gameText.SetActive(false);
        sign.SetActive(false);
    }
}
