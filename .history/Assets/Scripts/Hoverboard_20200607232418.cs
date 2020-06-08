using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class Hoverboard : MonoBehaviour
{
  public GameObject[] m_Points;

  public float m_MoveForce = 5f;
  public float m_TorqueForce = 5f;

  public float m_HoverHeight = 4f;


  // The force applied per unit of distance below the desired height.
  public float m_HoverForce = 5.0f;

  // The amount that the lifting force is reduced per unit of upward speed.
  // This damping tends to stop the object from bouncing after passing over
  // something.
  float m_HoverDamp = 0.5f;

  public LayerMask m_LayerMask;
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
