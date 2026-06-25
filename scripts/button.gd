extends Button

@export var rich_text: String
@export var text_colour: Color
@export var font_size: int = 16
@export var effect_ratio: float = 1.0
@export var outline_size: int = 0
@export var outline_colour: Color = Color.WHITE
@export var given_texture: Texture
@export var update_number: int = -1

@onready var TextLabel: RichTextLabel = $RichTextLabel
@onready var ApplyTexture: TextureRect = $TextureRect

var tween: Tween
var time_elapsed: float
var hovered: bool = false

# Called when the node enters the scene tree for the first time.
func _ready():
	TextLabel.push_color(text_colour)
	TextLabel.push_font_size(font_size)
	TextLabel.push_outline_size(outline_size)
	TextLabel.push_outline_color(outline_colour)
	TextLabel.append_text(rich_text)
	if given_texture != null:
		ApplyTexture.texture = given_texture

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


func _on_roulette_wheel_update_button(prize_number: int):
	if update_number != -1 and prize_number == update_number:
		TextLabel.clear()
		_ready()
