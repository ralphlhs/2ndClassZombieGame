using System;
using UnityEngine;
using UnityEngine.UI;

public class PlayerManager : MonoBehaviour
{
    private float moveSpeed = 5.0f;//플레이어 이동 속도
    public float mouseSensitivity = 100.0f; //마무스감도
    public Transform cameraTransform;//카메라의 Transform
    public CharacterController characterController;
    public Transform playerHead;
    //플레이어 머리 위치 (1인칭 모드를 위해)
    public float thirdPersonDistance = 3.0f;
    //3칭 모드에서 플레이어와 카메라의 거리
    public Vector3 thirdPersonOffset = new Vector3(0f, 1.5f, 0f);
    //3인칭 모드에서 카메라 오프
    public Transform playerLookObj;//플레이어 시야 위치
    public float zoomDistance = 1.0f;//카메라가 확대될때의 거리(3인칭 모드에서 사용)
    public float zoomSpeed = 5.0f;//확대축소가 되는 속도
    public float defaultFov = 60.0f;//기본 카메라 시야각
    public float zoomFov = 30.0f;//확대 시 카메라 시야각(1인칭 모드에서 사용)

    //====================================================
    private float currentDistance;//현재 카메라와의 거리(3인칭 모드)
    private float targetDistance;//목표 카메라 거리
    private float targetFov;//목표 FOV
    private bool isZoomed = false;//확대 여부 확인
    private Coroutine zoomCoroutine;//코루틴을 사용한  확대 축소 처리
    private Camera mainCamera; //카메라 컴포넌트

    private float pitch = 0.0f;//위아래 회전값
    private float yaw = 0.0f;//좌우 회전 값
    private bool isFirstPerson = false;///1인칭 모드 여부
    private bool isRotaterAroundPlayer = true;//카메라가 플레이어 주위를 회전하는지 여부

    //중력 관련 변수
    public float gravity = -9.81f;
    public float jumpHeight = 2.0f;
    private Vector3 velocity;
    private bool isGround;

    private Animator animator;
    private float horizontal;
    private float vertical;
    private bool isRunning = false;

    private bool isFire = false;
    private bool isDying = false;

    public AudioClip audioClipFire;
    public AudioClip audioClipItemGet;
    private AudioSource audioSource;
    public AudioClip audioClipWeaponChange;
    public GameObject RifleM4Obj;

    public float walkSpeed = 5.0f;
    public float runSpeed = 10.0f;
    private float currentSpeed = 1.0f;
    private int animationSpeed = 1;
    private string currentAnimation = "Idle";

    public Transform aimTarget;
    private float weaponMaxDistance = 100.0f;
    public GameObject cube;
    private int cubeHP = 10;
    public GameObject cube1;
    private int cube1HP = 10;
    RaycastHit[] hits;

    public Text text;
    public Text text1;
    public GameObject text2;

    public LayerMask TargetLayerMask;
    //public MultiAimConstraint multiAimConstraint;

    public Vector3 boxSize = new Vector3(1.0f, 1.0f, 1.0f);
    public float castDistance = 5.0f;
    public LayerMask itemLayer;
    public Transform itemGetPos;
    public GameObject crosshairObj;
    public GameObject m4IconImage;
    GameObject zombie;
    //ZombieManager zombieManager;

    bool isGetM4Item = false;
    bool isUseWeapon = false;

    public ParticleSystem m4Effect;

    private float rifleFireDelay = 0.5f;
    public ParticleSystem DamageParticleSystem;
    public AudioClip audioClipDamage;
    public AudioClip audioClipFlashOn;

    public Text bulletText;
    private int firebulletCount = 30;
    private int savebulletCount = 0;
    public GameObject flashLightObj;
    private bool isFlashLightOn = false;

    public int playerHP = 100;
    public Text playerHPtext;

    public GameObject PauseObj;
    private bool isPause = false;

    private WeaponMode currentWeaponMode = WeaponMode.Rifle;
    private int ShotgunRayCount = 5;
    private float shotGunSpredAngle = 10.0f;
    private float recoilStrength = 2.0f;
    private float maxRecoilAngle = 10.0f;
    private float currentRecoil = 0.0f;
    private float shakeDuration = 0.1f;
    private float shakeMagnitude = 0.1f;
    private Vector3 originalCameraPosition;
    private Coroutine cameraShakeCoroutin;
    public GameObject door;
    private bool isDoor = true;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        currentDistance = thirdPersonDistance;
        targetDistance = thirdPersonDistance;
        targetFov = defaultFov;
        mainCamera = cameraTransform.GetComponent<Camera>();
        mainCamera.fieldOfView = defaultFov;
        animator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        RifleM4Obj.SetActive(false);
        //crosshairObj.SetActive(false);
        //m4IconImage.SetActive(false);
    }

    void Update()
    {
        MouseSet();
        CameraSet();
        PlayerMovement();
        Run();
        AnimationSet();
        //AimSet();


    }

    private void MouseSet()
    {
        //마우스 입력을 받아 카마라와 플레이어 회전 처리
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        yaw += mouseX;
        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, -45.0f, 45.0f);

        isGround = characterController.isGrounded;

        if(isGround && velocity.y < 0)
        {
            velocity.y = -2.0f;
        }
    }

    private void CameraSet()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            isRotaterAroundPlayer = !isRotaterAroundPlayer;
        }
    }

    //===============================================================================

    void AnimationSet()
    {
        animator.SetFloat("Horizontal", horizontal);
        animator.SetFloat("Vertical", vertical);
        animator.SetBool("IsRun", isRunning);
    }

    void Run()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            isRunning = true;
        }
        else
        {
            isRunning = false;
        }
        moveSpeed = isRunning ? runSpeed : walkSpeed;
    }

    void PlayerMovement()
    {
        if (isFirstPerson)
        {
            FirstPersonMovement();
        }
        else
        {
            ThirdPersonMovement();
        }
    }

    void FirstPersonMovement()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        Vector3 moveDirection = cameraTransform.forward * vertical + cameraTransform.right * horizontal;
        moveDirection.y = 0;
        characterController.Move(moveDirection * moveSpeed * Time.deltaTime);
        cameraTransform.position = playerHead.position;
        cameraTransform.rotation = Quaternion.Euler(pitch, yaw, 0);
        transform.rotation = Quaternion.Euler(0f, cameraTransform.eulerAngles.y, 0);
    }

    void ThirdPersonMovement()
    {
        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");
        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        UpdateCameraPosition();
    }

    private void UpdateCameraPosition()
    {
        if (isRotaterAroundPlayer)
        {
            //카메라가 플레이어 오른쪽에서 회전하도록 설정
            Vector3 direction = new Vector3(0, 0, -currentDistance);
            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0);

            //카메라를 플레이어의 오른쪽에서 고정된 위치로 이동
            cameraTransform.position = transform.position + thirdPersonOffset + rotation * direction;

            //카메라가 플레이어의 위치를 따라가도록 설정
            cameraTransform.LookAt(transform.position + new Vector3(0, thirdPersonOffset.y, 0));

        }
        else
        {
            //플레이어가 직접 회전하는 모드
            transform.rotation = Quaternion.Euler(0f, yaw, 0);
            Vector3 direction = new Vector3(0, 0, -currentDistance);
            cameraTransform.position = playerLookObj.position + thirdPersonOffset + Quaternion.Euler(pitch, yaw, 0) * direction;
            cameraTransform.LookAt(playerLookObj.position + new Vector3(0, thirdPersonOffset.y, 0));

            UpdateAimTarget();
        }
    }

    private void UpdateAimTarget()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        aimTarget.position = ray.GetPoint(10.0f);
    }

    public void SetTargetDistance(float distance)
    {
        targetDistance = distance;
    }

    public void SetTargetFOV(float fov)
    {
        targetFov = fov;
    }

    //========================================================================================

    public void ItemBoxCast()
    {
        Vector3 origin = itemGetPos.position;
        Vector3 direction = itemGetPos.forward;
        RaycastHit[] hits;
        hits = Physics.BoxCastAll(origin, boxSize, direction, Quaternion.identity, castDistance, itemLayer);
        Debug.Log("박스 캐스팅 테스트 : " + hits);
        DebugBox(origin, direction);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider.name == "ItemM4")
            {
                hit.collider.gameObject.SetActive(false);
                SfxSound("FootStep", transform.position, 1.0f);
                Debug.Log("Item : " + hit.collider.name);
                m4IconImage.SetActive(true);
                isGetM4Item = true;
                bulletText.gameObject.SetActive(true);
            }
            if (hit.collider.tag == "Item")
            {
                hit.collider.gameObject.SetActive(false);
                SfxSound("FootStep", transform.position, 1.0f);

                savebulletCount += 30;
                if (savebulletCount > 120)
                {
                    savebulletCount = 120;
                }
                bulletText.text = $"{firebulletCount}/{savebulletCount}";
                bulletText.gameObject.SetActive(true);
            }
        }
    }


    void DebugBox(Vector3 origin, Vector3 direction)
    {
        Vector3 endPoint = origin + direction * castDistance;

        Vector3[] corners = new Vector3[8];
        corners[0] = origin + new Vector3(-boxSize.x, -boxSize.y, -boxSize.z) / 2;
        corners[1] = origin + new Vector3(boxSize.x, -boxSize.y, -boxSize.z) / 2;
        corners[2] = origin + new Vector3(-boxSize.x, boxSize.y, -boxSize.z) / 2;
        corners[3] = origin + new Vector3(boxSize.x, boxSize.y, -boxSize.z) / 2;
        corners[4] = origin + new Vector3(-boxSize.x, -boxSize.y, boxSize.z) / 2;
        corners[5] = origin + new Vector3(boxSize.x, -boxSize.y, boxSize.z) / 2;
        corners[6] = origin + new Vector3(-boxSize.x, boxSize.y, boxSize.z) / 2;
        corners[7] = origin + new Vector3(boxSize.x, boxSize.y, boxSize.z) / 2;

        Debug.DrawLine(corners[0], corners[1], Color.green, 3.0f);
        Debug.DrawLine(corners[1], corners[3], Color.green, 3.0f);
        Debug.DrawLine(corners[3], corners[2], Color.green, 3.0f);
        Debug.DrawLine(corners[2], corners[0], Color.green, 3.0f);
        Debug.DrawLine(corners[4], corners[5], Color.green, 3.0f);
        Debug.DrawLine(corners[5], corners[7], Color.green, 3.0f);
        Debug.DrawLine(corners[7], corners[6], Color.green, 3.0f);
        Debug.DrawLine(corners[6], corners[4], Color.green, 3.0f);
        Debug.DrawLine(corners[0], corners[4], Color.green, 3.0f);
        Debug.DrawLine(corners[1], corners[5], Color.green, 3.0f);
        Debug.DrawLine(corners[2], corners[6], Color.green, 3.0f);
        Debug.DrawLine(corners[3], corners[7], Color.green, 3.0f);
        Debug.DrawRay(origin, direction * castDistance, Color.green);
    }

    private void SfxSound(string soundname, Vector3 pos, float spatial)
    {
        SoundController.Instance.PlaySfx(soundname, pos, spatial);
    }
}

public enum WeaponMode
{
    Rifle,
    ShotGun,
}