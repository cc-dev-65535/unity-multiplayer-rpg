using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingCamera : MonoBehaviour
{
    public Transform target;

    public float rotSpeed = 1.5f;
    private float rotY;
    private Vector3 offset;

    // Start is called before the first frame update
    void Start()
    {
        //target = GameObject.Find("player Variant(Clone)").GetComponent<Transform>();
        target = transform.parent.GetComponent<Transform>();
        rotY = transform.eulerAngles.y;
        offset = target.position - transform.position;
    }

    // LateUpdate is called once per frame after all Updates for all objects are finished
    void LateUpdate()
    {
        float horInput = Input.GetAxis("Horizontal");
        if (!Mathf.Approximately(horInput, 0))
        {
            rotY += horInput * rotSpeed;
        }
        else
        {
            rotY += Input.GetAxis("Mouse X") * rotSpeed * 3;
        }

        Quaternion rotation = Quaternion.Euler(0, rotY, 0);
        transform.position = target.position - (rotation * offset);
        transform.LookAt(target);
    }
}
