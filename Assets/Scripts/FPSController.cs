using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(CharacterController))] //this is a great thing to show people as it shows them how to make sure components will set up on objects.
public class FPSController : MonoBehaviour
{
    private float _xRotation;
    private Vector3 _moveVector;
    private CharacterController _controller;
    [SerializeField] private float mouseSensitivity = 200f;
    [SerializeField] private float speed = 5f;
    [SerializeField] private Camera camera;
    [SerializeField] private float xCameraBounds = 60f;
    
    #region Smoothing code
    private Vector2 _currentMouseDelta;
    private Vector2 _currentMouseVelocity;
    [SerializeField] private float smoothTime = .1f;
    
    #endregion
    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Awake()
    {
        _controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        Movement();
        Rotation();
    }

    private void Movement()
    {
        //_moveVector = new Vector3(Input.GetAxis("Horizontal"),0, Input.GetAxis("Vertical")) * speed * Time.deltaTime;//initial way of showing movement
        _moveVector = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal"); //easier to explain after by using the forward and right vectors
        _moveVector.Normalize();
        _controller.SimpleMove(_moveVector * speed );
    }

    private void Rotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        Vector2 targetDelta = new Vector2(mouseX, mouseY);
        _currentMouseDelta = Vector2.SmoothDamp(_currentMouseDelta, targetDelta, ref _currentMouseVelocity, smoothTime);
        _xRotation -= _currentMouseDelta.y;
        _xRotation = Mathf.Clamp(_xRotation, -xCameraBounds, xCameraBounds);
        transform.Rotate(Vector3.up * _currentMouseDelta.x);
        camera.transform.localRotation = Quaternion.Euler(_xRotation, 0f, 0f);
    }

    private void LateUpdate()
    {

        
    }
}
