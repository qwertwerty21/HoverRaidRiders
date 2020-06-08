﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class Hoverboard : MonoBehaviour
{
  public GameObject[] m_Points;
  public float m_TorqueForce = 5f;

  public LayerMask m_LayerMask;
  private Rigidbody m_RigidBody;


  private void Awake()
  {
    m_RigidBody = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void Update()
  {
    float vertical = CrossPlatformInputManager.GetAxis("Vertical");
    float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");

    foreach (GameObject point in m_Points)
    {
      RaycastHit hit;
      float range = 100f;
      // Raycast downward
      Physics.Raycast(point.transform.position, Vector3.down, out hit, 100f, m_LayerMask);
    }
    m_RigidBody.AddTorque(transform.up * m_TorqueForce * horizontal);
  }
}
