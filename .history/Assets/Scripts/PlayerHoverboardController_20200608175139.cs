using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerHoverboardController : MonoBehaviour
{
  private Hoverboard m_Hoverboard;
  void Awake()
  {
    m_Hoverboard = GetComponent<Hoverboard>();
  }

  // Update is called once per frame
  void Update()
  {
    float vertical = CrossPlatformInputManager.GetAxis("Vertical");
    float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");

    m_Hoverboard.m_RigidBody.AddForce(vertical * m_MoveForce * -transform.right);
    m_Hoverboard.m_RigidBody.AddTorque(horizontal * m_TorqueForce * Vector3.up);
  }
}
