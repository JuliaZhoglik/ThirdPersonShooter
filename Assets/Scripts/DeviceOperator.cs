﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeviceOperator : MonoBehaviour
{
    public float radius = 1.5f;

    private void Update()
    {
        if (Input.GetButtonDown("Fire3"))
        {
            Debug.Log("Fire3");
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius);
            foreach(Collider hitCollider in hitColliders)
            {
                Debug.Log("hitCollider");
                Vector3 direction = hitCollider.transform.position - transform.position;
                if (Vector3.Dot(transform.forward, direction.normalized) > 0.5f)
                {
                    Debug.Log("Dot!");
                    hitCollider.SendMessage("Operate", SendMessageOptions.DontRequireReceiver);
                }
            }
        }
    }
}
