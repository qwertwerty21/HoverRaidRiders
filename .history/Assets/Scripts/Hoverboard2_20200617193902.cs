using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoverboard2 : MonoBehaviour
{
  public WheelCollider m_WheelColliderFrontLeft;
  public WheelCollider m_WheelColliderFrontRight;
  public WheelCollider m_WheelColliderBackLeft;
  public WheelCollider m_WheelColliderBackRight;
  public float m_MaxSteerAngle = 30;
  public float m_MotorForce = 50;
  private float m_SteeringAngle;

  public void Steer(float horizontal)
  {
    m_SteeringAngle = m_MaxSteerAngle * horizontal;
    m_WheelColliderFrontLeft.steerAngle = m_SteeringAngle;
    m_WheelColliderFrontRight.steerAngle = m_SteeringAngle;
  }

  public void Accelerate(float vertical)
  {
    m_WheelColliderFrontLeft.motorTorque = vertical * m_MotorForce;
    m_WheelColliderFrontRight.motorTorque = vertical * m_MotorForce;
  }

  // public void UpdateWheelPoses()
  // {
  //   UpdateWheelPose(frontDriverW, frontDriverT);
  //   UpdateWheelPose(frontPassengerW, frontPassengerT);
  //   UpdateWheelPose(rearDriverW, rearDriverT);
  //   UpdateWheelPose(rearPassengerW, rearPassengerT);
  // }

  // public void UpdateWheelPose(WheelCollider _collider, Transform _transform)
  // {
  //   Vector3 _pos = _transform.position;
  //   Quaternion _quat = _transform.rotation;

  //   _collider.GetWorldPose(out _pos, out _quat);

  //   _transform.position = _pos;
  //   _transform.rotation = _quat;
  // }

  private void Awake()
  {
    m_WheelColliderFrontLeft = GameObject.FindGameObjectWithTag("WheelColliderFrontLeft").GetComponent<WheelCollider>();
    m_WheelColliderFrontRight = GameObject.FindGameObjectWithTag("WheelColliderFrontRight").GetComponent<WheelCollider>();
    m_WheelColliderBackLeft = GameObject.FindGameObjectWithTag("WheelColliderBackLeft").GetComponent<WheelCollider>();
    m_WheelColliderBackRight = GameObject.FindGameObjectWithTag("WheelColliderBackRight").GetComponent<WheelCollider>();

  }
}