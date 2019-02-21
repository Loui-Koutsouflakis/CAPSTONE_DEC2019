using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallJump : MonoBehaviour
{
    public float jumpForce;

    private bool onWall;
    private float gravity;

    private RaycastHit hit;
    private Rigidbody rb;
    private Vector3 half = new Vector3(0.34f, 0.385f, 0.34f);

    private void Jump()
    {
        if (onWall)
        {
            rb.AddForce(new Vector3(0, jumpForce + rb.velocity.magnitude / 3), ForceMode.Impulse);
            transform.position = -transform.position;
        }
    }

    private void OnCollisionStay(Collision col)
    {
        if (col.gameObject.tag == "Ground" || col.gameObject.tag == "MovingPlatform")
        {
            if (Physics.BoxCast(rb.position, half, Vector3.back, out hit, Quaternion.identity, half.y) ||
                Physics.BoxCast(rb.position, half, Vector3.forward, out hit, Quaternion.identity, half.y) ||
                Physics.BoxCast(rb.position, half, Vector3.left, out hit, Quaternion.identity, half.y) ||
                Physics.BoxCast(rb.position, half, Vector3.right, out hit, Quaternion.identity, half.y))
            {
                Physics.gravity = new Vector3(0, -2, 0);
                onWall = true;

                if (Input.GetButtonDown("Jump"))
                {
                    Jump();
                }
            }
        }
    }

    private void OnCollisionExit(Collision col)
    {
        Physics.gravity = new Vector3(0, -gravity, 0);
    }
}
