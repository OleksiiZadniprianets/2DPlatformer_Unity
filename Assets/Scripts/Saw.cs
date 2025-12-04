using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Saw : MonoBehaviour
{
    public float speed;

    // Update is called once per frame
    void Update()
    {
        float angle = speed * Time.deltaTime;
        transform.Rotate(Vector3.forward, angle);
    }
}
