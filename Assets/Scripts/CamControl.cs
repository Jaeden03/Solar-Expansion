using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CamControl : MonoBehaviour
{
    private Camera myCam;
    public Slider zoomSlide;
    public float minScale;
    public float maxScale;

    public float moveTime = 1f;
    private float moveTimeCurrent = 1f;
    //private Vector3 newSpot = Vector3.zero;
    private Vector3 lastSpot = Vector3.zero;

    private Planet lastTarget;
    private Vector3 lastPoint = Vector3.zero;
    private float yOffset;

    private float minMoveValue = 1000f;

    //private Vector3 lookLocation;

    // Start is called before the first frame update
    void Start()
    {
        yOffset = transform.position.y;
        
        lastSpot.y = yOffset;
        lastPoint.y = yOffset;

        ChangeZoomSlideValue();
    }

    // Update is called once per frame
    void Update()
    {
        //ChangeZoomSlideValue();
        
        if (lastTarget != SolarSystem.currentlySelected) 
        {
            lastTarget = SolarSystem.currentlySelected;
            moveTimeCurrent = 0f;
            lastSpot = transform.position;
            //lastSpot = lookLocation;
            
            lastPoint = lastSpot;
        }
        else if(lastTarget)
        {
            if (Vector3.Distance(lastPoint, lastTarget.transform.position) > minMoveValue)
            { moveTimeCurrent = 0f; lastSpot = transform.position; } //lastSpot = lookLocation; }
        } 

        var target = Vector3.zero;
        if(SolarSystem.currentlySelected) 
        { target = SolarSystem.currentlySelected.transform.position; }
        
        target.y = yOffset;

        if (moveTimeCurrent <= moveTime) { moveTimeCurrent += Time.deltaTime; }

        transform.position = Vector3.Lerp(lastSpot, target, moveTimeCurrent / moveTime);
        //lookLocation = Vector3.Lerp(lastSpot, target, moveTimeCurrent / moveTime);
        //transform.LookAt(lookLocation);

    }

    
    public void ChangeZoomSlideValue()
    {
        if (zoomSlide) { SetZoom(zoomSlide.value); }
    }
    

    public void SetZoom(float val)
    {
        SolarSystem.screenScale =  val * SolarSystem.zoomWeight.Evaluate(val);

        if (!myCam) { myCam = GetComponent<Camera>(); }
        myCam.orthographicSize = Mathf.Lerp(minScale, maxScale, SolarSystem.screenScale);

        //Debug.Log(val);
    }

    public void ZoomInOut(float percent)
    {
        var val = zoomSlide.value * percent;
        if (zoomSlide) { zoomSlide.value += val; SetZoom(zoomSlide.value); }
    }


}
