extends TextureRect

func _ready():
	if ResourceLoader.exists("res://images/packed/portraits/birdmod_portrait.png"):
		texture = load("res://images/packed/portraits/birdmod_portrait.png")
