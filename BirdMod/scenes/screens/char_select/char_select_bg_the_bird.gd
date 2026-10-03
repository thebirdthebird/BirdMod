extends TextureRect

func _ready():
	if ResourceLoader.exists("res://images/packed/portraits/birdmod_bg.png"):
		texture = load("res://images/packed/portraits/birdmod_bg.png")
