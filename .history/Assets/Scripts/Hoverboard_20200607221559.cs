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

    m_RigidBody.AddForce(vertical * m_MoveForce * transform.forward);
    m_RigidBody.AddTorque(horizontal * m_TorqueForce * Vector3.up);

    RaycastHit hit;

    foreach (GameObject point in m_Points)
    {
      Ray downRay = new Ray(point.transform.position, -Vector3.up);
      // Raycast downward
      if (Physics.Raycast(downRay, out hit))
      {

        float distance = m_HoverHeight - hit.distance;
        if (distance > 0)
        {
          // Subtract the damping from the lifting force and apply it to
          // the rigidbody.
          float upwardSpeed = m_RigidBody.velocity.y;
          float lift = distance * m_HoverForce - upwardSpeed * m_HoverDamp;
          m_RigidBody.AddForce(lift * Vector3.up, point.transform.position);

        }
      }
    }
    m_RigidBody.AddTorque(transform.up * m_TorqueForce * horizontal);
  }
}
