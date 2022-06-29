using UnityEngine;

public class ObjectModulation : MonoBehaviour
{
  private MouseSignals mouseSignals;
  private const float SPEED_SCALE = 2.0f;

  // Start is called before the first frame update
  void Start()
  {
    mouseSignals = GameObject.Find("Main Camera").GetComponent<MouseSignals>();
  }

  // Update is called once per frame
  void Update()
  {
    /* Mouse speed ring */
    Transform speedBall = gameObject.transform.Find("SpeedRing");
    speedBall.localScale = new Vector3(1.0f + mouseSignals.mouseXSpeed * SPEED_SCALE, 1.0f, 1.0f + mouseSignals.mouseYSpeed * SPEED_SCALE);

    /* Mouse angle sphere */
    Transform angleSphere = gameObject.transform.Find("AngleSphere");
    float angleSphereX = speedBall.localScale.x / 2.0f * Mathf.Cos(mouseSignals.mouseAngleRad);
    float angleSphereY = speedBall.localScale.z / 2.0f * Mathf.Sin(mouseSignals.mouseAngleRad);
    angleSphere.localPosition = new Vector3(angleSphereX, angleSphereY, 2.0f);

    /* Mouse position sphere */
    Transform positionSphere = gameObject.transform.Find("PositionSphere");
    positionSphere.localPosition = new Vector3(mouseSignals.mouseXPosition / 4.0f, mouseSignals.mouseYPosition / 4.0f, 1.9f);
  }
}
