using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoverRaidRiders
{
  public interface IWeapon
  {
    float m_CooldownDuration { get; }
    void Attack();
  }

}