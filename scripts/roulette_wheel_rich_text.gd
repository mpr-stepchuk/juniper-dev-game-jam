extends RichTextLabel

signal wheel_spun(colour: int)

var colour: int
var number: int
var spinning: bool
var time_elapsed: float

# Called when the node enters the scene tree for the first time.
func _ready():
	number = randi_range(0, 36)
	spinning = false
	time_elapsed = 0

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	time_elapsed += delta*3.0
	if(!spinning):
		if(int(time_elapsed) == 1):
			time_elapsed = 0.0
			clear()
			push_bgcolor(Color.WHITE)
			number += 1
			if number == 37:
				number = 0
			_add_number()
	
	if Input.is_action_just_pressed("debug_spin_wheel"):
		for i in range (randi_range(20,70)):
			await get_tree().create_timer(0.1*(float(i)/30.0+1.0)).timeout
			clear()
			push_bgcolor(Color.WHITE)
			number += 1
			if number == 37:
				number = 0
			spinning = true
			_add_number()
		wheel_spun.emit(colour)
		await get_tree().create_timer(2.0).timeout
		spinning = false
		time_elapsed = 0.0

func _add_number():
	if (number == 0):
		push_color(Color.GREEN)
		colour = 0
	elif (number % 2 == 0):
		push_color(Color.BLACK)
		colour = 1
	else:
		push_color(Color.RED)
		colour = 2
	append_text(str(number))
	return
