extends Node2D

var prize: String

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_roulette_wheel_rich_text_wheel_spun(colour):
	match colour:
		0:
			print("Super Upgrade")
		1:
			print("Damage Upgrade")
		2:
			print("Health Pack")
