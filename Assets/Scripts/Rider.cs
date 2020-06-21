using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Rider : MonoBehaviour
{
  public Animator m_Animator;
  void Awake()
  {
    m_Animator = GetComponent<Animator>();
  }

  // Update is called once per frame
  void FixedUpdate()
  {
  }
}