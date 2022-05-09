using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CursorController : MonoBehaviour
{

    const int _TOUCH_ = 0;
    const int _MOUSE_ = 1;
    public GameObject cursor;
    public string sceneType;

    float time;

    float delay;
    private TrailRenderer trail;
    AppStore appStore;
    private RuntimePlatform platform;

    private Vector3 mousePosition;

    private Vector3 virtualKeyPosition;
    public float moveSpeed = 0.1f;

    // Start is called before the first frame update
    private void Start()
    {
        appStore = new AppStore();
        trail =cursor.GetComponent<TrailRenderer>();

        platform = Application.platform;

        //delay = PlayerPrefs.GetFloat("puzzle_delay");

        //time= PlayerPrefs.GetFloat("drawing_delay");


        delay = appStore.PointerDelay;
        time = appStore.DrawingHaloVanishingTime;
    }
    // Update is called once per frame
    void Update()
    {
        int INPUT_TYPE = _MOUSE_;

        if (platform == RuntimePlatform.Android || platform == RuntimePlatform.IPhonePlayer)
        {
            if (Input.touchCount > 0)
            {
                virtualKeyPosition = new Vector3(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
                INPUT_TYPE = _TOUCH_;
            }
        }
        else
        {
            if (Input.GetMouseButton(0))
            {
                virtualKeyPosition = new Vector3(Input.mousePosition.x, Input.mousePosition.y);

            }
        }
        SpawnLight2Pointer(virtualKeyPosition, INPUT_TYPE);


    }

    private void SpawnLight2Pointer(Vector3 pointerPosition, int INPUT_TYPE)
    {
        Vector3 currentPosition = Camera.main.ScreenToWorldPoint(pointerPosition);
        Vector3 cursorPosition = cursor.GetComponent<Transform>().position;
        currentPosition.z = 1;// Puts the z coordinates at 1 so it is visible to the camera.

        if (INPUT_TYPE==_TOUCH_)
        {

            Touch fireTouch = Input.GetTouch(0);
            switch (sceneType)
            {
                
                case "puzzle":
                    if (fireTouch.phase == TouchPhase.Began || fireTouch.phase == TouchPhase.Stationary)
                    {
                        ChangeTrailState(false, 0f);
                        cursor.SetActive(true);
                        cursor.GetComponent<Transform>().position = currentPosition; //Vector3.Lerp(cursorPosition, touchPos, speed);
                        cursor.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                    }


                    if (fireTouch.phase == TouchPhase.Ended || fireTouch.phase == TouchPhase.Canceled)
                    {
                        StartCoroutine(DelayDisapearing(delay));

                    }

                    break;
                case "drawing":
                    if (fireTouch.phase == TouchPhase.Stationary || fireTouch.phase == TouchPhase.Moved)
                    {
                        cursor.SetActive(true);
                        ChangeTrailState(true, time);

                        cursor.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                        cursor.GetComponent<Transform>().position = currentPosition; //Vector3.Lerp(cursorPosition, touchPos, speed);
                    }

                    if (fireTouch.phase == TouchPhase.Ended || fireTouch.phase == TouchPhase.Canceled)
                    {
                        ChangeTrailState(false, 0f);
                        cursor.SetActive(false);


                    }
                    break;
                default:
                    return;

            }
        }
        else
        {
            switch (sceneType)
            {

                case "puzzle":

                    if (Input.GetMouseButtonUp(0))
                    {
                        StartCoroutine(DelayDisapearing(delay));
                    }
                    else
                    {
                        ChangeTrailState(false, 0f);
                        cursor.SetActive(true);
                        cursor.GetComponent<Transform>().position = currentPosition; //Vector3.Lerp(cursorPosition, touchPos, speed);
                        cursor.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                    }

                    break;

                case "drawing":

                    if (Input.GetMouseButtonUp(0))
                    {
                        ChangeTrailState(false, 0f);
                        cursor.SetActive(false);

                    }
                    else
                    {
                        cursor.SetActive(true);
                        ChangeTrailState(true, time);

                        cursor.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                        cursor.GetComponent<Transform>().position = currentPosition; //Vector3.Lerp(cursorPosition, touchPos, speed);
                    }
                    break;
                default:
                    return;

            }
        
        }


    }

    #region

    /*
    void SpawnLightUsingMouse(Vector3 mouseMovement)
    {
        mousePosition = mouseMovement;
        mousePosition = Camera.main.ScreenToWorldPoint(mousePosition);
        Vector3 cursorPosition = cursor.GetComponent<Transform>().position;
        mousePosition.z = 1;// Puts the z coordinates at 1 so it is visible to the camera.

        switch (sceneType)
        {
            case "puzzle":
                Debug.Log(mousePosition);

               
                if (Input.GetMouseButtonUp(0))
                {
                    StartCoroutine(DelayDisapearing(delay));

                }
                else
                {
                    ChangeTrailState(false, 0f);
                    cursor.SetActive(true);
                    cursor.GetComponent<Transform>().position = mousePosition; //Vector3.Lerp(cursorPosition, touchPos, speed);
                    cursor.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);

                }

                break;

            case "drawing":
              
                if (Input.GetMouseButtonUp(0))
                {
                    ChangeTrailState(false, 0f);
                    cursor.SetActive(false);

                }
                else
                {
                    cursor.SetActive(true);
                    ChangeTrailState(true, time);

                    cursor.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                    cursor.GetComponent<Transform>().position = mousePosition; //Vector3.Lerp(cursorPosition, touchPos, speed);
                }
                break;
            default:
                return;
        }



    }

     


    void SpawnLightPointer(Touch fireTouch)
    {
        //Camera.main.ScreenToWorldPoint(fireTouch.position);
        Vector3 touchPos = Camera.main.ScreenToWorldPoint(fireTouch.position);
        touchPos.z = 1; // Puts the z coordinates at 1 so it is visible to the camera.
        Vector3 cursorPosition = cursor.GetComponent<Transform>().position;

        switch (sceneType)
        {
            case "puzzle":
                if (fireTouch.phase == TouchPhase.Began || fireTouch.phase == TouchPhase.Stationary)
                {
                    ChangeTrailState(false, 0f);
                    cursor.SetActive(true);
                    cursor.GetComponent<Transform>().position = touchPos; //Vector3.Lerp(cursorPosition, touchPos, speed);
                    cursor.transform.localScale = new Vector3(0.05f,0.05f,0.05f);

                }
              

                if (fireTouch.phase == TouchPhase.Ended || fireTouch.phase == TouchPhase.Canceled)
                {
                    StartCoroutine(DelayDisapearing(delay));

                }

                break;
            case "drawing":
                if (fireTouch.phase == TouchPhase.Stationary || fireTouch.phase == TouchPhase.Moved)
                {
                    cursor.SetActive(true);
                    ChangeTrailState(true, time);

                    cursor.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);

                    cursor.GetComponent<Transform>().position = touchPos; //Vector3.Lerp(cursorPosition, touchPos, speed);
                }

                if (fireTouch.phase == TouchPhase.Ended || fireTouch.phase == TouchPhase.Canceled)
                {
                    ChangeTrailState(false, 0f);
                    cursor.SetActive(false);


                }
                break;
            default:
                return;

        }



    }
      */
    #endregion
    void ChangeTrailState(bool emmiting, float t)
    {
        trail.emitting = emmiting;
        trail.time = t; 
    }
  
    IEnumerator DelayDisapearing(float delay)
    {
        Debug.Log("Corutine start");
        yield return new WaitForSecondsRealtime(delay);
        cursor.SetActive(false);
        ChangeTrailState(false, 0f);
        Debug.Log("Corutine ends");



    }


}