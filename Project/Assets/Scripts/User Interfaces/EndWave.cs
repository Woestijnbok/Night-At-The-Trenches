using UnityEngine;
using UnityEngine.UIElements;
using System;

public class EndWave : MonoBehaviour
{
    #region Variables
    // Class & struct member variables
    private HUD m_HUD = null;
    private UIDocument m_AttachedDocument = null;
    private VisualElement m_Root = null;
    private GroupBox m_WaveBox = null;
    private IntegerField m_EnemiesKilled = null;
    private IntegerField m_AlliesRescued = null;
    private IntegerField m_AlliesKilled = null;
    private IntegerField m_AlliesLeft = null;
    private GroupBox m_ResupplyBox = null;
    private IntegerField m_NrOfResuppliesIntegerField = null;
    private SliderInt m_BulletSlider = null;
    private SliderInt m_MedicPackSlider = null;
    private SliderInt m_SmokeGrenadeSlider = null;
    private SliderInt m_GasGrenadeSlider = null;
    private Button m_BuyButton = null;

    // Primitive member variables
    private int m_NrOfResupplies = 0;
    private int m_OldBulletSliderValue = 0;
    private int m_OldMedicPackSliderValue = 0;
    private int m_OldSmokeGrenadeSliderValue = 0;
    private int m_OldGasGrenadeSliderValue = 0;
    #endregion

    #region UnityFunctions
    private void Awake()
    {
        // Cash variables
        if (TryGetComponent<UIDocument>(out m_AttachedDocument))
        {
            m_Root = m_AttachedDocument.rootVisualElement;

            if (m_Root != null)
            {
                m_WaveBox = m_Root.Q<GroupBox>("WaveBox");
                if (m_WaveBox != null)
                {
                    m_EnemiesKilled = m_WaveBox.Q<IntegerField>("EnemiesKilled");
                    if (m_EnemiesKilled == null) Debug.LogWarning("End wave ui couldn't extract enemies killed integer field");

                    m_AlliesRescued = m_WaveBox.Q<IntegerField>("AlliesRescued");
                    if (m_AlliesRescued == null) Debug.LogWarning("End wave ui couldn't extract allies rescued integer field");

                    m_AlliesKilled = m_WaveBox.Q<IntegerField>("AlliesKilled");
                    if (m_AlliesKilled == null) Debug.LogWarning("End wave ui couldn't extract allies killed integer field");

                    m_AlliesLeft = m_WaveBox.Q<IntegerField>("AlliesLeft");
                    if (m_AlliesLeft == null) Debug.LogWarning("End wave ui couldn't extract allies left integer field");

                }
                else Debug.LogWarning("End wave ui couldn't extract WaveBox of root");


                m_ResupplyBox = m_Root.Q<GroupBox>("ResupplyBox"); 
                if (m_ResupplyBox != null)
                {
                    m_NrOfResuppliesIntegerField = m_ResupplyBox.Q<IntegerField>("NrOfResuppliesIntegerField");
                    if (m_NrOfResuppliesIntegerField == null) Debug.LogWarning("End wave ui couldn't extract resupplies integer field from root");

                    m_BulletSlider = m_ResupplyBox.Q<SliderInt>("BulletSlider");
                    if (m_BulletSlider == null) Debug.LogWarning("End wave ui couldn't extract bullet slider");
                    else m_BulletSlider.RegisterCallback<ClickEvent>(BulletSliderChanged);

                    m_MedicPackSlider = m_ResupplyBox.Q<SliderInt>("MedicPackSlider");
                    if (m_MedicPackSlider == null) Debug.LogWarning("End wave ui couldn't extract medic pack slider");
                    else m_MedicPackSlider.RegisterCallback<ClickEvent>(MedicPackSliderChanged);

                    m_SmokeGrenadeSlider = m_ResupplyBox.Q<SliderInt>("SmokeGrenadeSlider");
                    if (m_SmokeGrenadeSlider == null) Debug.LogWarning("End wave ui couldn't extract smoke grenade slider");
                    else m_SmokeGrenadeSlider.RegisterCallback<ClickEvent>(SmokeGrenadeSliderChanged);

                    m_GasGrenadeSlider = m_ResupplyBox.Q<SliderInt>("GasGrenadeSlider");
                    if (m_GasGrenadeSlider == null) Debug.LogWarning("End wave ui couldn't extract gas grenade slider");
                    else m_GasGrenadeSlider.RegisterCallback<ClickEvent>(GasGrenadeSliderChanged);
                }
                else Debug.LogWarning("End wave ui couldn't extract ResupplyBox of root");

                m_BuyButton = m_Root.Q<Button>("BuyButton");
                if(m_BuyButton != null) m_BuyButton.RegisterCallback<ClickEvent>(BuyButtonClicked);
                else Debug.LogWarning("End wave ui couldn't extract buy button of root");

            }
            else Debug.LogWarning("End wave ui couldn't extract root of ui document");
        }
        else Debug.LogWarning("End wave ui couldn't extract ui document component");

        m_HUD = gameObject.GetComponentInParent<HUD>();
        if (m_HUD == null) Debug.LogWarning("Couldn't find HUD object in scene");
    }
    #endregion

    #region NewFunctions
    public void Toggle(in bool show)
    {
        m_Root.visible = show;
    }

    public void BuyButtonClicked(ClickEvent clickEvent)
    {
        // Give information to hud of the bought supplies
        m_HUD.CurrentPlayer.NrOfBullets = m_OldBulletSliderValue;
        m_HUD.CurrentPlayer.NrOfMedicPacks = m_OldMedicPackSliderValue;
        m_HUD.CurrentPlayer.NrOfSmokeGrenades = m_OldSmokeGrenadeSliderValue;
        m_HUD.CurrentPlayer.NrOfGasGrenades = m_OldGasGrenadeSliderValue;
        m_HUD.CurrentPlayer.CurrentEquipment = Player.EquipmentType.Gun;
        m_HUD.InGameUI.SetAmmoCounter(m_HUD.CurrentPlayer.CurrentEquipment, m_HUD.CurrentPlayer.NrOfBullets);

        // Start the new wave
        m_HUD.CurrentField.NewWave();
    }
    
    public void SetWaveBox(in int nrEnemiesKilled, in int nrAlliesRescued, in int nrAlliesKilled, in int nrAlliesLeft, int waveNumber)
    {
        // Changes some elements in the wave box
        m_WaveBox.text = "Wave " + waveNumber.ToString() + " Completed";
        m_EnemiesKilled.value = nrEnemiesKilled;
        m_AlliesKilled.value = nrAlliesKilled;
        m_AlliesRescued.value = nrAlliesRescued;
        m_AlliesLeft.value = nrAlliesLeft;

        // Calculate the ammount of resupllies we can get
        m_NrOfResupplies = Math.Max((m_AlliesRescued.value - (m_AlliesKilled.value * 2) + nrEnemiesKilled), 0);
        m_NrOfResuppliesIntegerField.value = m_NrOfResupplies;
    }

    public void SetResupplyBox(in int NrOfBullets, in int NrOfMedicPacks, in int NrOfSmokeGrenades, in int NrOfGasGrenades)
    {
        // Change the min, max and current values of the sliders

        m_BulletSlider.lowValue = NrOfBullets;
        m_BulletSlider.value = NrOfBullets;
        m_OldBulletSliderValue = NrOfBullets;
        m_BulletSlider.highValue = NrOfBullets + m_NrOfResupplies;

        m_MedicPackSlider.lowValue = NrOfMedicPacks;
        m_MedicPackSlider.value = NrOfMedicPacks;
        m_OldMedicPackSliderValue = NrOfMedicPacks;
        m_MedicPackSlider.highValue = NrOfMedicPacks + m_NrOfResupplies;

        m_GasGrenadeSlider.lowValue = NrOfGasGrenades;
        m_GasGrenadeSlider.value = NrOfGasGrenades;
        m_OldGasGrenadeSliderValue = NrOfGasGrenades;
        m_GasGrenadeSlider.highValue = NrOfGasGrenades + m_NrOfResupplies;

        m_SmokeGrenadeSlider.lowValue = NrOfSmokeGrenades;
        m_SmokeGrenadeSlider.value = NrOfSmokeGrenades;
        m_OldSmokeGrenadeSliderValue = NrOfSmokeGrenades;
        m_SmokeGrenadeSlider.highValue = NrOfSmokeGrenades + m_NrOfResupplies;
    }

    public void BulletSliderChanged(ClickEvent clickEvent)
    {
        // Cash the new value
        int delta = m_OldBulletSliderValue - m_BulletSlider.value;
        m_OldBulletSliderValue = m_BulletSlider.value;

        // Change the other slider's max
        m_MedicPackSlider.highValue += delta;
        m_SmokeGrenadeSlider.highValue += delta;
        m_GasGrenadeSlider.highValue += delta;
        m_NrOfResuppliesIntegerField.value += delta;
    }

    public void MedicPackSliderChanged(ClickEvent clickEvent)
    {
        // Cash the new value
        int delta = m_OldMedicPackSliderValue - m_MedicPackSlider.value;
        m_OldMedicPackSliderValue = m_MedicPackSlider.value;

        // Change the other slider's max
        m_BulletSlider.highValue += delta;
        m_SmokeGrenadeSlider.highValue += delta;
        m_GasGrenadeSlider.highValue += delta;
        m_NrOfResuppliesIntegerField.value += delta;
    }

    public void SmokeGrenadeSliderChanged(ClickEvent clickEvent)
    {
        // Cash the new value
        int delta = m_OldSmokeGrenadeSliderValue - m_SmokeGrenadeSlider.value;
        m_OldSmokeGrenadeSliderValue = m_SmokeGrenadeSlider.value;

        // Change the other slider's max
        m_MedicPackSlider.highValue += delta;
        m_BulletSlider.highValue += delta;
        m_GasGrenadeSlider.highValue += delta;
        m_NrOfResuppliesIntegerField.value += delta;
    }

    public void GasGrenadeSliderChanged(ClickEvent clickEvent)
    {
        // Cash the new value
        int delta = m_OldGasGrenadeSliderValue - m_GasGrenadeSlider.value;
        m_OldGasGrenadeSliderValue = m_GasGrenadeSlider.value;

        // Change the other slider's max
        m_MedicPackSlider.highValue += delta;
        m_SmokeGrenadeSlider.highValue += delta;
        m_BulletSlider.highValue += delta;
        m_NrOfResuppliesIntegerField.value += delta;
    }
    #endregion
}