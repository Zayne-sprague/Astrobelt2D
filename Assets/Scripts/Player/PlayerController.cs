﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Cinemachine;
using DarkTonic.MasterAudio;

using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Text tooltip;

    //Speed Fields
    [SerializeField] float InitialShipSpeed = 1f;
    [SerializeField] float MinimumSpeed = .75f;
    [SerializeField] float ShipSpeedIncrementer = 0.25f;
    [SerializeField] float EndingShipSpeed = 3.5f;
    [SerializeField] float DecreasePerTurn = 0.3f;
    [SerializeField] float ShipSpeed = 1f;

    [SerializeField] float seconds_to_die = 3f;

    //ROTATION VARS
    [SerializeField] CinemachineVirtualCamera vcam;

    [SerializeField] float secondsToRotate = 2f;
    [SerializeField] float zoomOutScale = 5f;
    [SerializeField] float percentSpentZoomOut = .3f;
    [SerializeField] float percentSpentZoomIn = .3f;
    [SerializeField] float percentSpentRotatingShip = .2f;
    [SerializeField] float percentForDoubleTapHotSpot = .2f;

    private float time_of_rotation_begin;

    [SerializeField] GameObject turning_gas;
    private Animator gasAnimator;

    [SerializeField] Sprite[] sprites;

    public Quaternion fromAngle;
    public Quaternion toAngle;

    private int times_rotated;
    private bool rotating;
    private bool double_turn = false;
    private bool cameraRotating;
    private LensSettings m_Lens;
    private float startingZoom;
    private bool just_started = true;


    // presets

    Rigidbody2D myRigidBody;
    PolygonCollider2D myBodyCollider;
    public Animator myAnimator;
    public SpriteRenderer myRender;

    public bool isAlive;


    // Start is called before the first frame update
    void Start()
    {
        vcam = FindObjectOfType<CinemachineVirtualCamera>();

        //SETTERS
        myRigidBody = GetComponent<Rigidbody2D>();
        myBodyCollider = GetComponent<PolygonCollider2D>();
        myAnimator = GetComponent<Animator>();
        gasAnimator = turning_gas.GetComponent<Animator>();

        gasAnimator.SetBool("turning", false);

        myRender = GetComponent<SpriteRenderer>();
        isAlive = true;
        ShipSpeed = InitialShipSpeed;

        int playerID = PlayerPrefs.GetInt("PlayerID", 1);
        myAnimator.SetInteger("PlayerID", playerID);
        myRender.sprite = sprites[playerID - 1];

        //ROTATION SETTERS
        times_rotated = 0;
        m_Lens = vcam.m_Lens;
        startingZoom = m_Lens.OrthographicSize;
        myRigidBody.velocity = new Vector3(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) {
            myRigidBody.velocity = new Vector2(0f,0f);
            return;
        }

        if (isAlive && !myAnimator.enabled && myAnimator.GetInteger("PlayerID") == PlayerPrefs.GetInt("PlayerID", 1) && myRender.sprite == sprites[PlayerPrefs.GetInt("PlayerID", 1) - 1])
        {
            myAnimator.enabled = true;
            myRender.enabled = true;
        }

        rotateMain();
        die();
    }

    public Vector2 getMovement()
    {

        int playerspeed = 0;

        if (ShipSpeed < InitialShipSpeed + DecreasePerTurn)
        {
            playerspeed = 1;
        }else if (ShipSpeed < InitialShipSpeed + DecreasePerTurn * 4)
        {
            playerspeed = 2;
        }
        else if (ShipSpeed < InitialShipSpeed + DecreasePerTurn * 8)
        {
            playerspeed = 3;
        }
        else if (ShipSpeed < InitialShipSpeed + DecreasePerTurn * 12)
        {
            playerspeed = 4;
        }
        else if (ShipSpeed < EndingShipSpeed - DecreasePerTurn * 8)
        {
            playerspeed = 5;
        }
        else if (ShipSpeed > EndingShipSpeed - DecreasePerTurn * 4)
        {
            playerspeed = 6;
        }
        print(playerspeed);
        myAnimator.SetInteger("PlayerSpeedState", playerspeed);

        //return new Vector2(0f, 0f);
        if (times_rotated == 0)
        {
            return new Vector2(0f, ShipSpeed);
        } else if (times_rotated == 1)
        {
            return new Vector2(-ShipSpeed, 0f);
        } else if (times_rotated == 2)
        {
            return new Vector2(0f, -ShipSpeed);
        }
        else
        {
            return new Vector2(ShipSpeed, 0f);
        }
    }

    private void die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Astroids")) && isAlive){
            myAnimator.SetBool("Dead", true);
            MasterAudio.FireCustomEvent("Explosion", transform);
            myAnimator.SetInteger("PlayerID", 0);
            isAlive = false;

            StartCoroutine(deathPause());
        }
    }

    private IEnumerator deathPause()
    {
        float seconds = 0f;
        while ( seconds < seconds_to_die)
        {
            seconds += Time.deltaTime;
            yield return 0;
        }

        FindObjectOfType<ProceduralGameManager>().ProcessPlayerDeath();

    }





    /* ROTATION FUNCS */

    // OFFSET CAMERA TO PUT SHIP AT BOTTOM OF SCREEN.
    private Vector3 set_up_camera()
    {


        if (times_rotated == 0)
        {
            return new Vector3(0f, 3f, -10f);
        } else if (times_rotated == 1)
        {
            return new Vector3(-3f, 0f, -10f);
        }
        else if (times_rotated == 2)
        {
            return new Vector3(0f, -3f, -10f);
        }
        else
        {
            return new Vector3(-3f, 0f, -10f);
        }
    }

    // MAIN FUNCTION USED TO CONTROL ROTATION  - the camera is the last to finish rotating, wait on it.
    private void rotateMain()
    {   
        //early escape
        // either no gas or have rotated two times 
        if ((cameraRotating && double_turn) || !(ShipSpeed - DecreasePerTurn > MinimumSpeed)) { return; }

        // Save the time for the initial rotation
        if (!cameraRotating) { time_of_rotation_begin = Time.fixedTime; }

        // Gotta hit the hot spot
        if (cameraRotating && !double_turn && ((Time.fixedTime - time_of_rotation_begin) > (secondsToRotate * percentForDoubleTapHotSpot))){ return;  }

        bool mouseClick = CrossPlatformInputManager.GetButtonDown("Fire1");
        bool touchEvent = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;

        if (mouseClick || touchEvent)
        {
            if (just_started)
            {
                just_started = false;
                myRigidBody.velocity = getMovement();

                if (tooltip)
                {
                    tooltip.gameObject.SetActive(false);
                }
                return;
            }

            // This sound effect sucks
            //MasterAudio.FireCustomEvent("LeftTurn", transform);

            times_rotated = (times_rotated + 1) % 4;

            gasAnimator.SetBool("turning", true);

            if (!double_turn && cameraRotating)
            {
                double_turn = true;
                ShipSpeed = ShipSpeed - (DecreasePerTurn * 4);
                myRigidBody.velocity = getMovement();
                return;
            }
            else
            {
                ShipSpeed = ShipSpeed - DecreasePerTurn;
                myRigidBody.velocity = getMovement();
            }





            // zoom out
            StopAllCoroutines();

            StartCoroutine(rotatePlayerController());

            StartCoroutine(rotateZoom());
            StartCoroutine(rotateCamera());


        }
    }

    //Rotate the sprite 
    private IEnumerator rotatePlayerController()
    {
        cameraRotating = true;

        float time_rotating_player = percentSpentRotatingShip * secondsToRotate;
        float seconds = 0f;

        bool picked_up_double_turn = false;

        float new_angle = 90 * (times_rotated);
        float turn_angle = 90f;

        while (seconds <= time_rotating_player)
        {
            if (!picked_up_double_turn && double_turn)
            {
                picked_up_double_turn = true;

                time_rotating_player *= 2;
                turn_angle *= 2;
                new_angle = 90 * (times_rotated);

            }

            seconds += Time.deltaTime; 
            float dist = (Time.deltaTime / time_rotating_player) * turn_angle;
            this.transform.rotation = Quaternion.Euler(0, 0, dist + transform.localEulerAngles.z);

            yield return 0;
        }

        this.transform.rotation = Quaternion.Euler(0, 0, new_angle);


        if (percentSpentRotatingShip > percentSpentZoomOut)
        {
            cameraRotating = false;
            double_turn = false;

        }

        gasAnimator.SetBool("turning", false);

    }

    //Updates the Dutch of the camera - rotating it
    private IEnumerator rotateCamera()
    {

        float time_rotating_camera = percentSpentZoomOut * secondsToRotate;
        float seconds = 0f;

        Vector3 current_position = vcam.transform.position;
        Vector3 new_position = set_up_camera();

        float turn = 90f; //times_rotated % 2 == 0 ? 90f - m_Lens.Dutch : m_Lens.Dutch;
        float previous_dutch = m_Lens.Dutch;

        bool picked_up_double_turn = false;

        do
        {

            if (!picked_up_double_turn && double_turn)
            {
                picked_up_double_turn = true;

                turn += 90f;
                time_rotating_camera *= 2;

            }

            float dutch_changing = (Time.deltaTime / time_rotating_camera) * turn;
            m_Lens.Dutch = m_Lens.Dutch + dutch_changing; //times_rotated % 2 == 0 ? (m_Lens.Dutch + dutch_changing) : (m_Lens.Dutch - dutch_changing);
            vcam.m_Lens = m_Lens;
            
            seconds += Time.deltaTime;

            yield return 0;

        } while (seconds <= time_rotating_camera);

    
        m_Lens.Dutch = 90 + (times_rotated * 90); //times_rotated % 2 == 0 ? 90f : 0f;
        vcam.m_Lens = m_Lens;

        if (percentSpentRotatingShip <= percentSpentZoomOut)
        {
            cameraRotating = false;
            double_turn = false;

        }
    }

    // Zoom camera out - go idle - zoom in
    private IEnumerator rotateZoom()
    {
        // ZOOM OUT

        float time_zooming_out = percentSpentZoomOut * secondsToRotate;
        float seconds = 0f;

        float distance_to_zoom = zoomOutScale - m_Lens.OrthographicSize;
        


        while (seconds <= time_zooming_out)
        {
            float dist = (Time.deltaTime / time_zooming_out) * distance_to_zoom;

            seconds += Time.deltaTime;
            m_Lens.OrthographicSize += dist;
            vcam.m_Lens = m_Lens;

            yield return 0;

        }

        m_Lens.OrthographicSize = zoomOutScale;

        // IDLE THE CAMERA

        float time_idle = secondsToRotate * (1 - (percentSpentZoomIn + percentSpentZoomOut));
        float idle_seconds = 0f; 

        while (idle_seconds <= time_idle)
        {
            idle_seconds += Time.deltaTime;
            yield return 0;
        }

        // ZOOM IN 

        float time_zooming_in = percentSpentZoomIn * secondsToRotate;
        float zoom_out_seconds = 0f;

        distance_to_zoom = m_Lens.OrthographicSize - startingZoom;

        while (zoom_out_seconds <= time_zooming_in)
        {
            float dist = (Time.deltaTime / time_zooming_in) * distance_to_zoom;

            zoom_out_seconds += Time.deltaTime;
            m_Lens.OrthographicSize -= dist;
            vcam.m_Lens = m_Lens;

            yield return 0;

        }

        m_Lens.OrthographicSize = startingZoom;
    }

    public void increaseShipSpeed()
    {
        if(ShipSpeed >= EndingShipSpeed) { return; }

        ShipSpeed += ShipSpeedIncrementer;
        myRigidBody.velocity = getMovement();

    }


    /* -------------- */
}
