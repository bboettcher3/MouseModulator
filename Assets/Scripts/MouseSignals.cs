using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSignals : MonoBehaviour
{
  [HideInInspector]
  public float mouseX;
  [HideInInspector]
  public float mouseY;
  [HideInInspector]
  public float mouseSpeed;

  // Start is called before the first frame update
  void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
  }

  // Update is called once per frame
  void Update()
  {
    /* Update mouse values */
    mouseX = Input.GetAxis("Mouse X");
    mouseY = Input.GetAxis("Mouse Y");
    mouseSpeed = Mathf.Sqrt(Mathf.Pow(mouseX, 2) + Mathf.Pow(mouseX, 2));

    /* Update libmapper signals */
    gameObject.GetComponent<MapperDevice>().setSignalValue("mouseX", mouseX);
    gameObject.GetComponent<MapperDevice>().setSignalValue("mouseY", mouseY);
    gameObject.GetComponent<MapperDevice>().setSignalValue("mouseSpeed", mouseSpeed);

    //Debug.Log("x: " + mouseX + ", y:" + mouseY + ", speed: " + mouseSpeed);


  }
}
