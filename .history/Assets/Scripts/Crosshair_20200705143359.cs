using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoverRaidRiders
{
  public class Crosshair : MonoBehaviour
  {
    public string m_CrosshairName;
    public RectTransform m_CrosshairRectTransform;
    private Sprite m_CrosshairSprite;
    private Image m_CrosshairImage;

    public void EnableCrosshair()
    {
      m_CrosshairSprite = Resources.Load<Sprite>(m_CrosshairName) as Sprite;
      GameObject CrosshairGameObject = GameObject.FindGameObjectWithTag("Crosshair");
      m_CrosshairImage = CrosshairGameObject.GetComponent<Image>();
      m_CrosshairImage.sprite = m_CrosshairSprite;
      m_CrosshairRectTransform = CrosshairGameObject.GetComponent<RectTransform>();
    }

    public void DisableCrosshair()
    {
      m_CrosshairImage.sprite = null;
    }

    public void SetCrosshairColor(Color color)
    {
      if (m_CrosshairImage != null)
      {
        m_CrosshairImage.color = color;
      }
    }
    private void OnEnable()
    {
      this.EnableCrosshair();
    }
  }

}