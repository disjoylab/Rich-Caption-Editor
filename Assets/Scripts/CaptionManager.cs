using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CaptionManager : MonoBehaviour
{
    //Manage instances of CaptionRenderer
    public static CaptionManager Instance;
    public GameObject CaptionPrefab;

    private void Awake()
    {
        Instance = this;
    }

    public CaptionRenderer CreateCaptionRenderer()
    {
        GameObject go = Instantiate(Instance.CaptionPrefab, transform);
        return go.GetComponent<CaptionRenderer>();
    }
}
