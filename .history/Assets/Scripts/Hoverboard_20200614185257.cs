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
  public float m_AirSpeedReductionFactor = 10f;
  // additional gravity without having to adjust mass 
  // or world gravity
  public float m_AdditionalGravity = 1f;
  public float m_SteerStabilityForce = 1f;
  public float m_DriftSteerStabilityForce = 0.1f;
  public float m_RotationAmount = 50f;
  public float m_MaxRotationX = 90f;

  public float m_MaxRotationY = 15f;
  public float m_MaxRotationZ = 90f;
  [Range(0f, 10f)]
  public float m_HoverboardPointRayDistance = 15f;
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
  public float m_GroundCheckRayDistance = 20f;
  public Rigidbody m_RigidBody;
  public LayerMask m_GroundLayerMask; // could be unnecessary
  public bool m_IsGrounded = false;
  private float m_CurrentSpeed;
  private GameObject[] m_HoverboardPoints;

  private GameObject m_HoverboardGroundCheckPoint;

  public void Move(float horizontal, float vertical, bool isDrifting)
  {
    if (m_IsGrounded)
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
    }
    else
    {
      m_RigidBody.AddForce(vertical * m_CurrentSpeed * transform.forward / m_AirSpeedReductionFactor, ForceMode.Acceleration);
    }

    if (!Mathf.Approximately(horizontal, 0f))
    {
      m_RigidBody.constraints = RigidbodyConstraints.None;
    }

    // add turning force
    m_RigidBody.AddTorque(horizontal * m_TorqueForce * Vector3.up, ForceMode.Force);

    // // rotate with input
    // float rotationAmount = horizontal * m_RotationAmount;
    // rotationAmount *= Time.deltaTime;
    // transform.Rotate(rotationAmount, transform.rotation.y, rotationAmount);
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
    m_HoverboardGroundCheckPoint = GameObject.FindGameObjectWithTag("HoverboardGroundCheckPoint");

    // lower center of mass so we don't flip
    Vector3 centerOfMass = m_RigidBody.centerOfMass;
    centerOfMass.y -= 1f;
    m_RigidBody.centerOfMass = centerOfMass;

    // set currentSpeed 
    m_CurrentSpeed = m_InitialSpeed;
  }

  // private void Update()
  // {

  // }

  // Update is called once per frame
  private void FixedUpdate()
  {
    m_RigidBody.angularDrag = 2f;
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
      if (Physics.Raycast(downRay, out hit, m_HoverboardPointRayDistance))
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
          // midair movement accel should be reduced
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
        if (m_IsGrounded)
        {
          // if raycasting down fails, add max force as a last resort so we dont sink
          m_RigidBody.AddForceAtPosition(m_AbsoluteMaxLift * Vector3.up, point.transform.position, ForceMode.Impulse);
        }

      }
    }

    RaycastHit groundCheckHit;
    Ray groundCheckDownRay = new Ray(m_HoverboardGroundCheckPoint.transform.position, Vector3.down);
    Debug.DrawRay(center, Vector3.down, Color.blue);

    if (Physics.Raycast(groundCheckDownRay, out groundCheckHit, m_GroundCheckRayDistance, m_GroundLayerMask))
    {
      Debug.Log("CenterOfMassHit" + centerOfMassHit.normal);
      // after raycast, or however you get normal:
      // Compute angle to tilt with ground:
      Quaternion groundRotation = Quaternion.FromToRotation(Vector3.up, centerOfMassHit.normal) * transform.rotation;

      // tilt to align with ground:  
      transform.rotation = Quaternion.Slerp(transform.rotation, groundRotation, Time.deltaTime);
    }

    // // rotate to align with ground 
    // RaycastHit centerOfMassHit;
    // Vector3 center = m_RigidBody.centerOfMass;
    // center.y += 2f;
    // Ray centerOfMassDownRay = new Ray(center, Vector3.down);
    // Debug.DrawRay(center, Vector3.down, Color.blue);

    // if (Physics.Raycast(centerOfMassDownRay, out centerOfMassHit, Mathf.Infinity, m_GroundLayerMask))
    // {
    //   Debug.Log("CenterOfMassHit" + centerOfMassHit.normal);
    //   // after raycast, or however you get normal:
    //   // Compute angle to tilt with ground:
    //   Quaternion groundRotation = Quaternion.FromToRotation(Vector3.up, centerOfMassHit.normal) * transform.rotation;

    //   // tilt to align with ground:  
    //   transform.rotation = Quaternion.Slerp(transform.rotation, groundRotation, Time.deltaTime);
    // }

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

  void OnCollisionEnter(Collision collision)
  {
    // // eulerAngles is returning a value between 0 and 360, 
    // // so if > 180 we substract 180 so we can clamp correctly
    // float rotationX = transform.eulerAngles.x;
    // rotationX = rotationX > 180f ? rotationX - 360f : rotationX;
    // rotationX = Mathf.Clamp(rotationX, -m_MaxRotationX, m_MaxRotationX);

    // float rotationY = transform.eulerAngles.y;
    // rotationY = rotationY > 180f ? rotationY - 360f : rotationY;
    // rotationY = Mathf.Clamp(rotationY, -m_MaxRotationY, m_MaxRotationY);

    // float rotationZ = transform.eulerAngles.z;
    // rotationZ = rotationZ > 180f ? rotationZ - 360f : rotationZ;
    // rotationZ = Mathf.Clamp(rotationZ, -m_MaxRotationZ, m_MaxRotationZ);


    // // Converts our numbers into euler angles
    // Quaternion rotation = Quaternion.Euler(transform.eulerAngles.x, rotationY, transform.eulerAngles.z);

    // // Sets our new rot
    // transform.rotation = rotation;


    if (m_RigidBody.angularVelocity.magnitude > .01f)
    {
      Debug.Log("AngularVelocity magnitude before" + m_RigidBody.angularVelocity.magnitude);
      //Stop rotating
      m_RigidBody.angularVelocity = Vector3.zero;
      m_RigidBody.angularDrag = 9999f;

      m_RigidBody.constraints = RigidbodyConstraints.FreezeRotationY;
      // // add steer stability force
      // Vector3 worldAngularVelocity = m_RigidBody.angularVelocity;
      // Vector3 localAngularVelocity = transform.InverseTransformVector(worldAngularVelocity);

      // // // Create a force in the opposite direction of our sideways velocity 
      // // // (this creates stability when steering)
      // float angleAdjustmentForce = m_RigidBody.angularVelocity.magnitude;
      // Vector3 localOpposingForce = new Vector3(-localAngularVelocity.x * angleAdjustmentForce, 0f, 0f);
      // Vector3 worldOpposingForce = transform.TransformVector(localOpposingForce);


      // m_RigidBody.AddTorque(worldOpposingForce, ForceMode.Impulse);
      Debug.Log("AngularVelocity after" + m_RigidBody.angularVelocity.magnitude);

    }
  }

  private void OnDrawGizmosSelected()
  {
    Gizmos.color = Color.blue;
    //Use the same vars you use to draw your Overlap SPhere to draw your Wire Sphere.
    Gizmos.DrawWireSphere(m_HoverboardGroundCheckPoint.transform.position, m_GroundCheckSphereRadius);
  }
}
