using UnityEngine;

// Base class of all ai characters
public class Character : MonoBehaviour
{
    #region Variables
    // Shared member variables (const or static)
    protected const float m_DirectionTime = 3.0f;

    // Member variables open to the editor ([SerializeField])
    [SerializeField] protected bool m_IsInjured = false;

    // Class & struct member variables
    protected AudioSource m_HealedSound;
    protected Animator m_Animator = null;

    // Primitive member variables
    protected float m_HealthySpeed = 3.0f;
    protected float m_InjuredSpeed = 0.1f;
    protected float m_NormalSpeed = 1.0f;
    protected float m_Speed = 1.0f;
    protected bool m_IsDying = false;
    protected float m_DirectionTimer = 0.0f;
    #endregion

    #region Properties
    public float HealthySpeed
    {
        get { return m_HealthySpeed; }
        set { m_HealthySpeed = value; }
    }

    public float InjuredSpeed
    {
        get { return m_InjuredSpeed; }
        set { m_InjuredSpeed = value; }
    }

    public float NormalSpeed
    {
        get { return m_NormalSpeed; }
        set { m_NormalSpeed = value; }
    }

    public bool IsInjured
    {
        get { return m_IsInjured; }
        set
        {
            if (value)
            {
                m_Speed = m_InjuredSpeed;
                m_IsInjured = true;
            }
            else    // Healed characters are the fastests
            {
                m_Speed = m_HealthySpeed;
                m_IsInjured = false;
            }
        }
    }
    #endregion

    #region UnityFunctions
    protected virtual void Awake()
    {
        if (!TryGetComponent<AudioSource>(out m_HealedSound)) Debug.LogWarning("Audio source component not found for character");
    }

    protected virtual void Start()
    {
        if (m_IsInjured) m_Speed = m_InjuredSpeed;
        else m_Speed = m_NormalSpeed;

        // Rotate in range of the player
        ChangeDirection();
    }

    protected virtual void FixedUpdate()
    {
       if(!m_IsDying) Move(Time.deltaTime);
    }

    protected virtual void Update()
    {
        if(m_IsDying)
        {
            // Check if the death animation is over, ifso destroy this object
            if(m_Animator.GetCurrentAnimatorStateInfo(0).IsTag("Death") && m_Animator.GetCurrentAnimatorStateInfo(0).normalizedTime > 1.0f)
            {
                Destroy(gameObject);
            }
        }
    }
    #endregion

    #region NewFunctions
    protected void Move(float deltaTime)
    {
        // If we passed a set time we change our change our direction again
        m_DirectionTimer += deltaTime;
        if(m_DirectionTimer > m_DirectionTime)
        {
            m_DirectionTimer -= m_DirectionTime;
           ChangeDirection();
        }

        // Move forward in the LOCAL FORWARD DIRECTION
        transform.position += deltaTime * m_Speed * (transform.rotation * Vector3.forward);
    }

    protected void ChangeDirection()
    {
        // Rotate randomly around the y-axis, between a certain range TOWARDS THE PLAYER
        int angle = UnityEngine.Random.Range(110, 250);
        transform.rotation = Quaternion.Euler(0.0f, (float)angle, 0.0f);
    }

    public void Die()
    {
        // Disable our collider and rigidbody so we don't inte
        if (TryGetComponent<Collider>(out Collider collider))
        {
            collider.enabled = false;

            if (TryGetComponent<Rigidbody>(out Rigidbody rigidbody))
            {
                rigidbody.isKinematic = true;
            }
            else Debug.LogWarning("Character doesn't have rigidbody component");
        }
        else Debug.LogWarning("Character doesn't have collider component");

        // Adapt the animator so he starts the death animation
        if (m_Animator != null) m_Animator.SetBool("IsDying", true);
        else Debug.LogWarning("Character couldn't find animator component");

        m_IsDying = true;
    }

    public void PlayHealSound()
    {
        m_HealedSound.Play();
    }
    #endregion
}
