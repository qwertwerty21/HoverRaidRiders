﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class Hoverboard : MonoBehaviour
{
  public GameObject[] m_Points;
  public float m_TorqueForce = 5f;

  public float m_HoverHeight = 4f;


  // The force applied per unit of distance below the desired height.
  public float m_HoverForce = 5.0f;

  // The amount that the lifting force is reduced per unit of upward speed.
  // This damping tends to stop the object from bouncing after passing over
  // something.
  float m_HoverDamp = 0.5f;

  public LayerMask m_LayerMask;
  private Rigidbody m_RigidBody;


  private void Awake()
  {
    m_RigidBody = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  private void FixedUpdate()
  {
    float vertical = CrossPlatformInputManager.GetAxis("Vertical");
    float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");

    rb.AddForce(Input.GetAxis("Vertical") * moveForce * transform.forward);
    rb.AddTorque(Input.GetAxis("Horizontal") * rotateTorque * Vector3.up);

    float range = 100f;
    foreach (GameObject point in m_Points)
    {
      RaycastHit hit;
      // Raycast downward
      Physics.Raycast(point.transform.position, Vector3.down, out hit, range, m_LayerMask);
      float distance = hit.point -
    }
    m_RigidBody.AddTorque(transform.up * m_TorqueForce * horizontal);
  }
}
