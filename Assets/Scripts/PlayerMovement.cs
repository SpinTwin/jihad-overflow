using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
	public CharacterController controller;

	public float speed;
	public float gravity;
	public float jumpHeight;

	public Transform groundCheck;
	public float groundDistance;
	public LayerMask groundMask;

	private float x;
	private float z;

	private Vector3 move;
	private Vector3 velocity;
	private bool isGrounded;

	void Start()
	{

	}

	void Update()
	{
		isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

		if (isGrounded && velocity.y < 0)
		{
			velocity.y = -2f;
		}

		x = Input.GetAxis("Horizontal");
		z = Input.GetAxis("Vertical");

		move = transform.right * x + transform.forward * z;

		controller.Move(move * speed * Time.deltaTime);

		if (Input.GetButtonDown("Jump") && isGrounded)
		{
			velocity.y = Mathf.Sqrt(jumpHeight * 2 * gravity);
		}

		velocity.y -= gravity * Time.deltaTime;

		controller.Move(velocity * Time.deltaTime);
	}
}
