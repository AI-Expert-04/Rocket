using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionTest : MonoBehaviour
{
    public Renderer renderer;

    void OnCollisionEnter(Collision collision)
    {
        if (collision.relativeVelocity.y > 10f)
        {
            renderer.material.color = Color.red;
        }
        else
        {
            renderer.material.color = Color.green;
        }
    }
}
