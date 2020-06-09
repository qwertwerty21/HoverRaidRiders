using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerHoverboardController : MonoBehaviour
{
  private Hoverboard m_Hoverboard;
  void Awake()
  {
    m_Hoverboard = GetComponentInChildren<Hoverboard>();
    transform.parent = m_Hoverboard.transform;
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    float vertical = CrossPlatformInputManager.GetAxis("Vertical");
    float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");

    m_Hoverboard.Move(horizontal, vertical);

  }
}
