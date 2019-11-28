using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightFlicker : MonoBehaviour
{
    Light light;
    public float minIntensity = 1;
    public float maxIntensity = 15;
    [Range(1, 50)]
    public int smoothing = 12;

    Queue<float> smoothQueue;
    float lastSum = 0;

    public void Reset()
    {
        smoothQueue.Clear();
        lastSum = 0;
    }

    void Start()
    {
        smoothQueue = new Queue<float>(smoothing);
        if (light == null)
        {
            light = this.gameObject.GetComponent<Light>();
        }
    }

    void Update()
    {
        if (light == null)
            return;
        while (smoothQueue.Count >= smoothing)
        {
            lastSum -= smoothQueue.Dequeue();
        }
        float newVal = Random.Range(minIntensity, maxIntensity);
        smoothQueue.Enqueue(newVal);
        lastSum += newVal;
        light.intensity = lastSum / (float)smoothQueue.Count;
    }
}
