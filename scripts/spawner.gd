extends Marker2D

@export var actor_scene: PackedScene

# Called when the node enters the scene tree for the first time.
func _ready():
	pass

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	pass

func _spawn_actor():
	if actor_scene != null:
		var actor: Node2D = actor_scene.instantiate() as Node2D
		actor.global_transform = global_transform
		get_parent().add_child.call_deferred(actor)
	return

func _on_wave_start():
	print("START WAVE")
	_spawn_actor()
