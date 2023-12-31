using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player instance;

    public bool active;

    public float speedMove = 5;
    [ReadOnly] public float m_speedMove;
    float y;

    public float speedRot = 100;

    [SerializeField] CharacterController charController;
    UIGameplay uiGameplay;

    [Header("Animation")]
    [SerializeField] Animator animator;
    [SerializeField] AnimatorPlayer animatorPlayer;

    [Header("Effect")]
    [SerializeField] TrailRenderer trail;
    [SerializeField] ParticleSystem particle;

    [Header("Jarak")]
    public float jarakTempuh;

    [Header("Speedometer")]
    public float maxSpeedometer;
    public float minAngleArrow;
    public float maxAngleArrow;
    public float speedKMH;

    [Header("Baterai")]
    public float maxKMBaterai;
    public float baterai;
    private void Awake()
    {
        instance = this;
    }
    private void Start()
    {
        active = true;

        uiGameplay = UIGameplay.instance;

        jarakTempuh = SaveData.instance.gameData.jarakTempuh;

        baterai = maxKMBaterai;

        SetAnimator();

        y = transform.position.y;
    }


    private void Update()
    {
        if (active)
        {
            Move();
        }

        AkselerasiSpeed();
        RemPC();

        JarakTempuh();
        BateraiUI();

        transform.position = new Vector3(transform.position.x, y, transform.position.z);
    }
    private void FixedUpdate()
    {
        SpeedometerUI();
    }

    void SetAnimator()
    {
        animatorPlayer.SetAnimator(SaveData.instance.gameData.codeSkinPlayer);
    }
    void Move()
    {
        float inputX = SimpleInput.GetAxis("Horizontal");

        Vector3 v3 = charController.transform.forward;

        charController.Move(v3 * m_speedMove / 3.6f * Time.deltaTime);
        charController.transform.Rotate(Vector3.up * inputX * speedRot * Time.deltaTime);

        //Animation
        if (inputX > 0) PlayAnimation("Right");
        else if (inputX < 0) PlayAnimation("Left");
        else PlayAnimation("Forward");

        if (tanpaTrail) return;
        //Trail
        var inputminmax07 = (inputX > 0.7f || inputX < -0.7f);
        if (inputminmax07 && speedKMH > 10 && rem 
            || speedKMH > 15 && rem 
            || inputminmax07 && speedKMH < 2)
        {
            trail.emitting = true;
            var emission = particle.emission;
            emission.rateOverTime = 40;
        }
        else 
        {
            trail.emitting = false;
            var emission = particle.emission;
            emission.rateOverTime = 0;
        }
    }
    void PlayAnimation(string value)
    {
        animator.Play(value);
    }

    bool suaraRem;
    void AkselerasiSpeed()
    {
        if (rem)
        {
            m_speedMove = Mathf.Lerp(m_speedMove, 0, 1 * Time.deltaTime);

            if (m_speedMove > 15 && !suaraRem)
            {
                suaraRem = true;
                AudioManager.instance.SetLoopSfx(AudioManager.instance.remSfx.name, true);
                Debug.Log("Rem");
            }
            else if (m_speedMove <= 15 && suaraRem)
            {
                suaraRem = false;
                AudioManager.instance.SetLoopSfx(AudioManager.instance.remSfx.name, false);
            }
        }
        else 
        {
            m_speedMove = Mathf.Lerp(m_speedMove, speedMove, 0.1f * Time.deltaTime);

            if (suaraRem)
            {
                suaraRem = false;
                AudioManager.instance.SetLoopSfx(AudioManager.instance.remSfx.name, false);
            }

        }
    }

    bool rem;
    public void Rem(bool value)
    {
        rem = value;
    }
    public void RemPC()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Rem(true);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            Rem(false);
        }
    }

    bool tanpaTrail;
    public void StartRem(float value)
    {
        StartCoroutine(Coroutine());
        IEnumerator Coroutine()
        {
            tanpaTrail = true;
            rem = true;
            yield return new WaitForSeconds(1);
            tanpaTrail = false;
            rem = false;
        }
    }

    public void SetKlakson()
    {
        AudioManager.instance.SetSFX(AudioManager.instance.klaksonSfx.name);
    }
    void SpeedometerUI()
    {
        speedKMH = charController.velocity.magnitude * 3.6f;
        uiGameplay.speedText.text = speedKMH.ToString("F0");
        uiGameplay.indikatorImage.fillAmount = speedKMH / speedMove;

        if (AchievementManager.instance.achievementUI.achievementPrefabs[1].achievementData.value < m_speedMove)
        {
            AchievementManager.instance.SetValue("Capai kecepatan 80 km/h", int.Parse(m_speedMove.ToString("F0")));
        }
    }

    void JarakTempuh()
    {
        jarakTempuh += charController.velocity.magnitude * Time.deltaTime / 1000;
        uiGameplay.jarakTempuhText.text = jarakTempuh.ToString("F1") + " km";
        SaveData.instance.gameData.jarakTempuh = jarakTempuh;
    }
    public bool bateraiCharger;
    bool bateraiLemah;
    void BateraiUI()
    {
        //Rata rata max 50km
        if (bateraiCharger)
        {
            baterai = Mathf.MoveTowards(baterai, maxKMBaterai, 0.2f * Time.deltaTime);
        }
        else
        {
            baterai -= charController.velocity.magnitude * Time.deltaTime / 1000;
        }

        if (baterai < maxKMBaterai / 8 && !bateraiLemah)
        {
            bateraiLemah = true;
            UIManager.instance.SpawnNotifText("Baterai lemah");
        }
        else if (baterai > maxKMBaterai / 8 && bateraiLemah)
        {
            bateraiLemah = false;
        }

        if (baterai <= 0)
        {
            GameplayManager.instance.SetFinish(0);
            active = false;
        }

        uiGameplay.bateraiImage.fillAmount = baterai / maxKMBaterai;
    }

}
