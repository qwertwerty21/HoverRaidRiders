﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hoverboard : MonoBehaviour
{


  // Start is called before the first frame update
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {
    float turn = Input.GetAxis("Horizontal");
    rb.AddTorque(transform.up * torque * turn);
  }
}
