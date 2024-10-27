using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WebGL_View : MonoBehaviour
{    
    public bool WebGL_Object; //true = active in webgl build, false = inactive in webgl build 
    void Start()
    {
        gameObject.SetActive(WebGL_Object == WebGLManager.WebGL_Build);
    }
     
    void Update()
    {

    }
}
