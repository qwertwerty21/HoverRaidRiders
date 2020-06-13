using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hoverboard : MonoBehaviour
{
  // additional gravity without having to adjust mass 
  // or world gravity
  public float m_AdditionalGravity = 1f;
  public float m_SteerStabilityForce = 1f;
  public float m_DriftSteerStabilityForce = 0.1f;
  public float m_MaxRotationX = 90f;
  public float m_MaxRotationZ = 90f;
  [Range(0f, 100f)]
  public float m_HoverBounceSpeed = 1f;
  // additional force added to lift to create unstable effect
  [Range(0f, 100f)]
  public float m_HoverBounceHeight = 1f;
  public float m_AbsoluteMinLift = .2f;
  public float m_AbsoluteMaxLift = 5f;
  public float m_Speed = 5f;
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

  private GameObject m_HoverboardAccelPoint;

  public void Move(float horizontal, float vertical, bool isDrifting)
  {
    m_RigidBody.AddForce(vertical * m_Speed * transform.forward, ForceMode.Acceleration);
    m_RigidBody.AddTorque(horizontal * m_TorqueForce * Vector3.up, ForceMode.Force);
    Debug.Log("IsDrifting" + isDrifting);

    Vector3 worldVelocity = m_RigidBody.velocity;
    Vector3 localVelocity = transform.InverseTransformVector(worldVelocity);

    // Create a force in the opposite direction of our sideways velocity 
    // (this creates stability when steering)
    float steerStabilityForce = isDrifting ? m_DriftSteerStabilityForce : m_SteerStabilityForce;
    Vector3 localOpposingForce = new Vector3(-localVelocity.x * steerStabilityForce, 0f, 0f);
    Vector3 worldOpposingForce = transform.TransformVector(localOpposingForce);

    m_RigidBody.AddForce(worldOpposingForce, ForceMode.Impulse);

  }
  private void Awake()
  {
    m_RigidBody = GetComponent<Rigidbody>();
    m_HoverboardPoints = GameObject.FindGameObjectsWithTag("HoverboardPoint");
    m_HoverboardAccelPoint = GameObject.FindGameObjectWithTag("HoverboardAccelPoint");

    // lower center of mass so we don't flip
    Vector3 centerOfMass = m_RigidBody.centerOfMass;
    centerOfMass.y -= 1f;
    m_RigidBody.centerOfMass = centerOfMass;
  }

  // Update is called once per frame
  private void FixedUpdate()
  {
    // gravity 
    m_RigidBody.AddForce(Vector3.down * m_AdditionalGravity, ForceMode.Acceleration);

    // Float Points
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
          // lift1 += Random.Range(-m_HoverBounceHeight, m_HoverBounceHeight);
          lift1 += Mathf.Sin(Time.time * m_RigidBody.velocity.magnitude * m_HoverBounceSpeed) * m_HoverBounceHeight;
          Debug.Log("Sin " + Mathf.Sin(Time.time * m_RigidBody.velocity.magnitude * m_HoverBounceSpeed) * m_HoverBounceHeight);
          lift1 = Mathf.Clamp(lift1, m_AbsoluteMinLift, m_AbsoluteMaxLift);
          Debug.Log("lift1 " + lift1);
          Debug.Log("lift2 " + lift2);
          // todo
          // acceleration
          // gravity 
          // ussing Mathf.SmoothStep to control acceleration
          // drift sparks and boost
          // drift steer opposite force should be an adjustable variable

          m_RigidBody.AddForceAtPosition(lift1 * Vector3.up, point.transform.position, ForceMode.Acceleration);
        }
      }
      else
      {
        // if raycasting down fails, add max force as a last resort so we dont sink
        m_RigidBody.AddForceAtPosition(m_AbsoluteMaxLift * Vector3.up, point.transform.position);

      }
    }
  }

  // clamps rotation so we dont flip
  // private void LateUpdate()
  // {
  //   // eulerAngles is returning a value between 0 and 360, 
  //   // so if > 180 we substract 180 so we can clamp correctly
  //   float rotationX = transform.eulerAngles.x;
  //   rotationX = rotationX > 180f ? rotationX - 360f : rotationX;
  //   rotationX = Mathf.Clamp(rotationX, -m_MaxRotationX, m_MaxRotationX);

  //   float rotationZ = transform.eulerAngles.z;
  //   rotationZ = rotationZ > 180f ? rotationZ - 360f : rotationZ;
  //   rotationZ = Mathf.Clamp(rotationZ, -m_MaxRotationZ, m_MaxRotationZ);


  //   // Converts our numbers into euler angles
  //   Quaternion rotation = Quaternion.Euler(rotationX, transform.eulerAngles.y, rotationZ);

  //   // Sets our new rot
  //   transform.rotation = rotation;
  // }
}
