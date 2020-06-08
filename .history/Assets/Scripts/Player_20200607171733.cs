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

  private Camera m_Camera;


  public bool reverse = false;

  [HideInInspector] public Rigidbody rb;
  InputProcessing inputs;
  SkateAnim anim;
  float height;

  public bool aerial;

  [HideInInspector] public Quaternion PhysicsRotation;
  [HideInInspector] public Quaternion VelocityRotation;
  [HideInInspector] public Quaternion InputRotation;
  [HideInInspector] public Quaternion ComputedRotation;

  // Use this for initialization
  void Start()
  {

    Initialization();

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

    if (Physics.Raycast(ray, out hit, 1.05f * height))
    {
      if (aerial)
      {
        VelocityOnLanding();
      }
      aerial = false;
    }
    else
    {
      aerial = true;
      rb.velocity += Vector3.down * m_AdditionalGravity;
    }

  }

  void VelocityOnLanding()
  {
    float magn_vel = rb.velocity.magnitude;
    Vector3 new_vel = rb.velocity;
    new_vel.y = 0;
    new_vel = new_vel.normalized * magn_vel;

    rb.velocity += m_LandingAccelerationRatio * new_vel;

  }


  void SkaterMove(Vector2 inputs)
  {

    PhysicsRotation = aerial ? Quaternion.identity : GetPhysicsRotation(); // Rotation according to ground normal 
    VelocityRotation = GetVelocityRot();
    InputRotation = Quaternion.identity;
    ComputedRotation = Quaternion.identity;


    if (inputs.magnitude > 0.1f)
    {
      Vector3 adapted_direction = CamToPlayer(inputs);
      Vector3 planar_direction = transform.forward;
      planar_direction.y = 0;
      InputRotation = Quaternion.FromToRotation(planar_direction, adapted_direction);

      if (!aerial)
      {
        Vector3 Direction = InputRotation * transform.forward * m_Speed;
        rb.AddForce(Direction);
      }
    }

    ComputedRotation = PhysicsRotation * VelocityRotation * transform.rotation;
    transform.rotation = Quaternion.Lerp(transform.rotation, ComputedRotation, m_RotateSpeed * Time.deltaTime);
  }


  Quaternion GetVelocityRot()
  {
    Vector3 vel = rb.velocity;
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

    if (Physics.Raycast(ray, out hit, 1.05f * height))
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

  void Initialization()
  {
    m_Camera = Camera.main;
    // TargetRotation = transform.rotation; 
    // VelocityRotation = Quaternion.identity; 
    rb = GetComponent<Rigidbody>();
    inputs = GetComponent<InputProcessing>();
    anim = GetComponent<SkateAnim>();
    height = GetComponent<Collider>().bounds.size.y / 2f;
  }
}