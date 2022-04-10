using System.Collections;
using System.Collections.Generic;
using Majic.CM;
using UnityEngine;

public class RKTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameObject.Instantiate(ResourceFacade.instance.LoadPrefab("RKPrefab"));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
