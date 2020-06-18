using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerHoverboard2Controller : MonoBehaviour
{
  private Hoverboard2 m_Hoverboard;
  void Awake()
  {
    m_Hoverboard = GetComponent<Hoverboard2>();
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    float vertical = CrossPlatformInputManager.GetAxis("Vertical");
    float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");

    bool isDrifting = Input.GetKey(KeyCode.LeftShift);
    m_Hoverboard.Move(horizontal, vertical, isDrifting);

  }
}
