using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apploader : MonoBehaviour
{
    void Start()
    {
        ApplicationMgr.S.Init();
    }
}
