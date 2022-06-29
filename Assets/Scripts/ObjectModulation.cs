using UnityEngine;

public class ObjectModulation : MonoBehaviour
{
  private MouseSignals mouseSignals;
  private const float SPEED_SCALE = 2.0f;
  private const float SCROLL_POS_RANGE = 0.2f;

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
    angleSphere.localPosition = new Vector3(angleSphereX, angleSphereY, 1.9f);

    /* Mouse position sphere */
    Transform positionSphere = gameObject.transform.Find("PositionSphere");
    positionSphere.localPosition = new Vector3(mouseSignals.mouseXPosition / 4.0f, mouseSignals.mouseYPosition / 4.0f, 1.9f);

    /* Left/right/scroll click cubes */
    Transform leftClickCube = gameObject.transform.Find("LeftClickCube");
    leftClickCube.gameObject.SetActive(mouseSignals.mouseLeftClick == 1 ? true : false);
    Transform rightClickCube = gameObject.transform.Find("RightClickCube");
    rightClickCube.gameObject.SetActive(mouseSignals.mouseRightClick == 1 ? true : false);
    Transform scrollClickCube = gameObject.transform.Find("ScrollClickCube");
    scrollClickCube.gameObject.SetActive(mouseSignals.mouseScrollClick == 1 ? true : false);

    /* Scroll position cube */
    Transform scrollPositionCube = gameObject.transform.Find("ScrollPositionCube");
    scrollPositionCube.localPosition = new Vector3(0, mouseSignals.mouseScrollPosition * SCROLL_POS_RANGE, 1.99f);
    scrollPositionCube.localScale = new Vector3(0.5f + mouseSignals.mouseScrollSpeed, 0.05f, 0.1f);
  }
}
