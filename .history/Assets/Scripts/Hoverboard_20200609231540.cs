using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hoverboard : MonoBehaviour
{
  // additional force added to lift to create unstable effect
  [Range(0f, 5f)]
  public float m_RandomBounceFactor = 1f;
  public float m_AbsoluteMinLift = .2f;
  public float m_AbsoluteMaxLift = 5f;
  public float m_MoveForce = 5f;
  public float m_TorqueForce = 5f;
  public float m_IdealHoverHeight = 4f;
  // The force applied per unit of distance below the desired height.
  public float m_HoverForce = 5f;
  // The amount that the lifting force is reduced per unit of upward speed.
  // This damping tends to stop the object from bouncing after passing over
  // something.
  public float m_HoverDamp = 0.5f;
  public Rigidbody m_RigidBody;
  private GameObject[] m_HoverboardPoints;

  public void Move(float horizontal, float vertical)
  {
    m_RigidBody.AddForce(vertical * m_MoveForce * transform.forward);
    m_RigidBody.AddTorque(horizontal * m_TorqueForce * Vector3.up);
  }
  private void Awake()
  {
    m_RigidBody = GetComponent<Rigidbody>();
    m_HoverboardPoints = GameObject.FindGameObjectsWithTag("HoverboardPoint");

    // lower center of mass so we don't flip
    Vector3 centerOfMass = m_RigidBody.centerOfMass;
    centerOfMass.y -= .5f;
    m_RigidBody.centerOfMass = centerOfMass;
  }

  // Update is called once per frame
  private void FixedUpdate()
  {
    Debug.Log("EulerAngles " + transform.eulerAngles);
    RaycastHit hit;

    foreach (GameObject point in m_HoverboardPoints)
    {
      Ray downRay = new Ray(point.transform.position, Vector3.down);
      Debug.DrawRay(point.transform.position, Vector3.down, Color.red);
      // Raycast downward
      if (Physics.Raycast(downRay, out hit))
      {
        float hoverError = m_IdealHoverHeight - hit.distance;
        Debug.Log("hoverError" + hoverError);
        if (hoverError > 0)
        {
          // Subtract the damping from the lifting force and apply it to
          // the rigidbody.
          float upwardSpeed = m_RigidBody.velocity.y;
          float lift1 = m_HoverForce * Mathf.Pow((1f - (hit.distance / m_IdealHoverHeight)), 1.7f);
          float lift2 = hoverError * m_HoverForce - upwardSpeed * m_HoverDamp;
          lift2 += Random.Range(-m_RandomBounceFactor, m_RandomBounceFactor);
          lift2 = Mathf.Clamp(lift2, m_AbsoluteMinLift, m_AbsoluteMaxLift);
          Debug.Log("lift1 " + lift1);
          Debug.Log("lift2 " + lift2);
          // todo
          // drift

          m_RigidBody.AddForceAtPosition(lift2 * Vector3.up, point.transform.position);
        }
      }
      else
      {
        // if raycasting down fails, add max force as a last resort so we dont sink
        m_RigidBody.AddForceAtPosition(m_AbsoluteMaxLift * Vector3.up, point.transform.position);

      }
    }
  }
  private void LateUpdate()
  {
    float x = transform.rotation.eulerAngles.x;
    float z = transform.rotation.eulerAngles.z;

    // Converts our numbers into euler angles.
    Quaternion rotation = Quaternion.Euler(y, x, z);

    // Sets our new rot.
    transform.rotation = rotation;
  }
}
