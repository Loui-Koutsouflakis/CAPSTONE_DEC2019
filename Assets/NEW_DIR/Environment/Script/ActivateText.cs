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
            gameText.transform.position = Vector3.Lerp(gameText.transform.position, player.transform.position + camera.transform.forward.normalized * 1 + camera.transform.right.normalized * 2 + camera.transform.up.normalized * 1.5f, Time.deltaTime * 100);
            sign.transform.position = Vector3.Lerp(sign.transform.position, player.transform.position + camera.transform.forward.normalized * 1 + camera.transform.right.normalized * 2 + camera.transform.up.normalized * 1.5f, Time.deltaTime * 100);

            //gameText.transform.position = Vector3.Lerp(gameText.transform.position, camera.transform.position + camera.transform.forward.normalized * 3 + camera.transform.right.normalized * 2 + camera.transform.up.normalized * 1, Time.deltaTime * 100);
            //sign.transform.position = Vector3.Lerp(sign.transform.position, camera.transform.position + camera.transform.forward.normalized * 3 + camera.transform.right.normalized * 2 + camera.transform.up.normalized * 1, Time.deltaTime * 100);
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
                gameText.transform.position = camera.transform.position + camera.transform.forward.normalized * 3 + camera.transform.right.normalized * 2 + camera.transform.up.normalized * 1;
                sign.transform.position = camera.transform.position + camera.transform.forward.normalized * 3 + camera.transform.right.normalized * 2 + camera.transform.up.normalized * 1;
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
