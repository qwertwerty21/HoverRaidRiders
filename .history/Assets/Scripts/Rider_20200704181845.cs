using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace HoverRaidRiders
{
  public class Rider : MonoBehaviour
  {
    public Animator m_Animator;
    public Crosshair m_Crosshair;
    void Awake()
    {
      m_Animator = GetComponent<Animator>();
      m_Crosshair = GetComponent<Crosshair>();
    }

    public void Aim(Vector3 targetPoint)
    {

    }

    // Update is called once per frame
    void Update()
    {
      m_Crosshair.GetComponent<RectTransform>().localPosition += Vector3.right;
    }
    void FixedUpdate()
    {
    }
  }
}