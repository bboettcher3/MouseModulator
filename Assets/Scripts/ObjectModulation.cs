using UnityEngine;

public class ObjectModulation : MonoBehaviour
{
  public int smoothingPerc = 90;
  private MouseSignals mouseSignals;

  string[] animationNames =
    { "Attack01",
    "Attack02",
    "Defend",
    "DefendGetHit",
    "Die",
    "Dizzy",
    "GetHit",
    "RunFWD",
    "SenseSomethingRPT",
    "SenseSomethingST",
    "Take 001",
    "Taunt",
    "Victory",
    "WalkBWD",
    "WalkFWD",
    "WalkLeft",
    "WalkRight"};

  // Start is called before the first frame update
  void Start()
  {
    mouseSignals = GameObject.Find("Main Camera").GetComponent<MouseSignals>();
  }

  // Update is called once per frame
  void FixedUpdate()
  {

    // Update color emission from mouse directions
    // intensity -> mouseX
    float mouseX = mouseSignals.mouseXSpeed * 12;
    float mouseY = mouseSignals.mouseYSpeed * 12;
    float mouseSpeed = mouseSignals.mouseSpeed;
    gameObject.transform.localPosition = 
      Vector3.Lerp(gameObject.transform.localPosition, new Vector3(mouseX, mouseY, 10f), 1 - (smoothingPerc / 100.0f));
  }
}
