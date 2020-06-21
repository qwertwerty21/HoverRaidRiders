using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Cinemachine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }
  private CinemachineVirtualCamera m_CinemachineVirtualCamera;
  void Awake()
  {
    Instance = this;
    m_CinemachineVirtualCamera = Camera.main.GetComponent<CinemachineVirtualCamera>();
  }

  public void SetFieldOfView(float newFOV)
  {
    float currentFOV = m_CinemachineVirtualCamera.m_Lens.FieldOfView;
    m_CinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.Slerp(currentFOV, newFOV, Time.deltaTime * 4f);
  }
}