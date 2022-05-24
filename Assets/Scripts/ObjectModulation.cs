using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectModulation : MonoBehaviour
{
  Animation animation;
  MouseSignals mouseSignals;

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
    animation = gameObject.GetComponent<Animation>();
    mouseSignals = GameObject.Find("Main Camera").GetComponent<MouseSignals>();
    GameObject.Find("TurtleShell").GetComponent<SkinnedMeshRenderer>().material.EnableKeyword("_EMISSION");
  }

  // Update is called once per frame
  void Update()
  {
    
    // Update animation when mouse is clicked
    if (Input.GetMouseButtonDown(0))
    {
      int randIdx = Random.Range(0, animationNames.Length - 1);
      animation.Play(animationNames[randIdx], PlayMode.StopAll);
    }

    // Update color emission from mouse directions
    // intensity -> mouseX
    float mouseX = mouseSignals.mouseX / 10;
    float mouseY = mouseSignals.mouseY / 10;
    GameObject.Find("TurtleShell").GetComponent<SkinnedMeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(mouseX, mouseY));
    //Color color = Color.HSVToRGB(hue, 1, 1);
    // color hue -> mouseY
    
    //Debug.Log("h: " + hue + ", int: " + intensity);
    //color.a = intensity;
    //GameObject.Find("TurtleShell").GetComponent<SkinnedMeshRenderer>().material.SetColor("_EmissionColor", color);
    // material offset X -> mouseSpeed
    //float speed = Mathf.InverseLerp(0, 2, mouseSignals.mouseSpeed);
    //GameObject.Find("TurtleShell").GetComponent<SkinnedMeshRenderer>().material.SetTextureOffset("_MainTex", new Vector2(speed, 0));
  }
}
