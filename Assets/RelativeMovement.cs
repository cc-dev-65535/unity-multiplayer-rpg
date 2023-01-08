using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
//using System.Security.Cryptography;
using System.Collections.Specialized;
using System;
using System.Diagnostics;

public class RelativeMovement : NetworkBehaviour
{
    public GameObject cameraPrefab;
    private GameObject cameraOrbit;

    //private Transform target;

    public float rotSpeed = 15.0f;
    public float moveSpeed = 150.0f;

    public NetworkVariable<Vector3> Position = new NetworkVariable<Vector3>();
    private CharacterController charController;

    public override void OnNetworkSpawn()
    {
        //transform.position = new Vector3(3f, 3f, 3f);
        charController = GetComponent<CharacterController>();

        if (NetworkManager.Singleton.IsClient)
        {
            cameraOrbit = Instantiate(cameraPrefab, this.transform) as GameObject;
            GameObject.Find("World Camera").GetComponent<Camera>().enabled = false;
            // THIS DOESN'T WORK WHY??? TODO***
            //SubmitPositionRequestServerRpc(transform.position + new Vector3(0f, 10f, 0f));
        }
        else
        {
            Position.Value = new Vector3(0f, 3f, 0f);
        }
        //(camera as Camera).enabled = true;
        //target = camera.transform;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    public Vector3 GetMovementPosition()
    {
        Vector3 movement = Vector3.zero;
        float horInput = Input.GetAxis("Horizontal");
        //UnityEngine.Debug.Log("horInput is: " + horInput);
        float vertInput = Input.GetAxis("Vertical");
        //UnityEngine.Debug.Log("vertInput is: " + vertInput);
        if (horInput != 0 || vertInput != 0)
        {
            //Vector3 right = target.right;
            Vector3 right = cameraOrbit.GetComponent<Transform>().right;
            Vector3 forward = Vector3.Cross(right, Vector3.up);
            movement = (right * horInput) + (forward * vertInput);
            movement *= moveSpeed;
            movement = Vector3.ClampMagnitude(movement, moveSpeed);
            Quaternion direction = Quaternion.LookRotation(movement);
            transform.rotation = Quaternion.Lerp(transform.rotation, direction, rotSpeed * Time.deltaTime);
        }

        movement *= Time.deltaTime;
        //charController.Move(movement);
        //currentPosition = transform.position;
        return transform.position + movement;
        //return transform.position;
    }


    public void Move()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            //var randomPosition = GetRandomPositionOnPlane();
            //transform.position = randomPosition;
            //Position.Value = randomPosition;
        }
        else
        {
            //UnityEngine.Debug.Log("This is running");
            Vector3 movement = GetMovementPosition();
            SubmitPositionRequestServerRpc(movement);
        }
    }

    [ServerRpc]
    void SubmitPositionRequestServerRpc(Vector3 movement)
    {
        Position.Value = movement;
        //transform.position = Position.Value;
        //UnityEngine.Debug.Log("So is this :" + Position.Value);
    }

    // Update is called once per frame
    void Update()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            //charController.Move(Position.Value);
            transform.position = Position.Value;
            //UnityEngine.Debug.Log("Server position var is: " + Position.Value);
        } 
        else
        {
            Move();
            transform.position = Position.Value;
            //charController.Move(Position.Value);
            UnityEngine.Debug.Log("Client position var is: " + Position.Value);
        }
    }
}
