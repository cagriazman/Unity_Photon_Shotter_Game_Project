using System;
using UnityEngine;
using Photon.Pun;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviourPun
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 6.0f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;
        [Tooltip("control runing")]
        public bool isCanRun;
        private float groundCheckTimer = 0f;
        private bool groundChecking = true;
        [SerializeField]private float groundCheckTimerLimit=0.02f;
        
        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;
        
        [Header("Crouch Settings")]
        [Tooltip("Eğilme anındaki yürüme hızı")]
        public float CrouchSpeed = 2.0f;
        [Tooltip("Karakterin normal boyu (CharacterController yüksekliği)")]
        public float NormalHeight = 1.7f;
        [Tooltip("Eğilme anındaki boyu")]
        public float CrouchHeight = .85f;
        [Tooltip("Kamera hedefinin normal yerel Y pozisyonu")]
        public float NormalCameraY = 1.55f; 
        [Tooltip("Eğilme anındaki kamera yerel Y pozisyonu")]
        public float CrouchCameraY = 0.775f;
        [Tooltip("Eğilme ve kalkma geçiş hızı")]
        public float CrouchSpeedRate = 10f;



        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        [Header("Custom Animators & Sync")]
        public Animator ak47anim;
        public AnimasyonSenkronizasyon _animasyonSenkronizasyon;
        public float animSmoothTime = 0.1f;
        private Vector2 currentAnimInput;
        private Vector2 animVelocity;
        private float multiplierForAnim;
        public Transform topOftheHead;
        // --------------------------------------------------------

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;

        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        //turnanim
        private float _referansOrigin = 0;
        private float _zamanlayici = 0;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;


#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.0f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
            }
        }

        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif

            // --- EKLEME: Component alımı ---
            if (_animasyonSenkronizasyon == null)
                _animasyonSenkronizasyon = GetComponent<AnimasyonSenkronizasyon>();
            // -------------------------------

            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            //if(ChatYonetim.chatAcikMi)return;
            JumpAndGravity();
            GroundedCheck();
            Move();
            HandleCustomAnimations();
            HandleCrouch();
        }

        private void LateUpdate()
        {
            //if(ChatYonetim.chatAcikMi)return;
            CameraRotation();
        }

        // --- 5. YENİ METOT: Animasyon İşlemleri ---
        private void HandleCustomAnimations()
        {
            // A) Animasyon Yumuşatma ve Senkronizasyon
             
            Vector2 targetAnimInput = _input.move * multiplierForAnim;

            // Movement.cs'deki SmoothDamp mantığı
            currentAnimInput = Vector2.SmoothDamp(currentAnimInput, targetAnimInput, ref animVelocity, animSmoothTime);

            if (_animasyonSenkronizasyon != null)
            {
                _animasyonSenkronizasyon.Horizontal = currentAnimInput.x;
                _animasyonSenkronizasyon.Vertical = currentAnimInput.y;
            }

            // B) Silah Animasyonu (Speed parametresi)
            if (ak47anim != null)
            {
                float animSpeed = _input.move.magnitude > 0 ? 1f : 0f;
                ak47anim.SetFloat("speed", animSpeed, 0.15f, Time.deltaTime);
            }
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {

            //Don't multiply mouse input by Time.deltaTime
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            //_rotationVelocity2 += _input.look.x * RotationSpeed * deltaTimeMultiplier;


            // 1. Zamanlayıcıyı sürekli artır
            _zamanlayici += Time.deltaTime;

            // 2. Anlık farkı hesapla (Origin hala eski yerinde bekliyor)
            float anlikAciFarki = Mathf.DeltaAngle(_referansOrigin, CinemachineCameraTarget.transform.eulerAngles.y);

            // 3. Değeri gönder (Burada 100 ile çarpman hassasiyeti artırır, güzel)

            // 4. BEKLEME SÜRESİ KONTROLÜ (0.5 saniye doldu mu?)
            if (_zamanlayici >= 1f)
            {
                //_animasyonSenkronizasyon.HorizontalForMouse = anlikAciFarki;
                //Debug.Log("Giden Fark: " + anlikAciFarki + " | Yeni Origin: " + _referansOrigin);

                // 0.5 saniye dolunca nihayet origin'i kameranın şu anki açısına çekiyoruz
                _referansOrigin = CinemachineCameraTarget.transform.eulerAngles.y;

                // Zamanlayıcıyı sıfırla ki yeni bir 0.5 saniye başlasın
                _zamanlayici = 0f;
            }
            _animasyonSenkronizasyon.HorizontalForMouse = Mathf.Lerp(_animasyonSenkronizasyon.HorizontalForMouse, 2 * anlikAciFarki, Time.deltaTime * 3f);


            _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
            _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;


            // Aradaki farkı bul (-180 ile 180 arası döner)
            // float angleDifference = Mathf.DeltaAngle(_rotationVelocity, _rotationVelocity2);
            // Debug.Log("_rotationVelocity=" + _rotationVelocity
            // + " _rotationVelocity2=" + _rotationVelocity2
            // + " angleDifference=" + angleDifference);
            // clamp our pitch rotation
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            // gövdeyi sağ el mantığıyla yürütüz
            transform.Rotate(Vector3.up * _rotationVelocity);


            //_animasyonSenkronizasyon.HorizontalForMouse = angleDifference;

        }

        private void Move()
        {
            bool isForward = _input.move.y > 0.1f && Mathf.Abs(_input.move.x) < 0.1f;
            
            
            if (groundChecking != _controller.isGrounded)
            {
                groundCheckTimer += Time.deltaTime;
                if (groundCheckTimer > groundCheckTimerLimit)
                {
                    groundChecking = _controller.isGrounded;
                }
            }
            else
            {
                groundCheckTimer = 0f;
            }
            isCanRun = _input.sprint && isForward && groundChecking && !_input.getAim;
            multiplierForAnim = isCanRun ? 2f : 1f;
            ak47anim.SetBool("isRunHeld", isCanRun);

            float targetSpeed;
            
            // set target speed based on move speed, sprint speed and if sprint is pressed
            //targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            
            if (isCanRun)
            {
                targetSpeed=SprintSpeed;

            }
            
            else if(_input.crouch)
                targetSpeed=CrouchSpeed;
            
            else if (!isForward && _input.move != Vector2.zero)
                targetSpeed = MoveSpeed * 0.7f;

            else
                targetSpeed = MoveSpeed;
            

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            // normalise input direction
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (_input.move != Vector2.zero)
            {
                // move
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            // move the player
            _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) 
                             + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            
            Debug.Log("Target speed: "+targetSpeed);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);


                    photonView.RPC("RPC_ZiplaAnim", RpcTarget.Others);
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }

                // if we are not grounded, do not jump
                _input.jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }
        private void HandleCrouch()
        {
            

            if (!headCollisionInCrouch())
            {
                // Tuşa basılmasına göre hedef boyumuzu ve hedef kamera Y pozisyonumuzu seçiyoruz
                float targetHeight = _input.crouch ? CrouchHeight : NormalHeight;
                float targetCameraY = _input.crouch ? CrouchCameraY : NormalCameraY;

                // 1. Karakter Yüksekliğini Yumuşakça Değiştir (Lerp ile)
                _controller.height = Mathf.Lerp(_controller.height, targetHeight, Time.deltaTime * CrouchSpeedRate);

                // 2. CharacterController boyu kısalırken ayakların havada kalmaması için Merkez (Center) noktasını kaydırıyoruz
                // Bu sayede karakter havadan aşağı çökmek yerine, ayakları yerde sabit kalarak kafasını aşağı eğer!
                Vector3 targetCenter = new Vector3(_controller.center.x, _controller.height / 2f, _controller.center.z);

                _controller.center = targetCenter;
                
                //Vector3.Lerp(_controller.center, targetCenter, Time.deltaTime * CrouchSpeedRate);

                // 3. Kamerayı Yumuşakça Aşağı/Yukarı Kaydırıyoruz
                Vector3 camTargetPos = CinemachineCameraTarget.transform.localPosition;
                camTargetPos.y = Mathf.Lerp(camTargetPos.y, targetCameraY, Time.deltaTime * CrouchSpeedRate);
                CinemachineCameraTarget.transform.localPosition = camTargetPos;
            
                photonView.RPC("RPC_Crouch", RpcTarget.Others,_input.crouch);
            }
            
            
        }

        private bool headCollisionInCrouch()
        {
            return Physics.CheckSphere(topOftheHead.position+Vector3.up*0.5f, 0.5f);
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
            
            
            // Bu sayede karakteri hiyerarşide seçtiğinde kafasının üstündeki yeşil kontrol küresini görebileceksin
            Gizmos.color = Color.cyan;
           
            Gizmos.DrawWireSphere(topOftheHead.position+Vector3.up*GroundedRadius, GroundedRadius);
        }


    }
}