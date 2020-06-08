using System;
using System.Collections;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityStandardAssets.Utility;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(AudioSource))]
public class Player : MonoBehaviour
{
  [SerializeField] public float m_GravityMultiplier;
  [SerializeField] private float m_Health;
  [SerializeField] private bool m_IsWalking;
  [SerializeField] private float m_Speed;
  [SerializeField] private float m_DashThrust = 40f;
  [SerializeField] private float m_DashHeight = 1f;
  [SerializeField] private float m_JumpSpeed = 5f;
  [SerializeField] private float m_MaxJumps = 2;
  [SerializeField] private float m_KnockdownThreshold = 10f;
  [SerializeField] private float m_KnockdownDuration = 3f;
  [SerializeField] private float m_StickToGroundForce;
  [SerializeField] private MouseLook m_MouseLook;
  [SerializeField] private AudioClip m_JumpSound; // the sound played when character leaves the ground.
  [SerializeField] private AudioClip m_LandSound; // the sound played when character touches back on ground.
  [SerializeField] private float m_AutoTargetingRadius = 3f;
  [SerializeField] private float m_AutoTargetingRange = 10f;
  [SerializeField] private float m_AutoTargetingForce = 5f;
  [SerializeField] private float m_AutoTargetingStoppingDistance = 1f;
  private Animator m_Animator;
  private Rigidbody m_RigidBody;
  private Camera m_Camera;
  private bool m_CanJump = false;
  private bool m_CanDash = false;
  private float m_CurrentJumpCount = 0f;
  private float m_YRotation;
  private Vector2 m_LocomotionInput;
  private Vector3 m_MoveDir = Vector3.zero;
  private CharacterController m_CharacterController;
  private CollisionFlags m_CollisionFlags;
  private bool m_PreviouslyGrounded;
  private bool m_IsJumping;
  private AudioSource m_AudioSource;
  private float m_DoubleTapLastTapped = -0.1f;
  private Vector3 m_Impact = Vector3.zero;
  public Transform m_ClosestTargetTransform;
  private bool m_IsOverridingMouseLook = false;

  // Layer 8 is the Player Layer
  // Bit shift the index of the layer (8) to get a bit mask
  // This would cast rays only against colliders in layer 8.
  // But instead we want to collide against everything except layer 8. The ~ operator does this, it inverts a bitmask.
  private LayerMask m_LayerMask;

  // Use this for initialization
  private void Awake()
  {
    m_CharacterController = GetComponent<CharacterController>();
    m_RigidBody = GetComponent<Rigidbody>();
    m_Camera = Camera.main;
    m_StepCycle = 0f;
    m_NextStep = m_StepCycle / 2f;
    m_IsJumping = false;
    m_AudioSource = GetComponent<AudioSource>();
    m_MouseLook.Init(transform, m_Camera.transform);
    m_Animator = GetComponent<Animator>();
  }

  // Update is called once per frame
  private void Update()
  {

    RotateView();

    // reset jump if grounded
    if (m_CharacterController.isGrounded)
    {
      m_CurrentJumpCount = 0;
    }

    // jump
    // we want jump to be in update vs fixedupdate because it keeps getting missed in fixedupdate
    m_CanJump = CrossPlatformInputManager.GetButtonDown("Jump") && m_CurrentJumpCount < m_MaxJumps;
    if (m_CanJump)
    {
      m_CurrentJumpCount++;
      m_Animator.SetTrigger("jump");

      m_MoveDir.y = m_JumpSpeed;

      AddImpact(m_MoveDir, m_JumpSpeed);

      PlayJumpSound();
      m_IsJumping = true;
    }

    // dash
    if (Input.GetButtonDown("Horizontal") || Input.GetButtonDown("Vertical"))
    {

      if ((Time.time - m_DoubleTapLastTapped) < m_DoubleTapDelay)
      {
        m_CanDash = true;
      }
      m_DoubleTapLastTapped = Time.time;

    }
    // landed
    if (!m_PreviouslyGrounded && m_CharacterController.isGrounded)
    {
      PlayLandingSound();
      m_MoveDir.y = 0f;
      m_IsJumping = false;
    }
    m_Animator.SetBool("isGrounded", m_CharacterController.isGrounded);
    m_PreviouslyGrounded = m_CharacterController.isGrounded;

    GetClosestTarget();
  }

  private void FixedUpdate()
  {

    // apply the impact force:
    if (m_Impact.magnitude > 0.2) m_CharacterController.Move(m_Impact * Time.deltaTime);
    // consumes the m_Impact energy each cycle:
    m_Impact = Vector3.Lerp(m_Impact, Vector3.zero, 5 * Time.deltaTime);

    // apply gravity forces
    if (m_CharacterController.isGrounded)
    {
      m_MoveDir.y = -m_StickToGroundForce;
    }
    else
    {
      m_MoveDir += Physics.gravity * m_GravityMultiplier * Time.fixedDeltaTime;
    }

    // movement
    bool canMove = !m_Animator.GetBool("isGuarding") && !m_Animator.GetBool("isKnockdowned"); // you can be changed later
    if (canMove)
    {
      float speed;
      SetLocomotionInput(out speed);
      // always move along the camera forward as it is the direction that it being aimed at
      Vector3 desiredMove = transform.forward * m_LocomotionInput.y + transform.right * m_LocomotionInput.x;

      // get a normal for the surface that is being touched to move along it
      RaycastHit hitInfo;
      Physics.SphereCast(transform.position, m_CharacterController.radius, Vector3.down, out hitInfo,
        m_CharacterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
      desiredMove = Vector3.ProjectOnPlane(desiredMove, hitInfo.normal).normalized;

      m_MoveDir.x = desiredMove.x * speed;
      m_MoveDir.z = desiredMove.z * speed;

      // dash
      if (m_CanDash)
      {

        m_Animator.SetTrigger("dash");

        m_MoveDir.x = m_MoveDir.x * m_DashThrust;
        m_MoveDir.y = m_DashHeight;
        m_MoveDir.z = m_MoveDir.z * m_DashThrust;

        AddImpact(m_MoveDir, m_DashThrust);

        m_CanDash = false;
      }

      ProgressStepCycle(speed);
    }
    else
    {
      // if guarding make sure they cant move
      m_MoveDir.x = 0f;
      m_MoveDir.z = 0f;
    }

    m_CollisionFlags = m_CharacterController.Move(m_MoveDir * Time.fixedDeltaTime);

    m_MouseLook.UpdateCursorLock();
  }

  private void GetClosestTarget()
  {

    RaycastHit[] hits = Physics.SphereCastAll(transform.position, m_AutoTargetingRadius, transform.forward, m_AutoTargetingRange, m_LayerMask);
    float closestDistance = Mathf.Infinity;
    Transform closestTargetTransform = null;
    foreach (RaycastHit hit in hits)
    {
      Target target = hit.transform.gameObject.GetComponent<Target>();
      if (target)
      {
        // get distance 
        float distance = Vector3.Distance(target.transform.position, transform.position);
        if (distance < closestDistance)
        {
          closestDistance = distance;
          closestTargetTransform = target.transform;
        }
      }
    }
    m_ClosestTargetTransform = closestTargetTransform;

  }

  public void PlayParticleSystem(string tagName)
  {
    ParticleSystem[] particleSystems = GameObject.FindWithTag(tagName).GetComponentsInChildren<ParticleSystem>();
    foreach (ParticleSystem particleSystem in particleSystems)
    {
      particleSystem.Play();
    }
  }

  public void StopParticleSystem(string tagName)
  {
    ParticleSystem[] particleSystems = GameObject.FindWithTag(tagName).GetComponentsInChildren<ParticleSystem>();
    foreach (ParticleSystem particleSystem in particleSystems)
    {
      particleSystem.Stop();
    }
  }

  public void MoveTowardsClosestTarget()
  {
    if (m_ClosestTargetTransform && CrossPlatformInputManager.GetAxis("Vertical") >= 0f)
    {
      Vector3 direction = (m_ClosestTargetTransform.position - transform.position).normalized;
      m_IsOverridingMouseLook = true;
      // // // rotate to look at
      float distance = Vector3.Distance(m_ClosestTargetTransform.position, transform.position);

      if (distance > m_AutoTargetingStoppingDistance)
      {
        AddImpact(direction, m_AutoTargetingForce);
      }

      StartCoroutine(ResetCameraLook());

      // m_IsOverridingMouseLook = false;
    }
  }

  IEnumerator ResetCameraLook()
  {
    yield return new WaitForSeconds(1f);

    m_IsOverridingMouseLook = false;
  }

  private void PlayLandingSound()
  {
    m_AudioSource.clip = m_LandSound;
    m_AudioSource.Play();
    m_NextStep = m_StepCycle + .5f;
  }

  // call this function to add an impact force:
  public void AddImpact(Vector3 direction, float force)
  {
    direction.Normalize();
    if (direction.y < 0) direction.y = -direction.y; // reflect down force on the ground
    m_Impact += direction.normalized * force / m_RigidBody.mass;
  }

  private void PlayJumpSound()
  {
    m_AudioSource.clip = m_JumpSound;
    m_AudioSource.Play();
  }

  private void ProgressStepCycle(float speed)
  {
    if (m_CharacterController.velocity.sqrMagnitude > 0 && (m_LocomotionInput.x != 0 || m_LocomotionInput.y != 0))
    {
      m_StepCycle += (m_CharacterController.velocity.magnitude + (speed * (m_IsWalking ? 1f : m_RunStepLengthen))) *
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
    // pick & play a random footstep sound from the array,
    // excluding sound at index 0
    int n = Random.Range(1, m_FootstepSounds.Length);
    m_AudioSource.clip = m_FootstepSounds[n];
    m_AudioSource.PlayOneShot(m_AudioSource.clip);
    // move picked sound to index 0 so it's not picked next time
    m_FootstepSounds[n] = m_FootstepSounds[0];
    m_FootstepSounds[0] = m_AudioSource.clip;
  }

  private void SetLocomotionInput(out float speed)
  {
    // Read input
    float horizontal = CrossPlatformInputManager.GetAxis("Horizontal");
    float vertical = CrossPlatformInputManager.GetAxis("Vertical");

#if !MOBILE_INPUT
    // On standalone builds, walk/run speed is modified by a key press.
    // keep track of whether or not the character is walking or running
    // m_IsWalking = !Input.GetKey(KeyCode.LeftShift);
#endif

    // set the desired speed to be walking or running
    // speed = m_IsWalking ? m_Speed : m_RunSpeed;
    speed = m_Speed;
    m_LocomotionInput = new Vector2(horizontal, vertical);

    m_Animator.SetFloat("horizontal", horizontal);
    m_Animator.SetFloat("vertical", vertical);

    // normalize input if it exceeds 1 in combined length:
    if (m_LocomotionInput.sqrMagnitude > 1)
    {
      m_LocomotionInput.Normalize();
    }

  }

  private void RotateView()
  {
    if (m_IsLockedOn && m_LockOnTarget)
    {
      m_MouseLook.LockedLookRotation(transform, m_LockOnTarget.transform, m_Camera.transform);
    }
    else
    {
      // if (!m_IsOverridingMouseLook)
      // {

      m_MouseLook.LookRotation(transform, m_Camera.transform, m_IsOverridingMouseLook, m_ClosestTargetTransform);
      // }
    }
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
    body.AddForceAtPosition(m_CharacterController.velocity * 0.1f, hit.point, ForceMode.Impulse);
  }

  public void ToggleHitboxColliders(string name, bool isEnabled)
  {
    BaseHitBox[] hitboxes = GetComponentsInChildren<BaseHitBox>();
    for (int i = 0; i < hitboxes.Length; i++)
    {
      if (name == hitboxes[i].m_HitBoxName)
      {
        hitboxes[i].m_Collider.enabled = isEnabled;
      }
    }
  }

  public void DisableHitboxColliders()
  {
    BaseHitBox[] hitboxes = GetComponentsInChildren<BaseHitBox>();
    for (int i = 0; i < hitboxes.Length; i++)
    {
      hitboxes[i].m_Collider.enabled = false;
    }
  }

  IEnumerator RecoverFromKnockdown()
  {
    yield return new WaitForSeconds(m_KnockdownDuration);
    m_Animator.SetBool("isKnockdowned", false);
  }

  private void OnTriggerEnter(Collider otherCollider)
  {
    bool isGuarding = m_Animator.GetBool("isGuarding");
    if (otherCollider.gameObject.tag == "EnemyHitBox" && !isGuarding)
    {
      BaseHitBox enemyHitbox = otherCollider.gameObject.GetComponent<BaseHitBox>();
      Vector3 direction = enemyHitbox.GetDirection(m_RigidBody);
      float force = enemyHitbox.m_Damages[0].m_KnockbackForce;
      float damageAmount = enemyHitbox.m_Damages[0].m_DamageAmount;

      AddImpact(direction, force);

      if (damageAmount > m_KnockdownThreshold)
      {
        m_Animator.SetBool("isKnockdowned", true);
        StartCoroutine(RecoverFromKnockdown());
      }
      else
      {
        m_Animator.SetTrigger("stagger");
      }

      if (enemyHitbox.m_ShouldDestroyOnCollide)
      {
        Destroy(enemyHitbox.transform.gameObject);
      }
    }
  }

  private void OnParticleCollision(GameObject other)
  {
    bool isGuarding = m_Animator.GetBool("isGuarding");
    if (other.tag == "EnemyHitBox" && !isGuarding)
    {
      BaseHitBox enemyHitbox = other.GetComponent<BaseHitBox>();
      Vector3 direction = enemyHitbox.GetDirection(m_RigidBody);
      float force = enemyHitbox.m_Damages[0].m_KnockbackForce;
      float damageAmount = enemyHitbox.m_Damages[0].m_DamageAmount;

      AddImpact(direction, force);

      if (damageAmount > m_KnockdownThreshold)
      {
        m_Animator.SetBool("isKnockdowned", true);
        StartCoroutine(RecoverFromKnockdown());
      }
      else
      {
        m_Animator.SetTrigger("stagger");
      }

    }
  }
}