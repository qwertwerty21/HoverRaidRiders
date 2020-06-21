using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { public get; private set; }
  void Awake()
  {
    m_Animator = GetComponent<Animator>();
  }

  // Update is called once per frame
  void FixedUpdate()
  {
  }
}