using System;
using UnityEngine;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class FirstPersonController : MonoBehaviour {

    [Space(5)]
    [SerializeField] private bool m_IsWalking;
    [SerializeField] private float m_WalkSpeed;
    [SerializeField] private float m_RunSpeed;
    [SerializeField] private bool m_PressToCrouch;
    [SerializeField] private float m_CrouchSpeedModifier;
    [SerializeField] private float climbSpeed = 3.0f;
    [SerializeField] private float climbRate = 0.5f;
    [SerializeField] [Range(0f, 1f)] private float m_RunstepLenghten;
    [SerializeField] private float m_JumpSpeed;
    [SerializeField] public float m_GravityMultiplier;

    [Space(5)]
    [SerializeField] public MouseLook m_MouseLook;
    [SerializeField] private bool m_UseFovKick;
    [SerializeField] private FOVKick m_FovKick = new FOVKick();

    [Space(5)]
    [SerializeField] private bool m_UseHeadBob;
    [SerializeField] private CurveControlledBob m_HeadBob = new CurveControlledBob();
    [SerializeField] private LerpControlledBob m_JumpBob = new LerpControlledBob();

    [Space(5)]
    [SerializeField] private float m_StepInterval;
    [SerializeField] private AudioClip[] m_FootstepSounds;    // an array of footstep sounds that will be randomly selected from.
    [SerializeField] private AudioClip m_JumpSound;           // the sound played when character leaves the ground.
    [SerializeField] private AudioClip[] m_LadderSounds;      // the sound played when climbing ladders
    [SerializeField] private AudioClip m_LandSound;           // the sound played when character touches back on ground.

    // Ladder
    private bool m_onLadder = false;
    private float playTime = 0.0f;
    private Vector3 climbDirection = Vector3.up;
    private Vector3 lateralMove = Vector3.zero;
    private Vector3 ladderMovement = Vector3.zero;
    private Rigidbody myRigid;
    private GameObject ladderObject;

    // Inverted Jump
    private bool isGrounded = false;

    // Crouch
    private bool m_Crouch;
    private bool m_Crouching;

    private float m_DefaultWalkSpeed;
    private float m_DefaultRunSpeed;
    private bool m_Jump;
    private float m_YRotation;
    private bool m_PreviouslyGrounded;
    private float m_StepCycle;
    private float m_NextStep;
    private bool m_Jumping;

    private Vector3 m_OriginalCameraPosition;
    private Camera m_Camera;
    private AudioSource m_AudioSource;
    private Vector2 m_Input;
    private Vector3 m_MoveDir = Vector3.zero;
    private CharacterController m_CharacterController;
    private CollisionFlags m_CollisionFlags;

    // Use this for initialization
    private void Start() {
        m_CharacterController = GetComponent<CharacterController>();
        m_Camera = Camera.main;
        m_OriginalCameraPosition = m_Camera.transform.localPosition;
        m_FovKick.Setup(m_Camera);
        m_HeadBob.Setup(m_Camera, m_StepInterval);
        m_StepCycle = 0f;
        m_NextStep = m_StepCycle / 2f;
        m_Jumping = false;
        m_Crouching = false;
        m_DefaultRunSpeed = m_RunSpeed;
        m_DefaultWalkSpeed = m_WalkSpeed;
        m_AudioSource = GetComponent<AudioSource>();
        m_MouseLook.Init(transform, m_Camera.transform);
        myRigid = GetComponent<Rigidbody>();
        m_onLadder = false;
    }


    // Update is called once per frame
    private void Update() {
        if (Physics.gravity.y > 0) {
            isGrounded = (m_CharacterController.collisionFlags & CollisionFlags.Above) != 0 ? true : false;
        } else {
            isGrounded = (m_CharacterController.collisionFlags & CollisionFlags.Below) != 0 ? true : false;
        }
        RotateView();
        // the jump state needs to read here to make sure it is not missed
        if (!m_Jump && !m_onLadder) {
            m_Jump = Input.GetButtonDown("Jump");

            // UnCrouch when jumping (Only works in press to crouch mode)
            if (m_Jump) {
                m_Crouch = false;
            }
        }
        if (m_IsWalking) {
            if (m_PressToCrouch) {
                if (Input.GetButtonDown("Crouch")) {
                    if (m_Crouch)
                        m_Crouch = false;
                    else
                        m_Crouch = true;
                }
            } else {
                m_Crouch = Input.GetButton("Crouch");
            }
        } else if (!m_IsWalking) {
            m_Crouch = false;
        }


        if (!m_PreviouslyGrounded && isGrounded) {
            StartCoroutine(m_JumpBob.DoBobCycle());
            PlayLandingSound();
            m_MoveDir.y = 0f;
            m_Jumping = false;
        }
        if (!isGrounded && !m_Jumping && m_PreviouslyGrounded) {
            m_MoveDir.y = 0f;
        }

        m_PreviouslyGrounded = isGrounded;

        if (m_onLadder) {
            StopAllCoroutines();
            StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
            myRigid.useGravity = false;
            myRigid.isKinematic = true;
            LadderUpdate();
        } else {
            myRigid.useGravity = true;
            myRigid.isKinematic = true;
        }
    }

    //When on Ladder Trigger
    private void OnTriggerEnter(Collider ladder) {
        if (ladder.tag == "Ladder") {
            m_onLadder = true;
        }
    }

    //Ladder Trigger Exit
    private void OnTriggerExit(Collider ladder) {
        if (ladder.tag == "Ladder") {
            m_onLadder = false;
        }
    }

    private void FixedUpdate() {
        float speed;
        GetInput(out speed);

        if (!m_onLadder) {
            // always move along the camera forward as it is the direction that it being aimed at
            Vector3 desiredMove = transform.forward * m_Input.y + transform.right * m_Input.x;

            // get a normal for the surface that is being touched to move along it
            RaycastHit hitInfo;
            Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
                               m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
            desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

            m_MoveDir.x = desiredMove.x * speed;
            m_MoveDir.z = desiredMove.z * speed;


            if (m_Crouch && !m_Crouching) {
                m_Crouching = true;
                m_CharacterController.height = m_CharacterController.height / 2;
                if (!m_Jumping) {
                    m_WalkSpeed *= m_CrouchSpeedModifier;
                    m_RunSpeed *= m_CrouchSpeedModifier;
                }
            } else if (!m_Crouch && m_Crouching) {
                m_Crouching = false;
                m_WalkSpeed = m_DefaultWalkSpeed;
                m_RunSpeed = m_DefaultRunSpeed;
                m_CharacterController.height = m_CharacterController.height * 2;
            }

            if (isGrounded) {
                if (m_Jump) {
                    if (Physics.gravity.y > 0) {
                        m_MoveDir.y = -m_JumpSpeed;
                    } else {
                        m_MoveDir.y = m_JumpSpeed;
                    }
                    PlayJumpSound();
                    m_Jump = false;
                    m_Jumping = true;
                }
            } else {
                m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;

                if (CheckHeadCollision()) {
                    m_MoveDir.y = 0;
                }
            }
            m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.deltaTime);
            ProgressStepCycle(speed);
        }
        UpdateCameraPosition(speed);

        m_MouseLook.UpdateCursorLock();
    }

    //Ladder Movement
    private void LadderUpdate() {
        if (m_onLadder) {
            Vector3 verticalMove;
            // Normalize the vector to a magnitude of 1
            verticalMove = climbDirection.normalized;
            // Multiply the vector with the Player's input [1 , -1]
            verticalMove *= Input.GetAxis("Vertical");
            // Ask if the player is touching the ground to be able to move back
            lateralMove = new Vector3(Input.GetAxis("Horizontal"), 0, 
                isGrounded ? Input.GetAxis("Vertical") : 0);
            // Fixes the lateral movement
            lateralMove = transform.TransformDirection(lateralMove);
            // combine vertical and lateral move to get the real movement while in the ladder
            ladderMovement = verticalMove + lateralMove;

            // Move
            m_CharacterController.Move(ladderMovement * climbSpeed * Time.deltaTime);

            // Asks if the player is moving in the ladder to play the sound
            if (Input.GetAxis("Vertical") != 0 && !(m_AudioSource.isPlaying) && Time.time >= playTime) {
                PlayLadderSound();
            }

            // Disables ladder climbing when the player presses space
            if (Input.GetButtonDown("Jump")) {
                m_onLadder = false;
            }
        }
    }

    private void PlayLandingSound() {
        m_AudioSource.clip = m_LandSound;
        m_AudioSource.Play();
        m_NextStep = m_StepCycle + .5f;
    }


    private void PlayJumpSound() {
        m_AudioSource.clip = m_JumpSound;
        m_AudioSource.Play();
    }


    private void ProgressStepCycle(float speed) {

        if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_Input.x != 0 || m_Input.y != 0)) {
            if (!m_Jumping) {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            } else if (m_Input.x == -1) {
                m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunstepLenghten))) *
                             Time.fixedDeltaTime;
            }
        }

        if (!(m_StepCycle > m_NextStep)) {
            return;
        }

        m_NextStep = m_StepCycle + m_StepInterval;

        if (!m_Crouching)
            PlayFootStepAudio();
    }

    /// <summary>
    /// Plays FootStep Sounds
    /// </summary>
    private void PlayFootStepAudio() {
        if (!isGrounded) {
            return;
        }
        // pick & play a random footstep sound from the array,
        // excluding sound at index 0
        int n = Random.Range(1, m_FootstepSounds.Length);
        m_AudioSource.clip = m_FootstepSounds[n];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        // move picked sound to index 0 so it's not picked next time
        m_FootstepSounds[n] = m_FootstepSounds[0];
        m_FootstepSounds[0] = m_AudioSource.clip;
    }

    /// <summary>
    /// Plays Ladder Sounds
    /// </summary>
    void PlayLadderSound() {
        int r = Random.Range(0, m_LadderSounds.Length);
        m_AudioSource.clip = m_LadderSounds[r];
        m_AudioSource.PlayOneShot(m_AudioSource.clip);
        playTime = Time.time + climbRate;
    }


    private void UpdateCameraPosition(float speed) {
        Vector3 newCameraPosition = Vector3.zero;
        if (!m_UseHeadBob) {
            return;
        }
        if (m_CharacterController.velocity.magnitude > 0 && isGrounded) {
            m_Camera.transform.localPosition = Vector3.Lerp(m_Camera.transform.localPosition,
                m_HeadBob.DoHeadBob(m_CharacterController.velocity.magnitude +
                (speed * (m_IsWalking ? 1f : m_RunstepLenghten))), Time.deltaTime * 6f);
            newCameraPosition = m_Camera.transform.localPosition;
            newCameraPosition.y = m_Camera.transform.localPosition.y - m_JumpBob.Offset();
        } else {
            newCameraPosition = Vector3.Lerp(newCameraPosition, m_Camera.transform.localPosition, Time.deltaTime * 6f);
            newCameraPosition.y = m_OriginalCameraPosition.y - m_JumpBob.Offset();
        }
        m_Camera.transform.localPosition = Vector3.Lerp(m_Camera.transform.localPosition, newCameraPosition, Time.deltaTime * 6f);
    }


    private void GetInput(out float speed) {
        // Read input
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");


        bool waswalking = m_IsWalking;

        // On standalone builds, walk/run speed is modified by a key press.
        // keep track of whether or not the character is walking or running
        m_IsWalking = !Input.GetKey(KeyCode.LeftShift);

        // set the desired speed to be walking or running
        speed = m_IsWalking ? m_WalkSpeed : m_RunSpeed;
        m_Input = new Vector2(horizontal, vertical);

        // normalize input if it exceeds 1 in combined length:
        if (m_Input.sqrMagnitude > 1) {
            m_Input.Normalize();
        }

        // handle speed change to give an fov kick
        // only if the player is going to a run, is running and the fovkick is to be used
        if (m_IsWalking != waswalking && m_UseFovKick && m_CharacterController.velocity.sqrMagnitude > 0) {
            StopAllCoroutines();
            StartCoroutine(!m_IsWalking ? m_FovKick.FOVKickUp() : m_FovKick.FOVKickDown());
        }
    }


    private void RotateView() {
        m_MouseLook.LookRotation(transform, m_Camera.transform);
    }


    private void OnControllerColliderHit(ControllerColliderHit hit) {
        Rigidbody body = hit.collider.attachedRigidbody;
        //dont move the rigidbody if the character is on top of it
        if (m_CollisionFlags == CollisionFlags.Below) {
            return;
        }

        if (body == null || body.isKinematic) {
            return;
        }
        body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
    }

    private bool CheckHeadCollision() {
        if (m_CollisionFlags == CollisionFlags.Above) {
            return true;
        } else {
            return false;
        }
    }
}
