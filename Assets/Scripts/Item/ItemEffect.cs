using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemEffect : MonoBehaviour
{    public float floatHeight = 0.5f;
    public float floatTime = 0.2f;
    public float fallTime = 0.2f;
    public float swingAmount = 0.2f;
    public float swingSpeed = 3f;
    public float pulseScale = 1.3f;
    public float pulseSpeed = 5f;

    private Vector3 startPos;
    private Vector3 peakPos;
    private Vector3 endPos;
    private float timer = 0f;

    private enum State { Rising, Falling, Idle }
    private State currentState = State.Rising;

    private Vector3 baseScale;

    void Start()
    {
        startPos = transform.position;
        peakPos = startPos + Vector3.up * floatHeight;
        endPos = startPos;
        baseScale = transform.localScale;
    }

    void Update()
    {
        timer += Time.deltaTime;

        if (currentState == State.Rising)
        {
            float t = Mathf.Clamp01(timer / floatTime);
            transform.position = Vector3.Lerp(startPos, peakPos, t);
            transform.localScale = Vector3.Lerp(baseScale * pulseScale, baseScale, t);

            if (t >= 1f)
            {
                timer = 0f;
                currentState = State.Falling;
            }
        }
        else if (currentState == State.Falling)
        {
            float t = Mathf.Clamp01(timer / fallTime);
            transform.position = Vector3.Lerp(peakPos, endPos, t);

            if (t >= 1f)
            {
                timer = 0f;
                currentState = State.Idle;
            }
        }
        else if (currentState == State.Idle)
        {
           
            transform.position = endPos;
        }
    }


}
