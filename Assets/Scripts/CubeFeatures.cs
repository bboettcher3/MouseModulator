using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeFeatures : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
      float cubeScale = Camera.main.GetComponent<MapperDevice>().getSignalValue("cubeScale");
      Debug.Log("cubeScale: " + cubeScale);
    }
}
