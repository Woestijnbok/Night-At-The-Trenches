using UnityEngine;

public class SmokeGrenade : Projectile
{
    #region Variables
    // Shared member variables (const or static)
    static private Field m_Field = null;
    const float m_LeakTime = 3.0f;
    const float m_SmokeTime = 1.0f;

    // Class & struct member variables
    private AudioSource m_AudioSource = null;

    // Primitive member variables
    private bool m_IsLeaking = false;
    private float m_LeakTimer = 0.0f;
    private float m_SmokeTimer = 0.0f;
    private bool m_CanCover = false;
    #endregion

    #region UnityFunctions
    private void Awake()
    {
        if (!TryGetComponent<AudioSource>(out m_AudioSource)) Debug.LogWarning("Smoke grenade couldn't find audio source component");

        // Cash field this only happens once for the first smoke grenade
        if (m_Field == null)
        {
            m_Field = FindFirstObjectByType<Field>();
            if (m_Field == null) Debug.LogWarning("Couldn't find object with field component in scene");
        }
    }

    override protected void FixedUpdate()
    {
        // When the smoke grenade is leaking it will cover characters
        if (m_IsLeaking)
        {
            m_LeakTimer += Time.deltaTime;

            // If the smoke grenade is empty delete it
            if (m_LeakTimer > m_LeakTime) Destroy(gameObject);
            else
            {
                m_SmokeTimer += Time.deltaTime;

                // Check if the smoke grenade is leaking for the given interval so we can start covering characters
                if (m_SmokeTimer > m_SmokeTime)
                {
                    m_SmokeTimer -= m_SmokeTime;
                    m_CanCover = true;
                }
            }

           
        }
        // If it is not yet leaking just do the base projectile update
        else base.FixedUpdate();
    }

    private void OnTriggerEnter(Collider other)
    {
        // The first time we collide with someting: snap to the ground, start leaking and play a sound
        if (!m_IsLeaking)
        {
            m_AudioSource.Play();
            SnapToGround();
            m_IsLeaking = true;

            // Start leaking by making our collision area bigger
            if (TryGetComponent<SphereCollider>(out SphereCollider sphereCollider))
            {
                sphereCollider.radius *= 3.0f;
            }
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // Every frame we are leaking and can cover, we cover a character if he is in our collider
        if (m_IsLeaking && m_CanCover)
        {
            if (other.TryGetComponent<Character>(out Character _))
            {
                if (other.TryGetComponent<Enemy>(out Enemy _)) m_Field.EnemyCrossed();
                else m_Field.RescuedAlly();

                // Delete the rescued character
                Destroy(other.gameObject);
            }
        }
    }
    #endregion

    #region NewFunctions
    private void SnapToGround()
    {
        // Set the y-value and speed to 0
        Vector3 position = transform.position;
        position.y = 0.0f;
        transform.position = position;
        m_Speed = 0.0f;
    }
    #endregion
}
