﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{


  public float m_Speed = 1f;
  public float m_RotateSpeed = 1f;
  public float m_AdditionalGravity = 0.5f;
  public float m_LandingAccelerationRatio = 0.5f;
  // public bool reverse = false;  
  private Rigidbody m_RigidBody;
  // InputProcessing inputs;
  // SkateAnim anim;
  [HideInInspector] public Quaternion m_PhysicsRotation;
  [HideInInspector] public Quaternion m_VelocityRotation;
  [HideInInspector] public Quaternion m_InputRotation;
  [HideInInspector] public Quaternion m_ComputedRotation;
  private float m_Height;
  private Camera m_Camera;
  private bool m_IsAerial;

  // Use this for initialization
  void Start()
  {
    m_Camera = Camera.main;
    // TargetRotation = transform.rotation; 
    // VelocityRotation = Quaternion.identity; 
    m_RigidBody = GetComponent<Rigidbody>();
    inputs = GetComponent<InputProcessing>();
    anim = GetComponent<SkateAnim>();
    m_Height = GetComponent<Collider>().bounds.size.y / 2f;

  }

  // Update is called once per frame
  void Update()
  {
    CheckPhysics();

    Vector2 direction = inputs.GetDirection();
    SkaterMove(direction);

  }


  void CheckPhysics()
  {
    Ray ray = new Ray(transform.position, -transform.up);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, 1.05f * m_Height))
    {
      if (m_IsAerial)
      {
        VelocityOnLanding();
      }
      m_IsAerial = false;
    }
    else
    {
      m_IsAerial = true;
      m_RigidBody.velocity += Vector3.down * m_AdditionalGravity;
    }

  }

  void VelocityOnLanding()
  {
    float magn_vel = m_RigidBody.velocity.magnitude;
    Vector3 new_vel = m_RigidBody.velocity;
    new_vel.y = 0;
    new_vel = new_vel.normalized * magn_vel;

    m_RigidBody.velocity += m_LandingAccelerationRatio * new_vel;

  }


  void SkaterMove(Vector2 inputs)
  {

    PhysicsRotation = m_IsAerial ? Quaternion.identity : GetPhysicsRotation(); // Rotation according to ground normal 
    VelocityRotation = GetVelocityRot();
    InputRotation = Quaternion.identity;
    ComputedRotation = Quaternion.identity;


    if (inputs.magnitude > 0.1f)
    {
      Vector3 adapted_direction = CamToPlayer(inputs);
      Vector3 planar_direction = transform.forward;
      planar_direction.y = 0;
      InputRotation = Quaternion.FromToRotation(planar_direction, adapted_direction);

      if (!m_IsAerial)
      {
        Vector3 Direction = InputRotation * transform.forward * m_Speed;
        m_RigidBody.AddForce(Direction);
      }
    }

    ComputedRotation = PhysicsRotation * VelocityRotation * transform.rotation;
    transform.rotation = Quaternion.Lerp(transform.rotation, ComputedRotation, m_RotateSpeed * Time.deltaTime);
  }


  Quaternion GetVelocityRot()
  {
    Vector3 vel = m_RigidBody.velocity;
    if (vel.magnitude > 0.2f)
    {
      vel.y = 0;
      Vector3 dir = transform.forward;
      dir.y = 0;
      Quaternion vel_rot = Quaternion.FromToRotation(dir.normalized, vel.normalized);
      return vel_rot;
    }
    else
      return Quaternion.identity;
  }

  Quaternion GetPhysicsRotation()
  {
    Vector3 target_vec = Vector3.up;
    Ray ray = new Ray(transform.position, Vector3.down);
    RaycastHit hit;

    if (Physics.Raycast(ray, out hit, 1.05f * m_Height))
    {
      target_vec = hit.normal;
    }

    return Quaternion.FromToRotation(transform.up, target_vec);
  }

  Vector3 CamToPlayer(Vector2 d)
  {
    Vector3 cam_to_player = transform.position - m_Camera.transform.position;
    cam_to_player.y = 0;

    Vector3 cam_to_player_right = Quaternion.AngleAxis(90, Vector3.up) * cam_to_player;

    Vector3 direction = cam_to_player * d.y + cam_to_player_right * d.x;
    return direction.normalized;
  }

}