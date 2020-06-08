using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class Hoverboard : MonoBehaviour
{

  private Rigidbody m_RigidBody;
  private GameObject[] m_Points;

  private void Awake()
  {
    m_RigidBody = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  void Update()
  {
    float turn = CrossPlatformInputManager.GetAxis("Horizontal");
    rb.AddTorque(transform.up * torque * turn);
  }
}
