using System;
using Camera;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.InputSystem;
using Zenject;

namespace Anglerfish
{
    public class AnglerfishController : MonoBehaviour
    {
        [Header("Movement")] [SerializeField] float movementSpeed;
        [SerializeField] float lerpFastConst;
        [SerializeField] float lerpSlowConst;

        [Header("Dash")] [SerializeField] float dashSpeed;
        [SerializeField] float dashDuration;
        [SerializeField] float dashCooldown;

        [Header("Light")] [SerializeField] float maxLightEnergy;
        [SerializeField] float lightEnergyPerFish;
        [SerializeField] float lightEnergyPerSecond;
        [SerializeField] float defaultLightRadius;
        [SerializeField] float turnOnLightRadius;
        [SerializeField] Light2D light2d;
        [SerializeField] float defaultLightEffectOpacity;
        [SerializeField] float turnOnLightEffectOpacity;
        [SerializeField] SpriteRenderer lightEffect;

        [Header("Areas")] [SerializeField] float eatingAreaRadius;
        [SerializeField] float lureAreaRadius;
        [SerializeField] float spawnAreaRadius;

        [Header("Animator")] [SerializeField] Animator animator;

        [Inject] CameraController _cameraController;

        InputActions _inputActions;
        Rigidbody2D _rigidbody;
        Vector2 _movement;
        
        static readonly int Dash = Animator.StringToHash("Dash");
        static readonly int Eat = Animator.StringToHash("Eat");

        bool _isDashing;
        float _dashRemainingCooldown;

        bool _lightOn;
        float _lightEnergy;

        public float EatingAreaRadius => eatingAreaRadius;
        public float LureAreaRadius => lureAreaRadius;
        public float SpawnAreaRadius => spawnAreaRadius;

        public float DashRemainingCooldown => _dashRemainingCooldown;
        public float DashCooldown => dashCooldown;

        public float LightEnergy => _lightEnergy;
        public float MaxLightEnergy => maxLightEnergy;

        public event Action OnFishEaten;
        public event Action OnLightOn;
        public event Action OnLightOff;

#if UNITY_EDITOR
        void OnDrawGizmos()
        {
            var position = transform.position;

            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(position, spawnAreaRadius);

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(position, lureAreaRadius);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(position, eatingAreaRadius);
        }
#endif
        public void FishEaten()
        {
            OnFishEaten?.Invoke();
            _dashRemainingCooldown = 0;
            _lightEnergy = Mathf.Min(maxLightEnergy, _lightEnergy + lightEnergyPerFish);
            
            animator.SetTrigger(Eat);
        }

        void Start()
        {
            _rigidbody = GetComponent<Rigidbody2D>();

            _inputActions = new InputActions();
            _inputActions.Enable();
            _inputActions.Player.Move.performed += PerformMove;
            _inputActions.Player.Move.canceled += PerformMove;
            _inputActions.Player.Dash.performed += PerformDash;
            _inputActions.Player.Light.performed += PerformLightOn;
            _inputActions.Player.Light.canceled += PerformLightOff;

            _lightEnergy = maxLightEnergy;
            _lightOn = false;
        }

        void OnDestroy()
        {
            _inputActions.Player.Move.performed -= PerformMove;
            _inputActions.Player.Move.canceled -= PerformMove;
            _inputActions.Player.Dash.performed -= PerformDash;
            _inputActions.Player.Light.performed -= PerformLightOn;
            _inputActions.Player.Light.canceled -= PerformLightOff;
        }

        void Update()
        {
            _dashRemainingCooldown = Mathf.Max(_dashRemainingCooldown - Time.deltaTime, 0);

            if (_lightOn)
            {
                _lightEnergy = Mathf.Max(0, _lightEnergy - lightEnergyPerSecond * Time.deltaTime);
            }

            if (_lightEnergy <= 0)
            {
                LightOff();
            }

            var targetRadius = _lightOn ? turnOnLightRadius : defaultLightRadius;
            light2d.pointLightOuterRadius = Mathf.Lerp(light2d.pointLightOuterRadius, targetRadius, 4 * Time.deltaTime);

            var targetAlpha = _lightOn ? turnOnLightEffectOpacity : defaultLightEffectOpacity;
            var color = lightEffect.color;
            color.a = Mathf.Lerp(color.a, targetAlpha, 4 * Time.deltaTime);;
            lightEffect.color = color;
        }

        void FixedUpdate()
        {
            if (_isDashing) return;

            var x = LerpMove(_rigidbody.velocity.x, _movement.x);
            var y = LerpMove(_rigidbody.velocity.y, _movement.y);

            _rigidbody.velocity = new Vector2(x, y);
        }

        void PerformDash(InputAction.CallbackContext ctx)
        {
            if (_dashRemainingCooldown > 0) return;

            var worldPos = _cameraController.Camera.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            var direction = ((Vector2) (worldPos - transform.position)).normalized;

            _rigidbody.velocity = direction * dashSpeed;
            _isDashing = true;
            _dashRemainingCooldown = dashCooldown;
            
            animator.SetTrigger(Dash);

            Invoke(nameof(StopDash), dashDuration);
        }

        void PerformLightOn(InputAction.CallbackContext ctx)
        {
            if (_lightEnergy > 0)
            {
                _lightOn = true;
                OnLightOn?.Invoke();
            }
        }

        void PerformLightOff(InputAction.CallbackContext ctx)
        {
            LightOff();
        }

        void PerformMove(InputAction.CallbackContext ctx)
        {
            _movement = ctx.ReadValue<Vector2>() * movementSpeed;
        }

        void StopDash()
        {
            _rigidbody.velocity = _rigidbody.velocity.normalized * movementSpeed;
            _isDashing = false;
        }

        void LightOff()
        {
            _lightOn = false;
            OnLightOff?.Invoke();
        }

        float LerpMove(float oldMove, float newMove)
        {
            return Mathf.Approximately(newMove, 0)
                ? LerpSlow(oldMove, newMove)
                : LerpFast(oldMove, newMove);
        }

        float LerpFast(float a, float b)
        {
            return Mathf.Lerp(a, b, lerpFastConst);
        }

        float LerpSlow(float a, float b)
        {
            return Mathf.Lerp(a, b, lerpSlowConst);
        }
    }
}