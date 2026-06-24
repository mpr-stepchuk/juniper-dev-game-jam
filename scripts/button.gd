extends Button

@export var target_signal: Signal
@export var target_object: Node
@export var rich_text: String
@export var text_colour: Color
@export var effect_ratio: float = 1.0

@onready var TextLabel: RichTextLabel = $RichTextLabel

var tween: Tween
var time_elapsed: float
var hovered: bool = false

# Called when the node enters the scene tree for the first time.
func _ready():
	if target_signal != null and target_object != null:
		target_object.target_signal.connect()
	TextLabel.push_color(text_colour)
	TextLabel.append_text(rich_text)
	if get_parent().global_position != null:
		global_position += get_parent().get_parent().global_position

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	if is_hovered() and !hovered:
		hover()
		hovered = true
	elif !is_hovered() and mouse_exited:
		unhover()
		hovered = false

func hover():
	var scale_to: float = 1.2
	
	pivot_offset = size / 2.0
	scale_to = 1.2
	
	if tween and tween.is_running():
		tween.kill()
	tween = create_tween().set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_BACK)
	tween.parallel().tween_property(self, "scale:x", scale_to*effect_ratio, 0.2)
	tween.parallel().tween_property(self, "rotation_degrees", 6.0*[-1, 1].pick_random()*effect_ratio, 0.2)
	tween.parallel().tween_property(self, "rotation_degrees", 0.0, 0.4).set_delay(0.1)
	return
	
func unhover():
	
	pivot_offset = size / 2.0
	
	tween = create_tween().set_ease(Tween.EASE_OUT).set_trans(Tween.TRANS_BACK)
	tween.parallel().tween_property(self, "scale:x", 1.0, 0.1)
	return
