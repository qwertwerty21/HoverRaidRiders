using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Cinemachine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { public get; private set; }
  public CinemachineVirtualCamera m_CinemachineVirtualCamera;
  void Awake()
  {
    Instance = this;
    m_CinemachineVirtualCamera = Camera.main;
  }

  public void SetFieldOfView(float newFOV)
  {
    m_CinemachineVirtualCamera.m_Lens.FieldOfView = newFOV;
  }
}