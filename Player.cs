using Godot;
using System;

public partial class Player : CharacterBody3D
{
	// POV of the player character
	public Node3D Neck;
	public Camera3D Camera;
	
	// Grab Mechanic
	public RayCast3D Interaction;
	public Marker3D Hand;
	
	public const float Speed = 5.0f;
	public const float JumpVelocity = 4.5f;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/3d/default_gravity").AsSingle();

	public bool PickedObject;
	public int PullPower = 4;
	
	// Replacement of GDScript's onready
	public override void _Ready()
	{
	Neck = GetNode<Node3D>("Neck");
	Camera = GetNode<Camera3D>("Neck/Camera3D");
	Interaction = GetNode<RayCast3D>("Neck/Camera3D/Interaction");
	Hand = GetNode<Marker3D>("Neck/Camera3D/Hand");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector3 velocity = Velocity;

		// Add the gravity.
		if (!IsOnFloor())
			velocity.Y -= gravity * (float)delta;

		// Handle Jump.
		if (Input.IsActionJustPressed("ui_accept") && IsOnFloor())
			velocity.Y = JumpVelocity;

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 inputDir = Input.GetVector("left", "right", "forward", "back");
		Vector3 direction = (Neck.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();
		if (direction != Vector3.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Z = direction.Z * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Z = Mathf.MoveToward(Velocity.Z, 0, Speed);
		}

		Velocity = velocity;
		MoveAndSlide();
	}

	public override void _UnhandledInput(InputEvent @event)
	{
		//Camera Movement and Escape Key
		if(@event is InputEventMouseButton)
		{
			Input.MouseMode = Input.MouseModeEnum.Captured;
		}
		else if(@event.IsActionPressed("ui_cancel"))
		{
			Input.MouseMode = Input.MouseModeEnum.Visible;
		}
		
		if(Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			if(@event is InputEventMouseMotion MouseMotion)
			{
				Neck.RotateY((float)(-MouseMotion.Relative.X * 0.01));
				Camera.RotateX((float)(-MouseMotion.Relative.Y * 0.01));
				
				Vector3 rotation = Camera.Rotation;
				rotation.X = Mathf.Clamp(Camera.Rotation.X, Mathf.DegToRad(-30), Mathf.DegToRad(60));
				Camera.Rotation = rotation;
			}
		}

		// Object Grabber
		if(@event is InputEventMouseButton mouseEvent && mouseEvent.Pressed)
		{
			if(mouseEvent.ButtonIndex == MouseButton.Left)
			{
				PickObject();
			}
		}
	}

	public void PickObject(){
		Object collider = Interaction.GetCollider();

		if(collider != null && collider is RigidBody3D)
		{
			GD.Print("Colliding with a rigid body");
		}
	}
}
