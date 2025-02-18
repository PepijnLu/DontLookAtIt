using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public bool atCrossroad;
    bool spaceInput;
    [SerializeField] float maxMovementSpeed, acceleration, slowdown;
    public float movementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.Space)) spaceInput = true;
        else spaceInput = false;

        ChooseCrossroadDirection();
    }

    void FixedUpdate()
    {
        WalkForward();
    }

    void WalkForward()
    {
        if(atCrossroad) return;

        if(spaceInput)
        {
            if(movementSpeed < maxMovementSpeed)
            {
                movementSpeed += acceleration * Time.deltaTime;
            }
        }
        else
        {
            if(movementSpeed > 0)
            {
                movementSpeed -= slowdown * Time.deltaTime;
            }
        }

        if(movementSpeed > maxMovementSpeed) movementSpeed = maxMovementSpeed;
        if(movementSpeed < 0) movementSpeed = 0;

        transform.Translate(0, 0, movementSpeed * Time.deltaTime);
    }
    
    void ChooseCrossroadDirection()
    {
        if(!atCrossroad) return;

        if(Input.GetKeyDown(KeyCode.W))
        {
            //nothing
            StartCoroutine(TurnPlayer(0, 0));
        }
        else if(Input.GetKeyDown(KeyCode.S))
        {
            StartCoroutine(TurnPlayer(180, 1));
        }
        else if(Input.GetKeyDown(KeyCode.D))
        {
            StartCoroutine(TurnPlayer(90, 1));
        }
        else if(Input.GetKeyDown(KeyCode.A))
        {
            StartCoroutine(TurnPlayer(-90, 1));
        }
    }

    IEnumerator TurnPlayer(int angle, float duration)
    {
        float elapsedTime = 0f;

        Quaternion startRotation = transform.rotation;
        Quaternion endRotation = transform.rotation * Quaternion.Euler(0, angle, 0);

        while (elapsedTime < duration)
        {
            elapsedTime += 0.02f;
            float t = elapsedTime / duration;
            transform.rotation = Quaternion.Slerp(startRotation, endRotation, t);
            yield return new WaitForFixedUpdate();
        }

        // Ensure exact rotation at the end
        transform.rotation = endRotation;
        atCrossroad = false;
    }

    // void OnTriggerEnter(Collider collider)
    // {
    //     if(collider.CompareTag("Crossroad"))
    //     {
    //         Debug.Log("Collision");
    //         atCrossroad = true;
    //         movementSpeed = 0;
    //     }
    // }
}
