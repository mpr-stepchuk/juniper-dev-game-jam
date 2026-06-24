extends Node2D

@onready var CanvasLayer1 = $"CanvasLayer 2"
@onready var CanvasLayer2 = $"CanvasLayer 3"

var prize: String
var colour: int
var number: int
var spinning: bool
var time_elapsed: float

# Called when the node enters the scene tree for the first time.
func _ready():
	CanvasLayer1.transform = global_transform
	CanvasLayer2.transform = global_transform
	CanvasLayer1.rotation = global_rotation
	CanvasLayer2.rotation = global_rotation
	spinning = false
	time_elapsed = 0
	number = randi_range(1, 36)
	if(number != 1):
		rotate(deg_to_rad(-10*(number-1)))
	CanvasLayer1.rotation = global_rotation


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	CanvasLayer1.rotation = global_rotation
	time_elapsed += delta*3.0
	if(!spinning):
		if(int(time_elapsed) == 1):
			time_elapsed = 0.0
			number += 1
			rotate(deg_to_rad(-2))
			CanvasLayer1.rotation = global_rotation
			rotate(deg_to_rad(-2))
			CanvasLayer1.rotation = global_rotation
			rotate(deg_to_rad(-2))
			CanvasLayer1.rotation = global_rotation
			rotate(deg_to_rad(-2))
			CanvasLayer1.rotation = global_rotation
			rotate(deg_to_rad(-2))
			
			if number == 37:
				number = 1
				
	if Input.is_action_just_pressed("debug_spin_wheel") and !spinning:
		spinning = true
		for i in range (randi_range(20,70)):
			await get_tree().create_timer(0.1*(float(i)/30.0+1.0)).timeout
			number += 1
			rotate(deg_to_rad(-2))
			CanvasLayer1.rotation = global_rotation
			rotate(deg_to_rad(-2))
			CanvasLayer1.rotation = global_rotation
			rotate(deg_to_rad(-2))
			CanvasLayer1.rotation = global_rotation
			rotate(deg_to_rad(-2))
			CanvasLayer1.rotation = global_rotation
			rotate(deg_to_rad(-2))

			if number == 37:
				number = 1
		await get_tree().create_timer(2.0).timeout
		spinning = false
		time_elapsed = 0.0
