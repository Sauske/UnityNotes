using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ActionTest : MonoBehaviour {

	// Use this for initialization
	void Start () {

        Action<int> action = Test();
        action(10);
        action(100);
        action(1000);
	}


    Action<int> Test()
    {
        int count = 0;
        Action<int> action = delegate (int number)
        {
            count += number;
            Debug.Log(count);
        };

        action(1);
        return action;
    }
}
