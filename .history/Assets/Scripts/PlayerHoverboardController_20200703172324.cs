using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerHoverboardController : MonoBehaviour
{
  private Hoverboard m_Hoverboard;
  void Awake()
  {
    m_Hoverboard = GetComponent<Hoverboard>();
  }

  // Update is called once per frame
  void Update()
  {
    if (CrossPlatformInputManager.GetButtonDown("Jump"))
    {
      print("jump");
      m_Hoverboard.Jump();
    }
  }
  void FixedUpdate()
  {
    float vertical = CrossPlatformInputManager.GetAxis("Vertical");
    float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");

    m_Hoverboard.Move(horizontal, vertical);


  }
}
