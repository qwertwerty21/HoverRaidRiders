using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class Hoverboard : MonoBehaviour
{
  public float m_MoveForce = 5f;
  public float m_TorqueForce = 5f;
  public float m_HoverHeight = 4f;
  // The force applied per unit of distance below the desired height.
  public float m_HoverForce = 5f;
  // The amount that the lifting force is reduced per unit of upward speed.
  // This damping tends to stop the object from bouncing after passing over
  // something.
  public float m_HoverDamp = 0.5f;
  private GameObject[] m_HoverboardPoints;
  private Rigidbody m_RigidBody;


  private void Awake()
  {
    m_RigidBody = GetComponent<Rigidbody>();
    transform.parent = GameObject.FindWithTag("Player").transform;
  }

  // Update is called once per frame
  private void FixedUpdate()
  {
    // make a hoverpoint script with https://docs.unity3d.com/ScriptReference/RaycastHit-distance.html
    float vertical = CrossPlatformInputManager.GetAxis("Vertical");
    float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");

    m_RigidBody.AddForce(vertical * m_MoveForce * transform.forward);
    m_RigidBody.AddTorque(horizontal * m_TorqueForce * Vector3.up);

    RaycastHit hit;

    foreach (GameObject point in m_Points)
    {
      Ray downRay = new Ray(transform.position, -Vector3.up);
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
          m_RigidBody.AddForce(lift * Vector3.up);

        }
      }
    }
  }
}
