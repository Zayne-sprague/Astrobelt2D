using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using Cinemachine;

public class PlayerController : MonoBehaviour
{

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

    public Quaternion fromAngle;
    public Quaternion toAngle;

    private int times_rotated;
    private bool rotating;
    private bool cameraRotating;
    private LensSettings m_Lens;
    private float startingZoom;
 
    // presets

    Rigidbody2D myRigidBody;
    PolygonCollider2D myBodyCollider;
    Animator myAnimator;

    public bool isAlive;


    // Start is called before the first frame update
    void Start()
    {
        vcam = FindObjectOfType<CinemachineVirtualCamera>();

        //SETTERS
        myRigidBody = GetComponent<Rigidbody2D>();
        myBodyCollider = GetComponent<PolygonCollider2D>();
        myAnimator = GetComponent<Animator>();
        isAlive = true;
        ShipSpeed = InitialShipSpeed;

        int playerID = PlayerPrefs.GetInt("PlayerID", 1);
        myAnimator.SetInteger("PlayerID", playerID);


        //ROTATION SETTERS
        times_rotated = 0;
        m_Lens = vcam.m_Lens;
        startingZoom = m_Lens.OrthographicSize;
        myRigidBody.velocity = getMovement();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isAlive) {
            myRigidBody.velocity = new Vector2(0f,0f);
            return;
        }
        rotateMain();
        die();
    }

    public Vector2 getMovement()
    {
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
        if (cameraRotating || !(ShipSpeed - DecreasePerTurn > MinimumSpeed)) { return; }

        bool mouseClick = CrossPlatformInputManager.GetButtonDown("Fire1");
        bool touchEvent = Input.touchCount == 1 && Input.GetTouch(0).phase == TouchPhase.Began;

        if ( (mouseClick || touchEvent) && !rotating)
        {
            times_rotated = (times_rotated + 1) % 4;


            ShipSpeed = ShipSpeed - DecreasePerTurn;
            myRigidBody.velocity = getMovement();

           


           
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

    //Updates the Dutch of the camera - rotating it
    private IEnumerator rotateCamera()
    {
   
        float time_rotating_camera = percentSpentZoomOut * secondsToRotate;
        float seconds = 0f;

        Vector3 current_position = vcam.transform.position;
        Vector3 new_position = set_up_camera();

        float turn = 90f; //times_rotated % 2 == 0 ? 90f - m_Lens.Dutch : m_Lens.Dutch;
        float previous_dutch = m_Lens.Dutch;
        do
        {
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
