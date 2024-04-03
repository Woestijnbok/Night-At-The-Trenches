using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using System.Linq;


public class Player : MonoBehaviour
{
    #region NewDataTypes
    public enum EquipmentType
    {
        Gun,
        MedicPack,
        SmokeGrenade,
        GasGrenade
    }
    #endregion

    #region Variables
    // Shared member variables (const or static)
    static private Plane m_Plane = new Plane(Vector3.back, Vector3.forward * 10000f);

    // Member variables open to the editor ([SerializeField])
    [SerializeField] private InputActionAsset m_PlayerInput = null;
    [SerializeField] private Gun m_GunTemplate = null;
    [SerializeField] private MedicPack m_MedicPackTemplate = null;
    [SerializeField] private SmokeGrenade m_SmokeGrenadeTemplate = null;
    [SerializeField] private GasGrenade m_GasGrenadeTemplate = null;
    [SerializeField] private int m_StartAmmoCounter = 30;
    [SerializeField] private int m_StartGasGrenadeCounter = 3;
    [SerializeField] private int m_StartSmokeGrenadeCounter = 3;
    [SerializeField] private int m_StartMedicPackCounter = 6;


    // Class & struct & enum member variables
    private List<Projectile> m_Equipment = new List<Projectile>();
    private InputAction m_InputActionFireThrow = null;
    private InputAction m_InputActionGun = null;
    private InputAction m_InputActionMedicPack = null;
    private InputAction m_InputActionSmokeGrenade = null;
    private InputAction m_InputActionGasGrenade = null;
    private InputAction m_InputActionMove = null;
    private Gun m_Gun = null;
    private Field m_Field = null;
    private HUD m_HUD = null;
    private AudioSource m_AudioSource = null;
    private EquipmentType m_EquipmentType = EquipmentType.Gun;

    // Primitive member variables
    private int m_AmmoCounter = 30;
    private int m_GasGrenadeCounter = 3;
    private int m_SmokeGrenadeCounter = 3;
    private int m_MedicPackCounter = 6;
    #endregion

    #region Properties
    public EquipmentType CurrentEquipment
    {
        get { return m_EquipmentType; }
        set { m_EquipmentType = value; }
    }

    public int NrOfBullets
    {
        get { return m_AmmoCounter; }
        set { m_AmmoCounter = value; }
    }

    public int NrOfMedicPacks
    {
        get { return m_MedicPackCounter; }
        set { m_MedicPackCounter = value; }
    }

    public int NrOfSmokeGrenades
    {
        get { return m_SmokeGrenadeCounter; }
        set { m_SmokeGrenadeCounter = value; }
    }

    public int NrOfGasGrenades
    {
        get { return m_GasGrenadeCounter; }
        set { m_GasGrenadeCounter = value; }
    }
    #endregion

    #region UnityFunctions
    private void Awake()
    {
        // Make the gun
        m_Gun = Instantiate(m_GunTemplate, transform, true);
        m_Gun.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);

        // Cash variables
        if (!TryGetComponent<AudioSource>(out m_AudioSource)) Debug.LogWarning("Player couldn't find audio source component");

        m_Field = FindFirstObjectByType<Field>();
        if (m_Field == null) Debug.LogWarning("Scene doesn't contain an object with Field component");

        m_HUD = FindFirstObjectByType<HUD>();
        if (m_HUD == null) Debug.LogWarning("Scene doesn't contain an object with HUD component");

        if (m_PlayerInput != null)
        {
            InputActionMap inputActionMap = m_PlayerInput.FindActionMap("Gameplay");

            if (inputActionMap != null) 
            {
                m_InputActionFireThrow = inputActionMap.FindAction("Fire / Throw");
                if (m_InputActionFireThrow == null) Debug.LogWarning("Couldn't find 'Fire / Throw'(Action) in 'Gameplay'(ActionMap)");

                m_InputActionGun = inputActionMap.FindAction("Gun");
                if (m_InputActionGun == null) Debug.LogWarning("Couldn't find 'Gun'(Action) in 'Gameplay'(ActionMap)");

                m_InputActionMedicPack = inputActionMap.FindAction("Medic Pack");
                if (m_InputActionMedicPack == null) Debug.LogWarning("Couldn't find 'Medic Pack'(Action) in 'Gameplay'(ActionMap)");

                m_InputActionSmokeGrenade = inputActionMap.FindAction("Smoke Grenade");
                if (m_InputActionSmokeGrenade == null) Debug.LogWarning("Couldn't find 'Smoke Grenade'(Action) in 'Gameplay'(ActionMap)");

                m_InputActionGasGrenade = inputActionMap.FindAction("Gas Grenade");
                if (m_InputActionGasGrenade == null) Debug.LogWarning("Couldn't find 'Gas Grenade'(Action) in 'Gameplay'(ActionMap)");

                m_InputActionMove = inputActionMap.FindAction("Move");
                if (m_InputActionMove == null) Debug.LogWarning("Couldn't find 'Move'(Action) in 'Gameplay'(ActionMap)");
            }
            else Debug.LogWarning("Couldn't find 'Gameplay'(ActionMap) from the given InputActionAsset");
        }
        else Debug.LogWarning("Given INputActionAsset is null");

        // Subscribe to input action events
        m_InputActionFireThrow.performed += FireThrow;
        m_InputActionGun.performed += EquipGun;
        m_InputActionMedicPack.performed += EquipMedicPack;
        m_InputActionSmokeGrenade.performed += EquipSmokeGrenade;
        m_InputActionGasGrenade.performed += EquipGasGrenade;

        // Set varaiables
        m_AmmoCounter = m_StartAmmoCounter;
        m_GasGrenadeCounter = m_StartGasGrenadeCounter;
        m_SmokeGrenadeCounter = m_StartSmokeGrenadeCounter;
        m_MedicPackCounter = m_StartMedicPackCounter;
    }

    private void OnEnable()
    {
        m_PlayerInput.Enable();
    }

    private void Start()
    {
        m_HUD.InGameUI.SetAmmoCounter(m_EquipmentType, m_AmmoCounter);
    }

    private void FixedUpdate()
    {
        // Move character if the wave is in progress and a certain input is pressed, clip x position so we don't Show outside the level bounds
        if (m_Field.WaveInProgress)
        {
            Vector3 newPosition = transform.position + 10.0f * Time.deltaTime * m_InputActionMove.ReadValue<float>() * -Vector3.right;
            newPosition.x = Mathf.Clamp(newPosition.x, -19.0f, 19.0f);
            transform.position = newPosition;
        }
    }
        
    private void OnDisable()
    {
        m_PlayerInput.Disable();
    }
    #endregion

    #region NewFunctions
    private void FireThrow(InputAction.CallbackContext context)
    {
        // Throw or fire the currently equipt equipment

        if (m_Field.WaveInProgress)
        {
            switch (m_EquipmentType)
            {
                case EquipmentType.Gun:
                    if (m_AmmoCounter > 0)
                    {
                        Shoot();
                    }
                    break;
                case EquipmentType.MedicPack:
                    if (m_MedicPackCounter > 0)
                    {
                        ThrowMedicPack();
                    }
                    break;
                case EquipmentType.SmokeGrenade:
                    if (m_SmokeGrenadeCounter > 0)
                    {
                        ThrowSmokeGrenade();
                    }
                    break;
                case EquipmentType.GasGrenade:
                    if (m_GasGrenadeCounter > 0)
                    {
                        ThrowGasGrenade();
                    }
                    break;
            }
        }
    }

    private void EquipGun(InputAction.CallbackContext context)
    {
        // If the wave is in progress equip the gun and update the in game ui

        if (!m_Field.WaveInProgress) return;

        m_EquipmentType = EquipmentType.Gun;
        m_HUD.InGameUI.SetAmmoCounter(m_EquipmentType, m_AmmoCounter);
    }

    private void EquipMedicPack(InputAction.CallbackContext context)
    {
        // If the wave is in progress equip the medic pack and update the in game ui

        if (!m_Field.WaveInProgress) return;

        m_EquipmentType = EquipmentType.MedicPack;
        m_HUD.InGameUI.SetAmmoCounter(m_EquipmentType, m_MedicPackCounter);
    }

    private void EquipSmokeGrenade(InputAction.CallbackContext context)
    {
        // If the wave is in progress equip the smoke grenade and update the in game ui

        if (!m_Field.WaveInProgress) return;

        m_EquipmentType = EquipmentType.SmokeGrenade;
        m_HUD.InGameUI.SetAmmoCounter(m_EquipmentType, m_SmokeGrenadeCounter);
    }

    private void EquipGasGrenade(InputAction.CallbackContext context)
    {
        // If the wave is in progress equip the gas grenade and update the in game ui

        if (!m_Field.WaveInProgress) return;

        m_EquipmentType = EquipmentType.GasGrenade;
        m_HUD.InGameUI.SetAmmoCounter(m_EquipmentType, m_GasGrenadeCounter);
    }

    private Vector3 GetMouseDirection()
    {
        Vector3 mousePosition = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePosition);

        // Raycast to any object in the scene wher the ray can hit
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return (hit.point - m_Gun.transform.position).normalized;
        }
        // Default raycast with back plane (invisible)
        else
        {
            if (m_Plane.Raycast(ray, out float t))
            {
                Vector3 hitPoint = ray.GetPoint(t);

                return (hitPoint - m_Gun.transform.position).normalized;
            }
        }

        return Vector3.zero;
    }

    private void Shoot()
    {
        // Make the gun shoot and change the in game ui ammo counter

        m_Gun.Shoot(GetMouseDirection());
        --m_AmmoCounter;
        m_HUD.InGameUI.SetAmmoCounter(m_EquipmentType, m_AmmoCounter);
    }

    private void ThrowGasGrenade()
    {
        // Make a gas grenade and add it to list, play audio, change in game ui ammo counter

        m_Equipment.Add(Instantiate<GasGrenade>(m_GasGrenadeTemplate, transform.position, transform.rotation));
        m_Equipment.Last().Direction = GetMouseDirection();
        m_AudioSource.Play();
        --m_GasGrenadeCounter;
        m_HUD.InGameUI.SetAmmoCounter(m_EquipmentType, m_GasGrenadeCounter);
    }

    private void ThrowSmokeGrenade()
    {
        // Make a smoke grenade and add it to list, play audio, change in game ui ammo counter

        m_Equipment.Add(Instantiate<SmokeGrenade>(m_SmokeGrenadeTemplate, transform.position, transform.rotation));
        m_Equipment.Last().Direction = GetMouseDirection();
        m_AudioSource.Play();
        --m_SmokeGrenadeCounter;
        m_HUD.InGameUI.SetAmmoCounter(m_EquipmentType, m_SmokeGrenadeCounter);
    }

    private void ThrowMedicPack()
    {
        // Make a medic pack and add it to list, play audio, change in game ui ammo counter

        m_Equipment.Add(Instantiate<MedicPack>(m_MedicPackTemplate, transform.position, transform.rotation));
        m_Equipment.Last().Direction = GetMouseDirection(); ;
        m_AudioSource.Play();
        --m_MedicPackCounter;
        m_HUD.InGameUI.SetAmmoCounter(m_EquipmentType, m_MedicPackCounter);
    }

    public void Restart()
    {
        // Reset variables and change the in game ui using these fresh variables

        m_AmmoCounter = m_StartAmmoCounter;
        m_GasGrenadeCounter = m_StartGasGrenadeCounter;
        m_SmokeGrenadeCounter = m_StartSmokeGrenadeCounter;
        m_MedicPackCounter = m_StartMedicPackCounter;
        m_EquipmentType = EquipmentType.Gun;
        m_HUD.InGameUI.SetAmmoCounter(m_EquipmentType, m_AmmoCounter);
    }

    public void DeleteProjectiles()
    {
        // Delete all the bullets
        m_Gun.Reset();

        // Delete all other projectiles
        foreach (Projectile equipment in m_Equipment)
        {
            if (equipment != null)
            {
                Destroy(equipment.gameObject);
            }
        }

        m_Equipment.Clear();
    }
    #endregion
}