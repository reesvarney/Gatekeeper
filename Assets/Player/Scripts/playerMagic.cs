using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMagic : MonoBehaviour
{
    public float level { get; private set; } = 0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void addMagic(float num){
        level += num;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
