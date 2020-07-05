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
    private GameObject m_HoverboardGameObject;
    void Awake()
    {
      m_Animator = GetComponent<Animator>();
      m_Crosshair = GetComponent<Crosshair>();
      m_HoverboardGameObject = GameObject.FindGameObjectWithTag("Hoverboard");
      transform.parent = m_HoverboardGameObject.transform;
    }

    public void Aim(Vector3 targetPoint)
    {
      print("Aim targetPoint " + targetPoint);

      // clamp targetPoint by screen width and height
      Vector3 clampedTargetPoint = targetPoint;
      clampedTargetPoint.x = Mathf.Clamp(clampedTargetPoint.x, 0f, Screen.width);
      clampedTargetPoint.y = Mathf.Clamp(clampedTargetPoint.y, 0f, Screen.height);

      m_Crosshair.m_CrosshairRectTransform.anchoredPosition3D = clampedTargetPoint;
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