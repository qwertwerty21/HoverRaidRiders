using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using MoreMountains.Feedbacks;


public class Hoverboard : MonoBehaviour
{
  public float m_Mass = 3f;
  public float m_Drag = 1f;
  public float m_AngularDrag = 2f;
  [Range(0, 1)]
  public float m_AngularVelocityDampenPercentageOnCollisions = .7f;
  public float m_InitialSpeed = 10f;
  public float m_MaxSpeed = 25f;
  public float m_Acceleration = 1f;
  public float m_Deceleration = 1f;
  public float m_AutoStabilizeStability = 1f;
  public float m_AutoStabilizeSpeed = 2f;
  public float m_JumpForce = 10f;
  public float m_MaxJumps = 1f;
  // additional gravity without having to adjust mass 
  // or world gravity
  public float m_InitialAdditionalGravity = 1f;
  public float m_MaxAdditionalGravity = 99f;

  public float m_GravityAcceleration = 5f;
  public float m_SteerStabilityForce = .2f;
  // public float m_RotationAmount = 50f;
  public float m_MaxRotationX = 90f;

  public float m_MaxRotationY = 15f;
  public float m_MaxRotationZ = 90f;
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
  public float m_MinFOV = 40f;
  public float m_MaxFOV = 80f;
  public float m_FOVZoomInAdjustmentSpeed = 3f;
  public float m_FOVZoomOutAdjustmentSpeed = 7f;
  [Range(0f, 1f)]
  public float m_FOVSwitchThreshold = .9f;
  public float m_StaggerFeedbackThreshold = .6f;
  public float m_CameraSpeedLineThreshold = .9f;
  public Rigidbody m_RigidBody;
  public LayerMask m_GroundLayerMask; // could be unnecessary
  private Dictionary<string, MMFeedbacks> m_FeedbacksHash = new Dictionary<string, MMFeedbacks>();
  private Rider m_Rider;
  public bool m_IsGrounded = false;
  private float m_CurrentSpeed;
  private float m_CurrentJumpCount = 0f;
  private float m_CurrentAdditionalGravity;
  private GameObject[] m_HoverboardPoints;
  private GameObject m_HoverboardGroundCheckPoint;

  public void Move(float horizontal, float vertical)
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
    if (!Mathf.Approximately(vertical, 0f))
    {
      m_FeedbacksHash["AccelerationFeedback"].PlayFeedbacks();
      m_RigidBody.AddForce(vertical * m_CurrentSpeed * transform.forward, ForceMode.Acceleration);
    }

    // add turning force
    if (!Mathf.Approximately(horizontal, 0f))
    {
      m_FeedbacksHash["TurnFeedback"].PlayFeedbacks();

      m_RigidBody.AddTorque(horizontal * m_TorqueForce * Vector3.up, ForceMode.Force);
    }

    // add steer stability force
    Vector3 worldVelocity = m_RigidBody.velocity;
    Vector3 localVelocity = transform.InverseTransformVector(worldVelocity);

    // Create a force in the opposite direction of our sideways velocity 
    // (this creates stability when steering)
    float steerStabilityForce = m_SteerStabilityForce;
    Vector3 localOpposingForce = new Vector3(-localVelocity.x * steerStabilityForce, 0f, 0f);
    Vector3 worldOpposingForce = transform.TransformVector(localOpposingForce);
    m_RigidBody.AddForce(worldOpposingForce, ForceMode.Impulse);

    // set animator params (should this be a function inside Rider?)
    m_Rider.m_Animator.SetFloat("vertical", vertical);
    m_Rider.m_Animator.SetFloat("horizontal", horizontal);

    float currentSpeedPercentage = m_RigidBody.velocity.magnitude / m_MaxSpeed;

    // adjust field of view according to speed
    float targetFOV = m_MinFOV;
    float fovAdjustmentSpeed = m_FOVZoomInAdjustmentSpeed;
    if (currentSpeedPercentage > m_FOVSwitchThreshold)
    {
      targetFOV = Mathf.Clamp(currentSpeedPercentage * m_MaxFOV, m_MinFOV, m_MaxFOV);
      fovAdjustmentSpeed = m_FOVZoomOutAdjustmentSpeed;
    }
    GameManager.Instance.SetFieldOfView(targetFOV, fovAdjustmentSpeed);

    // play speedfeedback if appropriate
    if (currentSpeedPercentage > m_CameraSpeedLineThreshold)
    {
      GameManager.Instance.PlayCameraFeedback("SpeedFeedback");
    }
  }

  public void Jump()
  {
    if (m_CurrentJumpCount < m_MaxJumps)
    {
      m_CurrentJumpCount++;
      m_RigidBody.AddForce(m_JumpForce * Vector3.up, ForceMode.Impulse);
      m_FeedbacksHash["JumpFeedback"].PlayFeedbacks();
    }
    print("m_CurrentJumpCount " + m_CurrentJumpCount);
  }

  private void DampenAngularVelocity()
  {
    if (m_RigidBody.angularVelocity.magnitude > .01f)
    {
      m_RigidBody.angularVelocity *= m_AngularVelocityDampenPercentageOnCollisions;
    }
  }
  private void Awake()
  {
    m_RigidBody = GetComponent<Rigidbody>();
    m_HoverboardPoints = GameObject.FindGameObjectsWithTag("HoverboardPoint");
    m_HoverboardGroundCheckPoint = GameObject.FindGameObjectWithTag("HoverboardGroundCheckPoint");

    // add all feedbacks to hash for easy access
    MMFeedbacks[] feedbacks = GetComponentsInChildren<MMFeedbacks>();
    foreach (MMFeedbacks feedback in feedbacks)
    {
      m_FeedbacksHash.Add(feedback.gameObject.name, feedback);
    }

    m_Rider = GetComponentInChildren<Rider>();

    // lower center of mass so we don't flip
    Vector3 centerOfMass = m_RigidBody.centerOfMass;
    centerOfMass.y -= 1f;
    m_RigidBody.centerOfMass = centerOfMass;

    m_RigidBody.mass = m_Mass;
    m_RigidBody.drag = m_Drag;
    m_RigidBody.angularDrag = m_AngularDrag;
    // set currentSpeed 
    m_CurrentSpeed = m_InitialSpeed;
  }

  // Update is called once per frame
  private void FixedUpdate()
  {
    m_RigidBody.angularDrag = m_AngularDrag;

    // gravity 
    m_RigidBody.AddForce(Vector3.down * m_CurrentAdditionalGravity, ForceMode.Acceleration);

    // stabilize
    Vector3 predictedUp = Quaternion.AngleAxis(m_RigidBody.angularVelocity.magnitude * Mathf.Rad2Deg * m_AutoStabilizeStability / m_AutoStabilizeSpeed, m_RigidBody.angularVelocity) * transform.up;
    Vector3 torqueVector = Vector3.Cross(predictedUp, Vector3.up);
    m_RigidBody.AddTorque(torqueVector * m_AutoStabilizeSpeed * m_AutoStabilizeSpeed, ForceMode.Acceleration);

    // Float Points
    RaycastHit hit;
    foreach (GameObject point in m_HoverboardPoints)
    {
      Ray downRay = new Ray(point.transform.position, Vector3.down);
      Debug.DrawRay(point.transform.position, Vector3.down, Color.red);
      // Raycast downward
      if (Physics.Raycast(downRay, out hit, m_HoverboardPointRayDistance))
      {
        float lift = m_HoverForce * Mathf.Pow((1f - (hit.distance / m_IdealHoverHeight)), 1.7f);
        lift = System.Single.IsNaN(lift) ? m_AbsoluteMinLift : lift;
        float bounce = Mathf.Sin(Time.time * m_HoverBounceSpeed) * m_HoverBounceHeight / m_RigidBody.velocity.magnitude;
        lift += System.Single.IsNaN(bounce) ? 0f : bounce;
        lift = Mathf.Clamp(lift, m_AbsoluteMinLift, m_AbsoluteMaxLift);
        // todo
        // mmfeedbacks juice
        // jump juice and animation
        // limit jump count and do jump animation
        // cursor
        // shoot 
        // sword
        // animations for character
        // dotween to rotate board
        // drift sparks and boost
        // wall run 
        // rail grind
        // object pool all prefabs

        m_RigidBody.AddForceAtPosition(lift * Vector3.up, point.transform.position, ForceMode.Acceleration);
      }
    }

    // check if grounded
    RaycastHit groundCheckHit;
    Ray groundCheckDownRay = new Ray(m_HoverboardGroundCheckPoint.transform.position, Vector3.down);
    Debug.DrawRay(m_HoverboardGroundCheckPoint.transform.position, Vector3.down, Color.blue);

    if (Physics.Raycast(groundCheckDownRay, out groundCheckHit, m_GroundCheckRayDistance, m_GroundLayerMask))
    {
      m_IsGrounded = true;

      // reset gravity and jump count
      m_CurrentAdditionalGravity = m_InitialAdditionalGravity;
      m_CurrentJumpCount = 0f;
    }
    else
    {
      m_IsGrounded = false;
      m_CurrentAdditionalGravity = Mathf.SmoothStep(m_CurrentAdditionalGravity, m_MaxAdditionalGravity, Time.deltaTime * m_GravityAcceleration);
    }

  }

  void OnCollisionEnter(Collision collision)
  {
    DampenAngularVelocity();

    // we dont want to stagger if hoverboard contacts ground on bottom
    // because that looks weird
    var normal = collision.contacts[0].normal;
    if (normal.y < 1f && m_RigidBody.angularVelocity.magnitude > m_StaggerFeedbackThreshold)
    {
      m_FeedbacksHash["StaggerFeedback"].PlayFeedbacks();
    }
  }

  void OnCollisionStay(Collision collision)
  {
    DampenAngularVelocity();
  }
}
