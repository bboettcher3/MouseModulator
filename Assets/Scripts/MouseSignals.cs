using System;
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

  enum MouseSignal
  {
    MOUSE_X_SPEED = 0,
    MOUSE_Y_SPEED,
    MOUSE_SPEED,
    MOUSE_ANGLE_DEG,
    MOUSE_ANGLE_RAD
  };

  /* Libmapper device and signals */
  private IntPtr dev = IntPtr.Zero;
  private List<IntPtr> sigs = new List<IntPtr>();

  private const int NUM_AVG_FRAMES = 20;

  private float[] mouseXSpeedBuffer = new float[NUM_AVG_FRAMES];
  private float[] mouseYSpeedBuffer = new float[NUM_AVG_FRAMES];
  private int curBufferIdx = 0;

  // Start is called before the first frame update
  void Start()
  {
    dev = mpr.mpr_dev_new("MouseModulator");
    sigs.Add(mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseXSpeed", 1, mpr.Type.FLOAT, 0.0f, 1.0f));
    sigs.Add(mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseYSpeed", 1, mpr.Type.FLOAT, 0.0f, 1.0f));
    sigs.Add(mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseSpeed", 1, mpr.Type.FLOAT, 0.0f, 1.0f));
    sigs.Add(mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseAngleDeg", 1, mpr.Type.FLOAT, 0.0f, 360.0f));
    sigs.Add(mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseAngleRad", 1, mpr.Type.FLOAT, 0.0f, 6.283f));
  }

  // Update is called once per frame
  void Update()
  {
    
    if (Input.GetKeyDown(KeyCode.Escape))
    {
      /* Unlock mouse if ESC pressed */
      Cursor.lockState = CursorLockMode.None;
    } else if (Input.GetMouseButtonDown(0) && Cursor.lockState == CursorLockMode.None && Application.isFocused)
    {
      /* Re-lock mouse if mouse clicked while unlocked and focused */
      Cursor.lockState = CursorLockMode.Locked;
    }

    /* Update signal values */
    mpr.mpr_dev_poll(dev);

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
    mouseAngleDeg = Mathf.Rad2Deg * mouseAngleRad;
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_X_SPEED], mouseXSpeed);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_Y_SPEED], mouseYSpeed);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_SPEED], mouseSpeed);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_ANGLE_DEG], mouseAngleDeg);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_ANGLE_RAD], mouseAngleRad);

    curBufferIdx = (curBufferIdx + 1) % NUM_AVG_FRAMES;
  }

  void OnApplicationFocus(bool hasFocus)
  {
    /* Only lock cursor if application is focused */
    Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
  }
}