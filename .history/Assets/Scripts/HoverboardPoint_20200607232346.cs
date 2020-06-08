using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;


public class HoverboardPoint : MonoBehaviour
{
  private Hoverboard m_Hoverboard;

  public float m_HoverHeight = 4f;


  // The force applied per unit of distance below the desired height.
  public float m_HoverForce = 5.0f;

  // The amount that the lifting force is reduced per unit of upward speed.
  // This damping tends to stop the object from bouncing after passing over
  // something.
  float m_HoverDamp = 0.5f;



  private void Awake()
  {
    m_Hoverboard = GameObject.FindWithTag("Hoverboard").GetComponent<Hoverboard>();
  }

  // Update is called once per frame
  private void FixedUpdate()
  {

    RaycastHit hit;


    Ray downRay = new Ray(transform.position, Vector3.down);
    // Raycast downward
    if (Physics.Raycast(downRay, out hit))
    {

      float hoverError = m_HoverHeight - hit.distance;
      if (hoverError > 0)
      {
        // Subtract the damping from the lifting force and apply it to
        // the rigidbody.
        float upwardSpeed = m_Hoverboard.m_RigidBody.velocity.y;
        float lift = hoverError * m_HoverForce - upwardSpeed * m_HoverDamp;
        m_Hoverboard.m_RigidBody.AddForce(lift * Vector3.up);

      }
    }

  }
}
