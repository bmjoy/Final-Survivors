using Final_Survivors.Core;
using UnityEngine;

namespace Final_Survivors.Player
{
    public class Idle : _PlayerMovement
    {
        Vector3 mousePosition;

        public Idle(_PlayerMovementSM playerMovementSM) : base(playerMovementSM)
        {
        }

        public override void OnEnter()
        {
            InitSubStates();
            _playerMovementSM.Animator.SetBool("isIdle", true);

            if (true/*Weapon == pistol*/) // TODO REPLACE WITH WEAPON DETECTION
            {
                _playerMovementSM.Animator.SetBool("isOneHandWeapon", true);
            }
            // else
            // {
            //     _playerMovementSM.Animator.SetBool("isTwoHandWeapon", true);
            // }
        }

        public override void OnUpdate()
        {
            if (_playerMovementSM.PController.ActionTriggers[Events.DASH])
            {
                _playerMovementSM.ChangeState(new Dash(_playerMovementSM));
            }
            else if (_playerMovementSM.PController.ActionTriggers[Events.MOVE])
            {
                _playerMovementSM.ChangeState(new Move(_playerMovementSM));
            }
            else
            {
                if (_playerMovementSM.PController.ActionTriggers[Events.TBAG])
                {
                    _playerMovementSM.ChangeState(new Tbag(_playerMovementSM));
                }
                else
                {
                    if (mousePosition != Input.mousePosition)
                    {
                        _playerMovementSM.Animator.SetBool("isRotate", true);
                        _playerMovementSM.playerMovement.RotateToMouse();
                    }
                    else
                    {
                        _playerMovementSM.Animator.SetBool("isRotate", false);
                    }

                    mousePosition = Input.mousePosition;
                }
            }
        }

        public override void OnExit()
        {
            InitSubStates();
            _playerMovementSM.Animator.SetBool("isIdle", false);
        }

        private void InitSubStates()
        {
            _playerMovementSM.Animator.SetBool("isOneHandWeapon", false);
            _playerMovementSM.Animator.SetBool("isTwoHandWeapon", false);
            _playerMovementSM.Animator.SetBool("isRotate", false);
        }
    }
}
