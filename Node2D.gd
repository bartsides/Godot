extends Node2D

var Texture_Face = ["Head1", "Head2", "Head3"]
var Texture_Body = ["Body1", "Body2", "Body3"]
var Texture_LHands = ["Hand L 1", "Hand L 2", "Hand L 3"]
var Texture_RHands = ["Hand R 1", "Hand R 2", "Hand R 3"]



# Declare member variables here. Examples:
# var a = 2
# var b = "text
# Called when the node enters the scene tree for the first time.
func _ready():
	
	var Randy = get_node("Rig\Bone_Head\Bone_Head").Texture = $Body1
	
return Randy
	
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass
