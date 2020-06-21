using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { public get; private set; }
  void Awake()
  {
    Instance = this;
  }

  // Update is called once per frame
  void FixedUpdate()
  {
  }
}