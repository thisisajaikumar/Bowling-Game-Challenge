using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pin : MonoBehaviour
{
    private bool _done;

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.CompareTag("Ball"))
        {
            Ball.Instance.VFX();
        }

        if (collision.collider.CompareTag("Pin") && !_done)
        {
            float velocity = GetComponent<Rigidbody>().velocity.magnitude;
            if (velocity < 10)
            {
                Ball ball = Ball.Instance;
                var point = ball.Point;
                point += 1;
                ball.Point = point;
                _done = true;
            }
        }
    }
}