using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinScript : MonoBehaviour {

    // Use this for initialization
    void Start () {
        
    }

    // Update is called once per frame
    void Update () {
        
    }

    public void MoveCoin()
    {
        float xRange = Random.Range(-8.18f, 8.18f);
        float yRange = Random.Range(2.1f, -4.26f);
        transform.position = new Vector2(xRange, yRange);
    }
}