using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Cinemachine;

public class Player : MonoBehaviour
{
    //Speed Fields
    [SerializeField] float InitialShipSpeed = 1f;
    [SerializeField] float ShipSpeedIncrementer = 0.25f;
    [SerializeField] float EndingShipSpeed = 7f;
    [SerializeField] float ShipSpeed = 1f;

    //Rotation Fields
    [SerializeField] float secondsToRotate = 2f;
    [SerializeField] float zoomOutScale = 5f;
    [SerializeField] float percentSpentZoomOut = .3f;
    [SerializeField] float percentSpentZoomIn = .3f;
    [SerializeField] float percentSpentRotatingShip = .2f;
    [SerializeField] float seconds_to_die = 3f;

    [SerializeField] CinemachineVirtualCamera vcam;

    Rigidbody2D myRigidBody;
    PolygonCollider2D myBodyCollider;
    Animator myAnimator;

    public bool isAlive;
    public Quaternion fromAngle;
    public Quaternion toAngle;

    private Quaternion startingRotation;
    private int times_rotated;
    private bool rotating;
    private bool cameraRotating;
    private LensSettings m_Lens;
    private float startingZoom;

    // Start is called before the first frame update
    void Start()
    {
        vcam = FindObjectOfType<CinemachineVirtualCamera>();

        myRigidBody = GetComponent<Rigidbody2D>();
        myBodyCollider = GetComponent<PolygonCollider2D>();
        myAnimator = GetComponent<Animator>();
        isAlive = true;
        startingRotation = this.transform.rotation;
        times_rotated = 0;
        m_Lens = vcam.m_Lens;
        startingZoom = m_Lens.OrthographicSize;

        //speed
        ShipSpeed = InitialShipSpeed;

    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) {
            myRigidBody.velocity = new Vector2(0f, 0f);
            return;
        }
        move();
        rotateMain();
        die();

    }

    private void move()
    {
        Vector2 playerMovement = getMovement();
        myRigidBody.velocity = playerMovement;


    }

    public Vector2 getMovement()
    {
        if (times_rotated == 0)
        {
            return new Vector2(0f, -ShipSpeed);
        } else if (times_rotated == 1)
        {
            return new Vector2(ShipSpeed, 0f);
        } else if (times_rotated == 2)
        {
            return new Vector2(0f, ShipSpeed);
        }
        else
        {
            return new Vector2(-ShipSpeed, 0f);
        }
    }

    private Vector3 set_up_camera()
    {


        if (times_rotated == 0)
        {
            //m_Lens.Dutch = 90f;
            return new Vector3(0f, 3f, 0f);
        } else if (times_rotated == 1)
        {
            //m_Lens.Dutch = 0f;
            return new Vector3(-3f, 0f, 0f);
        }
        else if (times_rotated == 2)
        {
            //m_Lens.Dutch = 90f;
            return new Vector3(0f, -3f, 0f);
        }
        else
        {
            //m_Lens.Dutch = 0f;
            return new Vector3(-3f, 0f, 0f);
        }
    }


    public void triggerNewRotation()
    {
        if (times_rotated == 0)
        {
            myAnimator.SetBool("0", true);
            myAnimator.SetBool("90", false);
            myAnimator.SetBool("180", false);
            myAnimator.SetBool("270", false);
        }
        else if (times_rotated == 1)
        {
            myAnimator.SetBool("0", false);
            myAnimator.SetBool("90", true);
            myAnimator.SetBool("180", false);
            myAnimator.SetBool("270", false);
        }
        else if (times_rotated == 2)
        {
            myAnimator.SetBool("0", false);
            myAnimator.SetBool("90", false);
            myAnimator.SetBool("180", true);
            myAnimator.SetBool("270", false);
        }
        else
        {
            myAnimator.SetBool("0", false);
            myAnimator.SetBool("90", false);
            myAnimator.SetBool("180", false);
            myAnimator.SetBool("270", true);
        }
    }

    // ROTATE

    private void rotateMain()
    {   
        if (cameraRotating) { return; }

        bool mouseClick = CrossPlatformInputManager.GetButtonDown("Fire1");
        bool touchEvent = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;

        if ( (mouseClick || touchEvent) && !rotating)
        {
            times_rotated = (times_rotated + 1) % 4;

           
            // zoom out
            StopAllCoroutines();

            StartCoroutine(rotatePlayerController());

            StartCoroutine(rotateZoom());
            StartCoroutine(rotateCamera());


        }
    }

    private IEnumerator rotatePlayerController()
    {
        cameraRotating = true;

        float time_rotating_player = percentSpentRotatingShip * secondsToRotate;
        float seconds = 0f;

        float new_angle = 90 * (times_rotated);

        while (seconds <= time_rotating_player)
        {

            seconds += Time.deltaTime; 
            float dist = (Time.deltaTime / time_rotating_player) * 90f;
            this.transform.rotation = Quaternion.Euler(0, 0, dist + transform.localEulerAngles.z);

            yield return 0;
        }

        this.transform.rotation = Quaternion.Euler(0, 0, new_angle);


        if (percentSpentRotatingShip > percentSpentZoomOut)
        {
            cameraRotating = false;
        }
    }

    private IEnumerator rotateCamera()
    {
   
        float time_rotating_camera = percentSpentZoomOut * secondsToRotate;
        float seconds = 0f;

        Vector3 current_position = vcam.transform.position;
        Vector3 new_position = set_up_camera();

        float turn = times_rotated % 2 == 0 ? 90f - m_Lens.Dutch : m_Lens.Dutch;

        while (seconds <= time_rotating_camera)
        {
            float dutch_changing = (Time.deltaTime / time_rotating_camera) * turn;
            m_Lens.Dutch = times_rotated % 2 == 0 ? (m_Lens.Dutch + dutch_changing) : (m_Lens.Dutch - dutch_changing);

            seconds += Time.deltaTime;

            yield return 0;

        }

        m_Lens.Dutch = times_rotated % 2 == 0 ? 90f : 0f;

        if (percentSpentRotatingShip <= percentSpentZoomOut)
        {
            cameraRotating = false;
        }
    }

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

    }


    // HANDLERS //
    public void playerCrossingRoom()
    {

        // SPEED CHANGE
        if (ShipSpeed + ShipSpeedIncrementer <= EndingShipSpeed)
        {
            ShipSpeed += ShipSpeedIncrementer;
        }
        else
        {
            ShipSpeed = EndingShipSpeed;
        }
        // -------------
    }
    // -------- //


    private void die()
    {
        if (myBodyCollider.IsTouchingLayers(LayerMask.GetMask("Astroids"))){
            myAnimator.SetBool("Dead", true);
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

        FindObjectOfType<GameSession>().ProcessPlayerDeath();

    }
}
