using System;
using System.Collections.Generic;
using UnityEngine;

public class MouseSignals : MonoBehaviour
{
  [HideInInspector]
  public float mouseXPosition;
  [HideInInspector]
  public float mouseYPosition;
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
  [HideInInspector]
  public float mouseLeftClick;
  [HideInInspector]
  public float mouseRightClick;
  [HideInInspector]
  public float mouseScrollClick;
  [HideInInspector]
  public float mouseScrollPosition;
  [HideInInspector]
  public float mouseScrollSpeed;

  enum MouseSignal
  {
    MOUSE_X_POSITION = 0,
    MOUSE_Y_POSITION,
    MOUSE_X_SPEED,
    MOUSE_Y_SPEED,
    MOUSE_SPEED,
    MOUSE_ANGLE_DEG,
    MOUSE_ANGLE_RAD,
    MOUSE_LEFT_CLICK,
    MOUSE_RIGHT_CLICK,
    MOUSE_SCROLL_CLICK,
    MOUSE_SCROLL_POSITION,
    MOUSE_SCROLL_SPEED,
    NUM_SIGNALS
  };

  /* Libmapper device and signals */
  private IntPtr dev = IntPtr.Zero;
  private IntPtr[] sigs = new IntPtr[(int)MouseSignal.NUM_SIGNALS];

  private const int NUM_AVG_FRAMES = 20;
  private const float POSITION_SPEED_SCALAR = 0.04f;

  private float[] mouseXSpeedBuffer = new float[NUM_AVG_FRAMES];
  private float[] mouseYSpeedBuffer = new float[NUM_AVG_FRAMES];
  private float[] mouseScrollSpeedBuffer = new float[NUM_AVG_FRAMES];
  private int curBufferIdx = 0;

  // Start is called before the first frame update
  void Start()
  {
    dev = mpr.mpr_dev_new("MouseModulator");
    sigs[(int)MouseSignal.MOUSE_X_POSITION] = mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseXPosition", 1, mpr.Type.FLOAT, -1.0f, 1.0f);
    sigs[(int)MouseSignal.MOUSE_Y_POSITION] = mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseYPosition", 1, mpr.Type.FLOAT, -1.0f, 1.0f);
    sigs[(int)MouseSignal.MOUSE_X_SPEED] = mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseXSpeed", 1, mpr.Type.FLOAT, 0.0f, 1.0f);
    sigs[(int)MouseSignal.MOUSE_Y_SPEED] = mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseYSpeed", 1, mpr.Type.FLOAT, 0.0f, 1.0f);
    sigs[(int)MouseSignal.MOUSE_SPEED] = mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseSpeed", 1, mpr.Type.FLOAT, 0.0f, 1.0f);
    sigs[(int)MouseSignal.MOUSE_ANGLE_DEG] = mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseAngleDeg", 1, mpr.Type.FLOAT, 0.0f, 360.0f);
    sigs[(int)MouseSignal.MOUSE_ANGLE_RAD] = mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseAngleRad", 1, mpr.Type.FLOAT, 0.0f, 6.283f);
    sigs[(int)MouseSignal.MOUSE_LEFT_CLICK] = mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseLeftClick", 1, mpr.Type.INT, 0, 1);
    sigs[(int)MouseSignal.MOUSE_RIGHT_CLICK] = mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseRightClick", 1, mpr.Type.INT, 0, 1);
    sigs[(int)MouseSignal.MOUSE_SCROLL_CLICK] = mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseScrollClick", 1, mpr.Type.INT, 0, 1);
    sigs[(int)MouseSignal.MOUSE_SCROLL_POSITION] = mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseScrollPosition", 1, mpr.Type.FLOAT, -1.0f, 1.0f);
    sigs[(int)MouseSignal.MOUSE_SCROLL_SPEED] = mpr.mpr_sig_new(dev, mpr.Direction.OUTGOING, "mouseScrollSpeed", 1, mpr.Type.FLOAT, 0.0f, 1.0f);
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
    mouseXSpeedBuffer[curBufferIdx] = Input.GetAxis("Mouse X") / 2.0f;
    mouseYSpeedBuffer[curBufferIdx] = Input.GetAxis("Mouse Y") / 2.0f;
    mouseScrollSpeedBuffer[curBufferIdx] = Mathf.Abs(Input.mouseScrollDelta.y);

    /* Get moving averages of each signal */
    float mouseXSpeedSum = 0;
    float mouseYSpeedSum = 0;
    float mouseXSpeedAbsSum = 0;
    float mouseYSpeedAbsSum = 0;
    float mouseScrollSpeedSum = 0;
    for (int i = 0; i < NUM_AVG_FRAMES; ++i)
    {
      mouseXSpeedSum += mouseXSpeedBuffer[i];
      mouseYSpeedSum += mouseYSpeedBuffer[i];
      mouseXSpeedAbsSum += Mathf.Abs(mouseXSpeedBuffer[i]);
      mouseYSpeedAbsSum += Mathf.Abs(mouseYSpeedBuffer[i]);
      mouseScrollSpeedSum += mouseScrollSpeedBuffer[i];
    }

    float signedMouseXSpeed = mouseXSpeedSum / NUM_AVG_FRAMES;
    float signedMouseYSpeed = mouseYSpeedSum / NUM_AVG_FRAMES;

    /* Speed values (absolute values) */
    mouseXSpeed = mouseXSpeedAbsSum / NUM_AVG_FRAMES;
    mouseYSpeed = mouseYSpeedAbsSum / NUM_AVG_FRAMES;
    mouseSpeed = Mathf.Sqrt(Mathf.Pow(mouseXSpeed, 2) + Mathf.Pow(mouseYSpeed, 2));
    mouseScrollSpeed = mouseScrollSpeedSum / NUM_AVG_FRAMES;

    /* Position values (directional) */
    mouseXPosition = Mathf.Clamp(mouseXPosition + (signedMouseXSpeed * POSITION_SPEED_SCALAR), -1.0f, 1.0f);
    mouseYPosition = Mathf.Clamp(mouseYPosition + (signedMouseYSpeed * POSITION_SPEED_SCALAR), -1.0f, 1.0f);
    mouseScrollPosition = Mathf.Clamp(mouseScrollPosition + (mouseScrollSpeed * POSITION_SPEED_SCALAR), -1.0f, 1.0f);

    /* Angle values */
    if (mouseSpeed > 0.01f)
    {
      /* Have to use signed values to compute angles */
      mouseAngleRad = Mathf.Atan2(signedMouseYSpeed, signedMouseXSpeed);
      if (mouseAngleRad < 0.0f)
      {
        mouseAngleRad += 2 * Mathf.PI;
      }
      mouseAngleDeg = Mathf.Rad2Deg * mouseAngleRad;
    }

    /* Click values */
    mouseLeftClick = Input.GetMouseButton(0) ? 1 : 0;
    mouseRightClick = Input.GetMouseButton(1) ? 1 : 0;
    mouseScrollClick = Input.GetMouseButton(2) ? 1 : 0;
    
    /* Update libmapper signal values */
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_X_POSITION], mouseXPosition);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_Y_POSITION], mouseYPosition);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_X_SPEED], mouseXSpeed);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_Y_SPEED], mouseYSpeed);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_SPEED], mouseSpeed);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_ANGLE_DEG], mouseAngleDeg);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_ANGLE_RAD], mouseAngleRad);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_LEFT_CLICK], mouseLeftClick);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_RIGHT_CLICK], mouseRightClick);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_SCROLL_CLICK], mouseScrollClick);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_SCROLL_POSITION], mouseScrollPosition);
    mpr.mpr_sig_set_value(sigs[(int)MouseSignal.MOUSE_SCROLL_SPEED], mouseScrollSpeed);

    curBufferIdx = (curBufferIdx + 1) % NUM_AVG_FRAMES; // Increment moving average buffer index
  }

  void OnApplicationFocus(bool hasFocus)
  {
    /* Only lock cursor if application is focused */
    Cursor.lockState = hasFocus ? CursorLockMode.Locked : CursorLockMode.None;
  }
}