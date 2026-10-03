extends TextureRect

func _ready():
	if not ResourceLoader.exists("res://images/packed/portraits/birdmod_creature.png"):
		return
	texture = load("res://images/packed/portraits/birdmod_creature.png")
	if texture == null:
		return
	var w = float(texture.get_width())
	var h = float(texture.get_height())
	if h > 0.0:
		var s = 310.0 / h
		size = Vector2(w, h)
		scale = Vector2(s, s)
		position = Vector2(-(w * s) / 2.0, -310.0)
