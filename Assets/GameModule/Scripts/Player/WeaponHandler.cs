using UnityEngine;
using StarterAssets;
using UnityEngine.Animations.Rigging;
using Unity.Cinemachine;
using System.Collections;
using TMPro;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(ThirdPersonController))]
public class WeaponHandler : MonoBehaviour
{
    [Header("References")]
    public CinemachineCamera vcam;
    private CinemachineThirdPersonFollow cm_camera;
    private Camera mainCamera;
    private Animator anim;
    private ThirdPersonController controller;

    [Header("Shooting Stats")]
    public int damage = 25;
    public float shootRange = 50f;
    public float fireRate = 0.09f;
    public LayerMask enemyLayer;
    private float lastShootTime;

    [Header("Ammo & Reloading")]
    public int magSize = 30;
    public int totalAmmo = 120;
    private int currentAmmo;
    public float reloadTime = 2f;
    private bool isReloading = false;
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("States")]
    public bool isStunned = false; //có đang bị choáng không

    [Header("Shooting Effects")]
    [SerializeField] private string shootStateName = "Fire_Rifle";
    [SerializeField] private float shootBlendTime = 0.075f;
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private AudioClip emptyClipSound;
    [SerializeField] private ParticleSystem muzzleFlash;

    [Header("Aiming - Camera (FOV & Position)")]
    public float cameraTransitionSpeed = 10f;
    [Space(5)]
    public float normalFOV = 60f;
    public float aimFOV = 45f;
    [Space(5)]
    public Vector3 normalOffset = new Vector3(0.5f, 1.6f, -3f);
    public Vector3 aimOffset = new Vector3(0.3f, 1.6f, -2.2f);
    [Space(5)]
    public float normalVerticalArmLength = 0.4f;
    public float aimVericalArmLength = 0.2f;
    public float normalCameraSide = 1f;
    public float aimCameraSide = 0.75f;
    public float normalCameraDistance = 1.5f;
    public float aimCameraDistance = 0.85f;

    [Header("Aiming - IK Rigging & Target (Fix Parallax)")]
    [SerializeField] private Rig aimRig;
    [SerializeField] private float ikTransitionSpeed = 10f;
    [Tooltip("Tạo một Empty GameObject, đặt tên AimTarget và kéo vào đây")]
    public Transform aimTarget; 
    [Tooltip("Chọn layer Default, Environment, Enemy. BỎ CHỌN layer Player!")]
    public LayerMask aimLayerMask; 

    [Header("UI Crosshair")]
    public GameObject normalCrosshair;
    public GameObject aimCrosshair;

    public bool Aiming { get; private set; }

    void Awake()
    {
        anim = GetComponent<Animator>();
        controller = GetComponent<ThirdPersonController>();
        mainCamera = Camera.main;

        if (vcam != null)
        {
            cm_camera = vcam.GetComponent<CinemachineThirdPersonFollow>();
        }
    }

    void Start()
    {
        currentAmmo = magSize;
        UpdateAmmoUI();
    }

    void LateUpdate()
    {
        if (isReloading || isStunned) return;

        Aiming = Input.GetMouseButton(1);
        bool isShooting = Input.GetMouseButton(0);

        anim.SetBool("Aiming", Aiming);
        controller._strafe = Aiming;

        if (cm_camera != null)
        {
            HandleCameraAndUI();
        }

        UpdateAimTarget();

        if (aimRig != null)
        {
            float targetIkWeight = (Aiming && !isReloading) ? 1f : 0f;
            aimRig.weight = Mathf.Lerp(aimRig.weight, targetIkWeight, ikTransitionSpeed * Time.deltaTime);
        }

        if (Input.GetKeyDown(KeyCode.R) && currentAmmo < magSize && totalAmmo > 0)
        {
            StartCoroutine(ReloadRoutine());
            return;
        }

        if (Aiming && isShooting)
        {
            TryShoot();
        }
    }

    private void HandleCameraAndUI()
    {
        float targetFOV = Aiming ? aimFOV : normalFOV;
        Vector3 targetOffset = Aiming ? aimOffset : normalOffset;
        float targetVerticalArm = Aiming ? aimVericalArmLength : normalVerticalArmLength;
        float targetSide = Aiming ? aimCameraSide : normalCameraSide;
        float targetDistance = Aiming ? aimCameraDistance : normalCameraDistance;

        vcam.Lens.FieldOfView = Mathf.Lerp(vcam.Lens.FieldOfView, targetFOV, cameraTransitionSpeed * Time.deltaTime);
        cm_camera.ShoulderOffset = Vector3.Lerp(cm_camera.ShoulderOffset, targetOffset, cameraTransitionSpeed * Time.deltaTime);
        cm_camera.VerticalArmLength = Mathf.Lerp(cm_camera.VerticalArmLength, targetVerticalArm, cameraTransitionSpeed * Time.deltaTime);
        cm_camera.CameraSide = Mathf.Lerp(cm_camera.CameraSide, targetSide, cameraTransitionSpeed * Time.deltaTime);
        cm_camera.CameraDistance = Mathf.Lerp(cm_camera.CameraDistance, targetDistance, cameraTransitionSpeed * Time.deltaTime);

        if (normalCrosshair) normalCrosshair.SetActive(!Aiming);
        if (aimCrosshair) aimCrosshair.SetActive(Aiming);
    }

    private void UpdateAimTarget()
    {
        if (aimTarget == null) return;

        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        
        if (Physics.Raycast(ray, out RaycastHit hit, shootRange * 2f, aimLayerMask))
        {
            // Trượt mục tiêu tới điểm va chạm
            aimTarget.position = Vector3.Lerp(aimTarget.position, hit.point, Time.deltaTime * 20f);
        }
        else
        {
            // Nếu không chạm gì (ví dụ ngước lên trời), đẩy mục tiêu ra thật xa
            aimTarget.position = Vector3.Lerp(aimTarget.position, ray.GetPoint(shootRange * 2f), Time.deltaTime * 20f);
        }
    }

    private void TryShoot()
    {
        if (Time.time - lastShootTime < fireRate) return;
        lastShootTime = Time.time;

        if (currentAmmo <= 0)
        {
            if (emptyClipSound) AudioSource.PlayClipAtPoint(emptyClipSound, transform.position);
            if (totalAmmo > 0) StartCoroutine(ReloadRoutine());
            return;
        }

        currentAmmo--;
        UpdateAmmoUI();

        if (shootSound) AudioSource.PlayClipAtPoint(shootSound, transform.position);
        if (muzzleFlash) muzzleFlash.Play();
        anim.CrossFadeInFixedTime(shootStateName, shootBlendTime);

        ShootRaycast();
        MakeNoise();
    }

    private IEnumerator ReloadRoutine()
    {
        isReloading = true;
        Aiming = false;
        anim.SetBool("Aiming", false);
        anim.SetTrigger("Reload");

        if (reloadSound) AudioSource.PlayClipAtPoint(reloadSound, transform.position);
        
        yield return new WaitForSeconds(reloadTime);

        int ammoNeeded = magSize - currentAmmo;
        int ammoToLoad = Mathf.Min(ammoNeeded, totalAmmo);

        currentAmmo += ammoToLoad;
        totalAmmo -= ammoToLoad;
        
        UpdateAmmoUI();
        isReloading = false;
    }

    public void AddAmmo(int amount)
    {
        totalAmmo += amount;
        UpdateAmmoUI();
        
        Debug.Log("Nhặt được " + amount + " viên đạn. Tổng đạn hiện tại: " + totalAmmo);
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            ammoText.text = ("Ammo: " + currentAmmo.ToString() + " / " + totalAmmo.ToString());
        }
    }

    private void ShootRaycast()
    {
        Vector3 screenCenter = new Vector3(Screen.width / 2f, Screen.height / 2f, 0f);
        Ray ray = mainCamera.ScreenPointToRay(screenCenter);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, shootRange, enemyLayer))
        {
            EnemyStats enemy = hit.collider.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        Debug.DrawLine(ray.origin, ray.origin + ray.direction * shootRange, Color.green, 1f);
    }

    private void MakeNoise()
    {
        NoiseManager.Instance?.MakeNoise(transform.position, 40f);
        ZombieWorldManager world = FindAnyObjectByType<ZombieWorldManager>();
        if (world != null)
        {
            world.AddNoise(3f);
        }
    }

    public void StaggerWeapon(float staggerDuration)
    {
        StartCoroutine(StaggerRoutine(staggerDuration));
    }

    private System.Collections.IEnumerator StaggerRoutine(float duration)
    {
        isStunned = true;
        yield return new WaitForSeconds(duration);
        isStunned = false;
    }
}