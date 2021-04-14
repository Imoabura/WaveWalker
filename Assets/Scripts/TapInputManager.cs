using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapInputManager : MonoBehaviour
{
    [SerializeField] int touchLimit = 10; // for 10 fingers
    [SerializeField] float tapTimeThreshold = .2f;

    float[] touchStartTime;
    Vector2[] touchStartPos;
    //bool[] touchMoved;

    // Start is called before the first frame update
    void Start()
    {
        touchStartTime = new float[touchLimit];
        //touchMoved = new bool[touchLimit];
        touchStartPos = new Vector2[touchLimit];
    }

    // Update is called once per frame
    void Update()
    {
        foreach (Touch t in Input.touches)
        {
            int idNum = t.fingerId;

            if (t.phase == TouchPhase.Began)
            {
                touchStartTime[idNum] = Time.time;
                //touchMoved[idNum] = false;
                touchStartPos[idNum] = t.position;
            }
            //if (t.phase == TouchPhase.Moved)
            //{
            //    if (t.deltaPosition.magnitude >= tapMoveThreshold)
            //    {
            //        float touchMovement = (touchStartPos[idNum] - t.position).magnitude;
            //        Debug.Log($"movement: {touchMovement}; deltaMovement: {t.deltaPosition.magnitude}");
            //        touchMoved[idNum] = true;
            //        Debug.Log("Movement!");
            //    }
            //}
            if (t.phase == TouchPhase.Ended)
            {
                float timeElapsed = Time.time - touchStartTime[idNum];
                if (timeElapsed <= tapTimeThreshold)
                {
                    Vector2 touchPos = t.position;
                    //Debug.Log($"Tap Recorded @ {touchPos}; Time Elapsed: {timeElapsed}");
                }
            }
        }
    }
}
