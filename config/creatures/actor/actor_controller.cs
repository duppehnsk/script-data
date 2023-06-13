    using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityStandardAssets.CrossPlatformInput;
//using UnityStandardAssets.Utility;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class actor_controller : MonoBehaviour
{

    public GameObject Actor;
    //public Animator _animHand;
    public Joystick MovementJoystick;
    public FixedTouchField CameraManager;
    public GameObject SpotLightActor;
    [HideInInspector]public bool lightenable = false;
    public GameObject textcrosshair;

    public CharacterController _CharacterController;


    [HideInInspector]public bool m_IsWalking;
    [SerializeField] private float m_WalkSpeed;
    [SerializeField] private float m_RunSpeed;
    [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
    [SerializeField] private float m_JumpSpeed;
    [SerializeField] private float m_StickToGroundForce;
    [SerializeField] private float m_GravityMultiplier;
    [SerializeField] private float m_StepInterval;   // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
    [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

    public Camera m_Camera;
    private bool m_Jump;
    private float m_YRotation;
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;
    private bool m_PreviouslyGrounded;
    private Vector3 m_OriginalCameraPosition;
    private float m_StepCycle;
    private float m_NextStep;
    public bool m_Jumping;
    private AudioSource m_AudioSource;
    [HideInInspector]public float horizontal;
    [HideInInspector]public float vertical;
    [HideInInspector]public bool Staying;
    [HideInInspector]public bool CanWalk;

    private string landtag;
    private float UPvertical;
    private float uphorizontal;




    //MOUSELOOK
    //[HideInInspector]
    public float XSensitivity = 2f;
    //[HideInInspector]
    public float YSensitivity = 2f;
    [HideInInspector]public bool clampVerticalRotation = true;
    [HideInInspector]public float MinimumX = -90F;
    [HideInInspector]public float MaximumX = 90F;
    [HideInInspector]public bool smooth;
    [HideInInspector]public float smoothTime = 5f;
    [HideInInspector]public bool lockCursor = true;
    [HideInInspector]public Vector2 MouseAxis;
    private Vector2 StopAnim;
    public bool crouch;
    public bool devdistancedraw;
    public bool TypeControlMobile;


    private Quaternion m_CharacterTargetRot;
    private Quaternion m_CameraTargetRot;
    public bool m_cursorIsLocked = true;

    [Header("Effects Sounds")]
    [SerializeField] public AudioClip[] Effects;
    
    [Header("FootStep Sounds")]
    [SerializeField] public AudioClip[] asphalt;
    [SerializeField] public AudioClip[] bush;
    [SerializeField] public AudioClip[] default1;
    [SerializeField] public AudioClip[] earth;
    
    [SerializeField] public AudioClip[] grass;
    [SerializeField] public AudioClip[] gravel;
    [SerializeField] public AudioClip[] metal;
    [SerializeField] public AudioClip[] wood;
    [SerializeField] public AudioClip[] water;
    [SerializeField] public AudioClip[] tin;
    // Use this for initialization


    public void DevDistance()
    {
        if(devdistancedraw == true)
        {
            Vector3 vectorCH = new Vector3 (Screen.width/2, Screen.height/2, 0);
            RaycastHit hit;
            Ray ray = m_Camera.ScreenPointToRay(vectorCH);
            if (Physics.Raycast(ray, out hit))
            {
                textcrosshair.SetActive(true);
                    float res = ((float)(int)(hit.distance * 100)) / 100;
                    textcrosshair.GetComponent<Text>().text = res.ToString();
            }
        } 
    }

    private void Start()
    {
        SpawnActor();
        CheckPermission();
        SetControlType();
        XSensitivity = PlayerPrefs.GetFloat("Sensivity");
        YSensitivity = PlayerPrefs.GetFloat("Sensivity");
        m_IsWalking = true;
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = Camera.main;
        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle / 2f;
        m_Jumping = false;
        m_AudioSource = GetComponent<AudioSource>();
        Init(transform, m_Camera.transform);
    }

    void SpawnActor()
    {
        if(PlayerPrefs.HasKey("name_location_next"))
        {
            if(PlayerPrefs.GetString("name_location_next") == SceneManager.GetActiveScene().name)
            {
                if(PlayerPrefs.HasKey("num_perexod"))
                    {  
                        var pere = GameObject.Find("spawnpoint_"+PlayerPrefs.GetInt("num_perexod"));
                        Actor.transform.position = pere.transform.position;
                        PlayerPrefs.DeleteKey("name_location_next");
                        PlayerPrefs.DeleteKey("num_perexod");
                    }

            }
        }
    }
    public void SetControlType()
    {
        if(PlayerPrefs.GetInt("TControl") == 0)
        {
            TypeControlMobile = true;
        }
        if(PlayerPrefs.GetInt("TControl") == 1)
        {
            TypeControlMobile = false;
            m_cursorIsLocked = true;
        }
    }


    public void ToggleLight()
    {
        if (lightenable == false)
        {
            lightenable = true;
        }
        else lightenable = false;
    }

        // Update is called once per frame
        private void Update()
        {
            DevDistance();
            if (horizontal == 0 && vertical == 0)
            {
                m_IsWalking = true;
            }
            RotateView();
            // the jump state needs to read here to make sure it is not missed
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }

            if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
            {
                m_MoveDir.y = 0f;
                m_Jumping = false;
            }
            if (!m_CharacterController.isGrounded && !m_Jumping && m_PreviouslyGrounded)
            {
                m_MoveDir.y = 0f;
            }

            m_PreviouslyGrounded = m_CharacterController.isGrounded;
        }


        public void JumpButton()
        {
            m_Jump = true;
        }

        void CheckCrosshair()
        {
            var uihud = FindObjectOfType<ui_hud_actor>();
            Vector3 vectorCH = new Vector3 (Screen.width/2, Screen.height/2, 0);
            RaycastHit hit;
            Ray ray = m_Camera.ScreenPointToRay(vectorCH);
            if (Physics.Raycast(ray, out hit))
            {
                if(hit.transform.gameObject.tag=="stalker")
                {
                    textcrosshair.SetActive(true);
                    //Debug.Log("YEP");
                    //textcrosshair.GetComponent<Text>().text = hit.transform.gameObject.GetComponent<stalker_script>()._name + "\n" +  hit.transform.gameObject.GetComponent<stalker_script>().group;
                    textcrosshair.GetComponent<Text>().color = Color.green;
                    
                }

                else textcrosshair.SetActive(false);
            }
            if (Physics.Raycast(ray, out hit,2f))
            {
                
                if(hit.transform.gameObject.tag=="stalker")
                {
 
                
                }
                
                
                
            }
        }


        public void Crouch()
        {
            if(crouch == true)
            {
                crouch = false;
                
            }
            
            else if(crouch == false)
            {
                crouch = true;
                
            } 

        }

        private void FixedUpdate()
        {
            if(crouch == true)
            {
                _CharacterController.height = 0;
            }else _CharacterController.height = 1.3f;
            
            AutoRun();
        if (lightenable == true)
        {
            SpotLightActor.SetActive(true);
        }
        else SpotLightActor.SetActive(false);

        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit))
        {
           landtag = hit.transform.gameObject.tag;
        }
        if(PlayerPrefs.GetInt("IDStalkers") == 1)
       {
        CheckCrosshair();
        }


        if (m_IsWalking == false)
            {
            //_animHand.SetBool("stay", false);
            //_animHand.SetBool("run", true);
            //_animHand.SetBool("walk", false);
            }
            //else hud.SetBool("run", false);
        if(CanWalk == false || horizontal + vertical == 0)
        {
            Staying = true;
            //_animHand.SetBool("stay", true);
            //_animHand.SetBool("run", false);
            //_animHand.SetBool("walk", false);
        }else Staying = false;

            float speed;
            GetInput(out speed);
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward*m_Input.y + transform.right*m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height/2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            if (vertical < 0 )
            {
                UPvertical = vertical*(-1);
            }
            else UPvertical = vertical;

            
            if(CanWalk == true)
            {
                if(UPvertical < 0.3)
            {
                m_MoveDir.x = desiredMove.x*speed*0.3f;
                m_MoveDir.z = desiredMove.z*speed*0.3f;
            }
            else
            {
            m_MoveDir.x = desiredMove.x*speed*UPvertical;
            m_MoveDir.z = desiredMove.z*speed*UPvertical;
            }
            if(vertical < -0.3f)
                {
                m_MoveDir.x = desiredMove.x*speed*0.3f;
                m_MoveDir.z = desiredMove.z*speed*0.3f;
                //Debug.Log(UPvertical);      
                }
            
            } else{
                m_MoveDir.x = 0;
                m_MoveDir.z = 0;
                speed = 0;
            }

            if (m_CharacterController.isGrounded)
            {
                m_MoveDir.y = -m_StickToGroundForce;

                if (m_Jump)
                {
                    m_MoveDir.y = m_JumpSpeed;
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            }
            else
            {
                m_MoveDir += Physics.gravity*m_GravityMultiplier*Time.fixedDeltaTime;
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir*Time.fixedDeltaTime);
            ProgressStepCycle(speed);
            UpdateCursorLock();
        }


        private void PlayJumpSound()
        {
            m_AudioSource.clip = m_JumpSound;
            m_AudioSource.Play();
        }


        private void ProgressStepCycle(float speed)
        {
            if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0))
            {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed*(m_IsWalking ? 1f : m_RunstepLenghten)))*
                             Time.fixedDeltaTime;
            }

            if (!(m_StepCycle > m_NextStep))
            {
                return;
            }

            m_NextStep = m_StepCycle + m_StepInterval;

            PlayFootStepAudio();
        }


        private void PlayFootStepAudio()
        {
            if (!m_CharacterController.isGrounded)
            {
                return;
            }

        CheckFootStep();
        }

        void CheckFootStep()
        {
            
        if (landtag == "Wood")
        {
            int n = Random.Range(1, wood.Length);
            m_AudioSource.clip = wood[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            wood[n] = wood[0];
            wood[0] = m_AudioSource.clip;
        }
        if (landtag == "asphalt")
        {
            int n = Random.Range(1, asphalt.Length);
            m_AudioSource.clip = asphalt[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            asphalt[n] = asphalt[0];
            asphalt[0] = m_AudioSource.clip;
        }
        if (landtag == "bush")
        {
            int n = Random.Range(1, bush.Length);
            m_AudioSource.clip = bush[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            bush[n] = bush[0];
            bush[0] = m_AudioSource.clip;
        }
        if (landtag == "default")
        {
            int n = Random.Range(1, default1.Length);
            m_AudioSource.clip = default1[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            default1[n] = default1[0];
            default1[0] = m_AudioSource.clip;
        }
        if (landtag == "earth")
        {
            int n = Random.Range(1, earth.Length);
            m_AudioSource.clip = earth[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            earth[n] = earth[0];
            earth[0] = m_AudioSource.clip;
        }
        if (landtag == "grass")
        {
            int n = Random.Range(1, grass.Length);
            m_AudioSource.clip = grass[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            grass[n] = grass[0];
            grass[0] = m_AudioSource.clip;
        }
        if (landtag == "gravel")
        {
            int n = Random.Range(1, gravel.Length);
            m_AudioSource.clip = gravel[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            gravel[n] = gravel[0];
            gravel[0] = m_AudioSource.clip;
        }
        if (landtag == "metal")
        {
            int n = Random.Range(1, metal.Length);
            m_AudioSource.clip = metal[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            metal[n] = metal[0];
            metal[0] = m_AudioSource.clip;
        }
        if (landtag == "water")
        {
            int n = Random.Range(1, water.Length);
            m_AudioSource.clip = water[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            water[n] = water[0];
            water[0] = m_AudioSource.clip;
        }
        if (landtag == "tin")
        {
            int n = Random.Range(1, tin.Length);
            m_AudioSource.clip = tin[n];
            m_AudioSource.PlayOneShot(m_AudioSource.clip);
            // move picked sound to index 0 so it's not picked next time
            tin[n] = tin[0];
            tin[0] = m_AudioSource.clip;
        }
        }

        void CheckPermission()
        {
            //Camera.main.useOcclusionCulling
            Camera.main.farClipPlane = PlayerPrefs.GetFloat("DistanceView");
            m_Camera.fieldOfView = PlayerPrefs.GetFloat("FOV");
            if(PlayerPrefs.GetFloat("FOV") < 67f)
            {
                m_Camera.fieldOfView = 68f;
            }
            if(PlayerPrefs.GetInt("DistanceMeter") == 1)
            {
                devdistancedraw = true;
            }else devdistancedraw = false;
        }


        void AutoRun()
        {
            if(vertical > 0.8f)
            {
                m_IsWalking = false;
            }
            else m_IsWalking = true;
            if(vertical <0.8f)
            {
           // _animHand.SetBool("stay", false);
           // _animHand.SetBool("run", false);
           // _animHand.SetBool("walk", true);
            }
        }

        private void GetInput(out float speed)
        {
            // Read input
            if(TypeControlMobile)
            {
            horizontal = MovementJoystick.Horizontal;
            vertical = MovementJoystick.Vertical;
            }
            if(!TypeControlMobile)
            {
            horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
            vertical = CrossPlatformInputManager.GetAxis("Vertical");
            }
            //

            bool waswalking = m_IsWalking;

#if !MOBILE_INPUT
            // On standalone builds, walk/run speed is modified by a key press.
            // keep track of whether or not the character is walking or running
            //m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif
            // set the desired speed to be walking or running
            speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
            m_Input = new Vector2(horizontal, vertical);

            // normalize input if it exceeds 1 in combined length:
            if (m_Input.sqrMagnitude > 1)
            {
                m_Input.Normalize();
            }
        }


        private void RotateView()
        {   

            LookRotation (transform, m_Camera.transform);
        }


        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            Rigidbody body = hit.collider.attachedRigidbody;
            //dont move the rigidbody if the character is on top of it
            if (m_CollisionFlags == CollisionFlags.Below)
            {
                return;
            }

            if (body == null || body.isKinematic)
            {
                return;
            }
            body.AddForceAtPosition(m_CharacterController.velocity*0.1f, hit.point, ForceMode.Impulse);
        }
    
    public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;
        }


        public void LookRotation(Transform character, Transform camera)
        {
            MouseAxis = CameraManager.TouchDist;
            float yRot = 0;
            float xRot = 0;

           

            if(TypeControlMobile)
            {
                yRot = MouseAxis.x * XSensitivity;
                xRot = MouseAxis.y * YSensitivity;
                m_cursorIsLocked = false;
            }
            if(!TypeControlMobile)
            {
                yRot = CrossPlatformInputManager.GetAxis("Mouse X") * 2;
                xRot = CrossPlatformInputManager.GetAxis("Mouse Y") * 2;
                
                
            }
            m_CharacterTargetRot *= Quaternion.Euler (0f, yRot, 0f);
            m_CameraTargetRot *= Quaternion.Euler (-xRot, 0f, 0f);
            if(clampVerticalRotation)
                m_CameraTargetRot = ClampRotationAroundXAxis (m_CameraTargetRot);

            if(smooth)
            {
                character.localRotation = Quaternion.Slerp (character.localRotation, m_CharacterTargetRot,
                    smoothTime * Time.deltaTime);
                camera.localRotation = Quaternion.Slerp (camera.localRotation, m_CameraTargetRot,
                    smoothTime * Time.deltaTime);
            }
            else
            {
                character.localRotation = m_CharacterTargetRot;
                camera.localRotation = m_CameraTargetRot;
            }

            UpdateCursorLock();
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if(!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
            {};
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            if(Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }


            if (m_cursorIsLocked)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.Locked;
                UnityEngine.Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                UnityEngine.Cursor.lockState = CursorLockMode.None;
                UnityEngine.Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

        public void Run()
        {
            if (m_IsWalking == false)
            {
                m_IsWalking = true;
            }
            else m_IsWalking = false;
            
        }
    
    }
