using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HoverRaidRiders
{
  public class Weapon : MonoBehaviour
  {
    public float m_CooldownTime = 1f;
    private bool canAttack = true;

    virtual public void Attack()
    {

    }
  }

}