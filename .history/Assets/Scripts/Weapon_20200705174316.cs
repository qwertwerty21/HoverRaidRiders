using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoverRaidRiders
{
  public abstract class Weapon : MonoBehaviour
  {
    public float m_CooldownDuration = 1f;
    public abstract void Attack();
  }

}