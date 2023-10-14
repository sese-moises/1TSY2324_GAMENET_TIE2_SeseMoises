using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class PlayerMovementController : MonoBehaviour
{
    public Joystick _joystick;
    public FixedTouchField _fixedTouchField;

    private RigidbodyFirstPersonController _rigidBodyFirstPersonController;

    private Animator _animator;

    void Start()
    {
        _rigidBodyFirstPersonController = this.GetComponent<RigidbodyFirstPersonController>();
        _animator = this.GetComponent<Animator>();
    }
    

    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        _rigidBodyFirstPersonController._joystickInputAxis.x = _joystick.Horizontal;
        _rigidBodyFirstPersonController._joystickInputAxis.y = _joystick.Vertical;
        _rigidBodyFirstPersonController.mouseLook._lookInputAxis = _fixedTouchField.TouchDist;

        _animator.SetFloat("horizontal", _joystick.Horizontal);
        _animator.SetFloat("vertical", _joystick.Vertical);

        if (Mathf.Abs(_joystick.Horizontal) > 0.9 || Mathf.Abs(_joystick.Vertical) > 0.9)
        {
            _animator.SetBool("isRunning", true);
            _rigidBodyFirstPersonController.movementSettings.ForwardSpeed = 10;
        }
        else
        {
            _animator.SetBool("isRunning", false);
            _rigidBodyFirstPersonController.movementSettings.ForwardSpeed = 5;
        }
    }
}
