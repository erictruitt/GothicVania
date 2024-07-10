using TrixieGames.Managers;
using UnityEngine;

namespace TrixieGames.Gameplay.Player
{
    [DefaultExecutionOrder(-100)]
    public class PlayerInputController : MonoBehaviour
    {
        [SerializeField]
        private InputManager m_InputManager;

        private float horizontal;
        public float Horizontal { get { return horizontal; } }

        private bool moveHeld;
        public bool MoveHeld { get { return moveHeld; } }

        private bool jumpHeld;
        public bool JumpHeld { get { return jumpHeld; } }

        private bool jumpPressed;
        public bool JumpPressed { get { return jumpPressed; } }

        private bool crouchHeld;
        public bool CrouchHeld { get { return crouchHeld; } }

        private bool crouchPressed;
        public bool CrouchPressed { get { return crouchPressed; } }


        private void Awake()
        {
            m_InputManager.m_AttackEvent += HandleAttackEvent;
            m_InputManager.m_MoveEvent += HandleMoveEvent;
            m_InputManager.m_JumpStartEvent += HandleJumpStartEvent;
            m_InputManager.m_JumpEndEvent += HandleJumpEndEvent;
            m_InputManager.m_PauseEvent += HandlePauseEvent;
        }

        private void HandlePauseEvent()
        {
            Debug.LogError("TODO: Implement PlayerInput.HandlePauseEvent()");
        }

        private void HandleJumpEndEvent()
        {
            jumpPressed = false;
            jumpHeld = false;
        }

        private void HandleJumpStartEvent()
        {
            jumpPressed = true;
        }

        private void HandleMoveEvent(float _direction)
        {
            Debug.Log("Direction " + _direction);
            horizontal = _direction;
            horizontal = Mathf.Clamp(horizontal, -1f, 1f);
        }

        private void HandleAttackEvent()
        {
            Debug.LogError("TODO: Implement PlayerInput.HandleAttackEvent()");
        }
        private void HandleCrouchStartEvent()
        {
            crouchPressed = true;
        }

        private void HandleCrouchEndEvent()
        {
            crouchPressed = false;
        }

        void Update()
        {
            CheckSpecialInput();

            if (GameManager.IsGameOver())
                return;
        }

        void CheckSpecialInput()
        {
            if (jumpPressed == true)
                jumpHeld = m_InputManager.PlayerControls.Player.Jump.IsPressed();

            moveHeld = m_InputManager.PlayerControls.Player.Movement.IsPressed();
        }
    }
}
