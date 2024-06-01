using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TransitionStar : MonoBehaviour
{
    [SerializeField] private Image fade;
    [SerializeField] private GameObject fadeFloatRef;
    
    private float FadeAmount {
        set => fade.material.SetFloat("_Size", value);
    }
    
    private void Start()
    {
        Material mat = Instantiate(fade.material);
        fade.material = mat;
    }
    
    private void Update()
    {
        //Need to be between 0 and 500
        FadeAmount = fadeFloatRef.transform.localPosition.x;
    }
}
