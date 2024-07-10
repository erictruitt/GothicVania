using System;
using TrixieGames.Input;
using UnityEngine;
using UnityEngine.InputSystem;
using static TrixieGames.Input.PlayerInputActions;

namespace TrixieGames.Managers
{
    [CreateAssetMenu(menuName = "InputManager")]
    [DefaultExecutionOrder(-100)]
    public class InputManager : ScriptableObject, IPlayerActions
    {
        private PlayerInputActions playerControls;
        public PlayerInputActions PlayerControls { get { return playerControls; } }

        public event Action m_AttackEvent;
        public event Action<float> m_MoveEvent;
        public event Action m_JumpStartEvent;
        public event Action m_JumpEndEvent;
        public event Action m_PauseEvent;

        private void OnEnable()
        {
            if (playerControls == null)
            {
                playerControls = new PlayerInputActions();
                playerControls.Player.SetCallbacks(instance: this);
                playerControls.Player.Enable();
            }
        }

        public void OnAttack(InputAction.CallbackContext _context)
        {
            m_AttackEvent?.Invoke();
        }

        public void OnMovement(InputAction.CallbackContext _context)
        {
            m_MoveEvent?.Invoke(_context.ReadValue<float>());
        }

        public void OnJump(InputAction.CallbackContext _context)
        {
            switch (_context.phase)
            {
                case InputActionPhase.Performed:
                    m_JumpStartEvent?.Invoke();
                    break;
                case InputActionPhase.Canceled:
                    m_JumpEndEvent?.Invoke();
                    break;
            }
        }

        public void OnPause(InputAction.CallbackContext _context)
        {
            m_PauseEvent?.Invoke();
        }
    }
}
