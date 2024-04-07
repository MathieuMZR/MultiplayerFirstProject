using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Light))]
public class ShinyLight : MonoBehaviour
{
    [SerializeField] Gradient gradient;
    [SerializeField] AnimationCurve gradientCurve;
    [SerializeField] float speed = 1f;

    private float gradientTime;
    private Light light;

    private void Awake()
    {
        light = GetComponent<Light>();
    }

    // Update is called once per frame
    void Update()
    {
        light.color = gradient.Evaluate(gradientCurve.Evaluate(gradientTime));
        gradientTime += Time.deltaTime * speed;
    }

    private void OnDisable()
    {
        light.color = Color.white;
    }
}
