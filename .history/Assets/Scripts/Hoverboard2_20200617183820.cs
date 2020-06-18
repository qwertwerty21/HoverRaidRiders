using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleCarController : MonoBehaviour
{
  public WheelCollider m_WheelColliderFrontLeft;
  public WheelCollider m_WheelColliderFrontRight;
  public WheelCollider m_WheelColliderBackLeft;
  public WheelCollider m_WheelColliderBackLeft;
  public float m_MaxSteerAngle = 30;
  public float m_MotorForce = 50;
  private float m_SteeringAngle;
  private float m_horizontalInput;
  private float m_verticalInput;


  public void GetInput()
  {
    m_horizontalInput = Input.GetAxis("Horizontal");
    m_verticalInput = Input.GetAxis("Vertical");
  }

  private void Steer()
  {
    m_SteeringAngle = maxSteerAngle * m_horizontalInput;
    frontDriverW.steerAngle = m_SteeringAngle;
    frontPassengerW.steerAngle = m_SteeringAngle;
  }

  private void Accelerate()
  {
    frontDriverW.motorTorque = m_verticalInput * motorForce;
    frontPassengerW.motorTorque = m_verticalInput * motorForce;
  }

  private void UpdateWheelPoses()
  {
    UpdateWheelPose(frontDriverW, frontDriverT);
    UpdateWheelPose(frontPassengerW, frontPassengerT);
    UpdateWheelPose(rearDriverW, rearDriverT);
    UpdateWheelPose(rearPassengerW, rearPassengerT);
  }

  private void UpdateWheelPose(WheelCollider _collider, Transform _transform)
  {
    Vector3 _pos = _transform.position;
    Quaternion _quat = _transform.rotation;

    _collider.GetWorldPose(out _pos, out _quat);

    _transform.position = _pos;
    _transform.rotation = _quat;
  }

  private void FixedUpdate()
  {
    GetInput();
    Steer();
    Accelerate();
    UpdateWheelPoses();
  }


}