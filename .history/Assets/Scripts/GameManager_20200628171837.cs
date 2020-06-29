using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Cinemachine;

public class GameManager : MonoBehaviour
{
  public static GameManager Instance { get; private set; }
  private CinemachineVirtualCamera m_CinemachineVirtualCamera;

  private Dictionary<string, MMFeedbacks> m_CameraFeedbacksHash = new Dictionary<string, MMFeedbacks>();

  void Awake()
  {
    Instance = this;
    m_CinemachineVirtualCamera = Camera.main.GetComponent<CinemachineVirtualCamera>();

    // add all feedbacks to hash for easy access
    MMFeedbacks[] feedbacks = Camera.main.GetComponentsInChildren<MMFeedbacks>();
    foreach (MMFeedbacks feedback in feedbacks)
    {
      m_FeedbacksHash.Add(feedback.gameObject.name, feedback);
    }
  }

  public void SetFieldOfView(float newFOV, float fovAdjustmentSpeed)
  {
    float currentFOV = m_CinemachineVirtualCamera.m_Lens.FieldOfView;
    m_CinemachineVirtualCamera.m_Lens.FieldOfView = Mathf.SmoothStep(currentFOV, newFOV, Time.deltaTime * fovAdjustmentSpeed);
  }

  public void PlayCameraFeedback(string name)
  {
    m_CameraFeedbacksHash[name].PlayFeedbacks();
  }
}