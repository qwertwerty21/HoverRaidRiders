using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Hoverboard : MonoBehaviour
{

  public float m_InitialSpeed = 10f;
  public float m_MaxSpeed = 25f;
  // when drifting, the your current speed will be
  // DIVIDED by this number, 
  // so the HIGHER the SLOWER the drift speed
  // the LOWER the FASTER the drift speed
  public float m_DriftSpeedReductionFactor = 10f;
  public float m_Acceleration = 1f;
  public float m_Deceleration = 1f;
  // additional gravity without having to adjust mass 
  // or world gravity
  public float m_AdditionalGravity = 1f;
  public float m_SteerStabilityForce = 1f;
  public float m_DriftSteerStabilityForce = 0.1f;
  public float m_MaxRotationX = 90f;
  public float m_MaxRotationZ = 90f;
  [Range(0f, 10f)]
  public float m_HoverBounceSpeed = 1f;
  // additional force added to lift to create unstable effect
  // DECREASES as velocity INCREASES
  [Range(0f, 10f)]
  public float m_HoverBounceHeight = 1f;
  public float m_AbsoluteMinLift = .2f;
  public float m_AbsoluteMaxLift = 5f;
  public float m_TorqueForce = 5f;
  public float m_IdealHoverHeight = 4f;
  // The force applied per unit of distance below the desired height.
  public float m_HoverForce = 5f;
  // The amount that the lifting force is reduced per unit of upward speed.
  // This damping tends to stop the object from bouncing after passing over
  // something.
  public float m_HoverDamp = 0.5f;
  public Rigidbody m_RigidBody;

  public LayerMask m_GroundLayerMask;

  private float m_CurrentSpeed;
  private GameObject[] m_HoverboardPoints;

  private GameObject m_HoverboardAccelPoint;

  public void Move(float horizontal, float vertical, bool isDrifting)
  {
    // accelerate if moving forward
    if (vertical > 0f)
    {
      m_CurrentSpeed = Mathf.SmoothStep(m_CurrentSpeed, m_MaxSpeed, Time.deltaTime * m_Acceleration);
    }
    // decelerate if moving back or stopping
    else
    {
      m_CurrentSpeed = Mathf.SmoothStep(m_CurrentSpeed, m_InitialSpeed, Time.deltaTime * m_Deceleration);
    }

    // add forward force
    Debug.Log("CurrentSpeed " + m_CurrentSpeed);
    if (isDrifting)
    {
      m_RigidBody.AddForce(vertical * m_CurrentSpeed * transform.forward / m_DriftSpeedReductionFactor, ForceMode.Impulse);
    }
    else
    {
      m_RigidBody.AddForce(vertical * m_CurrentSpeed * transform.forward, ForceMode.Acceleration);
    }

    // add turning force
    m_RigidBody.AddTorque(horizontal * m_TorqueForce * Vector3.up, ForceMode.Force);
    Debug.Log("IsDrifting" + isDrifting);

    // add steer stability force
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

    // set currentSpeed 
    m_CurrentSpeed = m_InitialSpeed;
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
          // Debug.Log("magnitude " + m_RigidBody.velocity.magnitude);
          float bounce = Mathf.Sin(Time.time * m_HoverBounceSpeed) * m_HoverBounceHeight / m_RigidBody.velocity.magnitude;
          lift1 += System.Single.IsNaN(bounce) ? 0f : bounce;
          Debug.Log("bounce " + bounce);
          lift1 = Mathf.Clamp(lift1, m_AbsoluteMinLift, m_AbsoluteMaxLift);
          // Debug.Log("lift1 " + lift1);
          // Debug.Log("lift2 " + lift2);
          // todo
          // drift sparks and boost
          // replace mousey with kenny blocky asset
          // animations for character
          // mmfeedbacks juice
          // dotween to rotate board

          m_RigidBody.AddForceAtPosition(lift1 * Vector3.up, point.transform.position, ForceMode.Acceleration);
        }
      }
      else
      {
        // if raycasting down fails, add max force as a last resort so we dont sink
        m_RigidBody.AddForceAtPosition(m_AbsoluteMinLift * Vector3.up, point.transform.position, ForceMode.Impulse);

      }
    }

    // rotate to align with ground 
    RaycastHit centerOfMassHit;
    Ray centerOfMassDownRay = new Ray(m_RigidBody.centerOfMass, Vector3.down);
    Debug.DrawRay(m_RigidBody.centerOfMass, Vector3.down, Color.blue);

    if (Physics.Raycast(centerOfMassDownRay, out centerOfMassHit, Mathf.Infinity, m_GroundLayerMask))
    {
      // after raycast, or however you get normal:
      // Compute angle to tilt with ground:
      Quaternion groundRotation = Quaternion.FromToRotation(Vector3.up, centerOfMassHit.normal) * transform.rotation;
      // // Base angle is straight up, spun only on y:
      // // "facing" is the script variable used for turning:
      // transform.rotation = Quaternion.Euler(0, 1f, 0);
      // tilt to align with ground:  
      transform.rotation = Quaternion.Slerp(transform.rotation, groundRotation, Time.deltaTime * 50f);
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
