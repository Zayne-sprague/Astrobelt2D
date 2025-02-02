﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    public IntRange time_to_twinkle = new IntRange(0, 10);

    private float twinkle_time = 1;
    private float time;

    Animator myanimator;
    // Start is called before the first frame update
    void Start()
    {
        myanimator = GetComponent<Animator>();

        twinkle_time = (float)time_to_twinkle.Random;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        if (time > twinkle_time)
        {
            if (time_to_twinkle.Random > 3)
            {
                myanimator.SetTrigger("twinkle");
            }
            time = 0;

        }
    }
}
