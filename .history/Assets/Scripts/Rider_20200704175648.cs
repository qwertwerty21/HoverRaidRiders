using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace HoverRaidRiders
{

  public class Rider : MonoBehaviour
  {
    public Animator m_Animator;
    void Awake()
    {
      m_Animator = GetComponent<Animator>();
    }

    public void Aim(Vector3 targetPoint)
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
    void FixedUpdate()
    {
    }
  }
}