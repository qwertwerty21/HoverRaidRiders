using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class Hoverboard : MonoBehaviour
{

  public float m_MoveForce = 5f;
  public float m_TorqueForce = 5f;
  public Rigidbody m_RigidBody;
  private void Awake()
  {
    m_RigidBody = GetComponent<Rigidbody>();
  }

  // Update is called once per frame
  private void FixedUpdate()
  {
    // make a hoverpoint script with https://docs.unity3d.com/ScriptReference/RaycastHit-distance.html
    // in each 
    float vertical = CrossPlatformInputManager.GetAxis("Vertical");
    float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");

    m_RigidBody.AddForce(vertical * m_MoveForce * Vector3.forward);
    m_RigidBody.AddTorque(horizontal * m_TorqueForce * Vector3.up);


  }
}
