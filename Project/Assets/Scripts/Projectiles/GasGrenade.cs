using UnityEngine;

public class GasGrenade : Projectile
{
    #region Variables
    // Shared member variables (const or static)
    private const int m_DamagePerSecond = 50;
    private const float m_LeakTime = 5.0f;

    // Class & struct member variables
    private AudioSource m_AudioSource = null;

    // Primitive member variables
    private bool m_IsLeaking = false;
    private float m_LeakTimer = 0.0f;
    private float m_DamageTimer = 0.0f;
    private bool m_CanDamage = false;
    #endregion

    #region UnityFunctions
    private void Awake()
    {
        if (!TryGetComponent<AudioSource>(out m_AudioSource)) Debug.LogWarning("Gas grenade couldn't find audio source component");
    }

    override protected void FixedUpdate()
    {
        // When the gas grenade is leaking it will damage characters every second
        if (m_IsLeaking)
        {
            m_LeakTimer += Time.deltaTime;

            // If the gas grenade is empty delete it
            if (m_LeakTimer > m_LeakTime) Destroy(gameObject);
            else
            {
                m_DamageTimer += Time.deltaTime;

                // Check if the gas grenade is leaking for 1 second so we can start damaging characters
                if (m_DamageTimer > 1.0f)
                {
                    m_DamageTimer -= 1.0f;
                    m_CanDamage = true;
                }
                else if (m_CanDamage) m_CanDamage = false;
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
        // Every second we are leaking we damage a character if he is in our collider
        if (m_IsLeaking && m_CanDamage)
        {
            if (other.TryGetComponent<Health>(out Health otherHealth)) otherHealth.Damage(m_DamagePerSecond);
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
