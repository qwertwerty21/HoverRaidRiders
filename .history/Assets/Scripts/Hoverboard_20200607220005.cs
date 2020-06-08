using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class Hoverboard : MonoBehaviour
{
  public GameObject[] m_Points;
  public float m_TorqueForce = 5f;
  private Rigidbody m_RigidBody;

  private void Awake()
  {
    m_RigidBody = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void Update()
  {
    float turn = CrossPlatformInputManager.GetAxis("Horizontal");
    m_RigidBody.AddTorque(transform.up * m_TorqueForce * turn);
  }
}
