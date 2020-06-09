using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHoverboardController : MonoBehaviour
{
  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    float vertical = CrossPlatformInputManager.GetAxis("Vertical");
    float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");

    m_RigidBody.AddForce(vertical * m_MoveForce * -transform.right);
    m_RigidBody.AddTorque(horizontal * m_TorqueForce * Vector3.up);
  }
}
