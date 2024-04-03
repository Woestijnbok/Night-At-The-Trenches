using UnityEngine;
using UnityEngine.UIElements;
using static Player;

public class InGame : MonoBehaviour
{
    #region Variables
    // Member variables open to the editor ([SerializeField])
    private HUD m_HUD = null;
    private UIDocument m_AttachedDocument = null;
    private VisualElement m_Root = null;
    private IntegerField m_AllyCounter = null;
    private Label m_WaveLabel = null;
    private IntegerField m_AmmoCounter = null;
    private VisualElement m_GasGrenadeIcon = null;
    private VisualElement m_SmokeGrenadeIcon = null;
    private VisualElement m_MedicPackIcon = null;
    private VisualElement m_BulletIcon = null;
    #endregion

    #region UnityFunctions
    private void Awake()
    {
        if (TryGetComponent<UIDocument>(out m_AttachedDocument)) 
        {
            m_Root = m_AttachedDocument.rootVisualElement;

            if (m_Root != null)
            {
                m_AllyCounter = m_Root.Q<IntegerField>("AllyCounter");
                if (m_AllyCounter == null) Debug.LogWarning("Couldn't find the ally counter");

                m_WaveLabel = m_Root.Q<Label>("WaveNumber");
                if (m_WaveLabel == null) Debug.LogWarning("Couldn't find the wave label");

                m_AmmoCounter = m_Root.Q<IntegerField>("AmmoCounter");
                if (m_AmmoCounter == null) Debug.LogWarning("Couldn't find the ammo counter");

                m_GasGrenadeIcon = m_Root.Q<VisualElement>("GasGrenadeIcon");
                if (m_GasGrenadeIcon == null) Debug.LogWarning("Couldn't find gas grendade icon");

                m_SmokeGrenadeIcon = m_Root.Q<VisualElement>("SmokeGrenadeIcon");
                if (m_SmokeGrenadeIcon == null) Debug.LogWarning("Couldn't find smoke grenade icon");

                m_MedicPackIcon = m_Root.Q<VisualElement>("MedicPackIcon");
                if (m_MedicPackIcon == null) Debug.LogWarning("Couldn't find medic pack icon");

                m_BulletIcon = m_Root.Q<VisualElement>("BulletIcon");
                if (m_BulletIcon == null) Debug.LogWarning("Couldn't find bullet icon");

            }
            else Debug.LogWarning("In game ui could not find root of ui document");
        }
        else Debug.LogWarning("In game ui could not find it's ui document");


        Field field = FindFirstObjectByType<Field>();
        if(field != null) field.OnAlliesChanged += UpdateAllyCounter;
        else Debug.LogWarning("Can't find object with Field component in scene");

        m_HUD = gameObject.GetComponentInParent<HUD>();
        if (m_HUD == null) Debug.LogWarning("Couldn't find object with hud component in scene");
    }
    #endregion

    #region NewFunctions
    public void UpdateAllyCounter(int amount)
    {
        m_AllyCounter.value = amount;
    }

    public void SetWaveLabel(in int waveNumber)
    {
        m_WaveLabel.text = "Wave " + waveNumber.ToString();
    }

    public void Toggle(in bool show)
    {
        m_Root.visible = show;
    }

    public void SetAmmoCounter(in EquipmentType type, in int amount)
    {
        // Hide every ammo icon
        m_SmokeGrenadeIcon.visible = false;
        m_GasGrenadeIcon.visible = false;
        m_MedicPackIcon.visible = false;
        m_BulletIcon.visible = false;

        // Show the wanted ammo icon
        switch (type)
        {
            case EquipmentType.Gun:
                m_BulletIcon.visible = true;
                break;
            case EquipmentType.MedicPack:
                m_MedicPackIcon.visible = true;
                break;
            case EquipmentType.SmokeGrenade:
                m_SmokeGrenadeIcon.visible = true;
                break;
            case EquipmentType.GasGrenade:
                m_GasGrenadeIcon.visible = true;
                break;
        }

        // Set the ammo counter to the given amount
        m_AmmoCounter.value = amount;
    }
    #endregion
}
