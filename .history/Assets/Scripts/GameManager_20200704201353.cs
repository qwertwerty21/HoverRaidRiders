using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Cinemachine;
using MoreMountains.Feedbacks;


namespace HoverRaidRiders
{
  public class GameManager : MonoBehaviour
  {
    public static GameManager Instance { get; private set; }

    public Camera m_MainCamera;
    private CinemachineVirtualCamera m_CinemachineVirtualCamera;

    private Dictionary<string, MMFeedbacks> m_CameraFeedbacksHash = new Dictionary<string, MMFeedbacks>();

    void Awake()
    {
      Instance = this;
      m_MainCamera = Camera.main;
      m_CinemachineVirtualCamera = m_MainCamera.GetComponent<CinemachineVirtualCamera>();

      // add all feedbacks to hash for easy access
      MMFeedbacks[] feedbacks = m_MainCamera.GetComponentsInChildren<MMFeedbacks>();
      foreach (MMFeedbacks feedback in feedbacks)
      {
        m_CameraFeedbacksHash.Add(feedback.gameObject.name, feedback);
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
}