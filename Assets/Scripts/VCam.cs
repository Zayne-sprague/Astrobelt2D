using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class VCam : MonoBehaviour
{
    Player player;
    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    // Update is called once per frame
    void Update()
    {
    }
}
