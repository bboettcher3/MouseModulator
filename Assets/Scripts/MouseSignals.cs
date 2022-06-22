using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseSignals : MonoBehaviour
{
  [HideInInspector]
  public float mouseXSpeed;
  [HideInInspector]
  public float mouseYSpeed;
  [HideInInspector]
  public float mouseSpeed;
  [HideInInspector]
  public float mouseAngleDeg;
  [HideInInspector]
  public float mouseAngleRad;

  private const int NUM_AVG_FRAMES = 20;

  private float[] mouseXSpeedBuffer = new float[NUM_AVG_FRAMES];
  private float[] mouseYSpeedBuffer = new float[NUM_AVG_FRAMES];
  private int curBufferIdx = 0;

  // Start is called before the first frame update
  void Start()
  {
    Cursor.lockState = CursorLockMode.Locked;
  }

  // Update is called once per frame
  void Update()
  {
    /* Get new mouse values */
    mouseXSpeedBuffer[curBufferIdx] = Input.GetAxis("Mouse X") / 4.0f;
    mouseYSpeedBuffer[curBufferIdx] = Input.GetAxis("Mouse Y") / 4.0f;

    /* Get moving averages of each signal */
    float mouseXSpeedSum = 0;
    float mouseYSpeedSum = 0;
    for (int i = 0; i < NUM_AVG_FRAMES; ++i)
    {
      mouseXSpeedSum += mouseXSpeedBuffer[i];
      mouseYSpeedSum += mouseYSpeedBuffer[i];
    }

    /* Update libmapper signals */
    mouseXSpeed = mouseXSpeedSum / NUM_AVG_FRAMES;
    mouseYSpeed = mouseYSpeedSum / NUM_AVG_FRAMES;
    mouseSpeed = Mathf.Sqrt(Mathf.Pow(mouseXSpeed, 2) + Mathf.Pow(mouseYSpeed, 2));
    mouseAngleRad = Mathf.Atan2(mouseYSpeed, mouseXSpeed);
    if (mouseAngleRad < 0.0f)
    {
      mouseAngleRad += 2 * Mathf.PI;
    }
    Debug.Log(mouseAngleRad);
    mouseAngleDeg = Mathf.Rad2Deg * mouseAngleRad;
    gameObject.GetComponent<MapperDevice>().setSignalValue("mouseXSpeed", mouseXSpeed);
    gameObject.GetComponent<MapperDevice>().setSignalValue("mouseYSpeed", mouseYSpeed);
    gameObject.GetComponent<MapperDevice>().setSignalValue("mouseSpeed", mouseSpeed);
    gameObject.GetComponent<MapperDevice>().setSignalValue("mouseAngleDeg", mouseAngleDeg);
    gameObject.GetComponent<MapperDevice>().setSignalValue("mouseAngleRad", mouseAngleRad);
    //Debug.Log("x: " + mouseX + ", y:" + mouseY + ", speed: " + mouseSpeed);

    curBufferIdx = (curBufferIdx + 1) % NUM_AVG_FRAMES;
  }
}
