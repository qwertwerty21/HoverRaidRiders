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
      print("Aim targetPoint " + targetPoint);
      m_Crosshair.m_CrosshairRectTransform.position = targetPoint;
    }

    // Update is called once per frame
    void Update()
    {
      // print("cros" + m_Crosshair);
      // m_Crosshair.m_CrosshairRectTransform.localPosition += Vector3.right;
    }
    void FixedUpdate()
    {
    }
  }
}