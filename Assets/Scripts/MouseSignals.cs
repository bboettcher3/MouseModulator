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

  private int numIgnoreZeroFrames = 40;
  private int curZeroFramesIgnored = 0;

  // Start is called before the first frame update
  void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
  }

  // Update is called once per frame
  void Update()
  {
    /* Update mouse values */
    float newMouseX = Input.GetAxis("Mouse X");
    float newMouseY = Input.GetAxis("Mouse Y");
    float newMouseSpeed = Mathf.Sqrt(Mathf.Pow(newMouseX, 2) + Mathf.Pow(newMouseY, 2));
    if (newMouseSpeed == 0)
    {
      curZeroFramesIgnored++;
    } else
    {
      curZeroFramesIgnored = 0;
    }

    /* Update libmapper signals */
    if (newMouseSpeed != 0 || (newMouseSpeed == 0 && curZeroFramesIgnored >= numIgnoreZeroFrames))
    {
      mouseX = newMouseX;
      mouseY = newMouseY;
      mouseSpeed = newMouseSpeed;
      gameObject.GetComponent<MapperDevice>().setSignalValue("mouseX", mouseX);
      gameObject.GetComponent<MapperDevice>().setSignalValue("mouseY", mouseY);
      gameObject.GetComponent<MapperDevice>().setSignalValue("mouseSpeed", mouseSpeed);
      //Debug.Log("x: " + mouseX + ", y:" + mouseY + ", speed: " + mouseSpeed);
    }

  }
}
