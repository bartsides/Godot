extends Node2D

var Texture_Face = ["Head1", "Head2", "Head3"]
var Texture_Body = ["Body1", "Body2", "Body3"]
var Texture_LHands = ["Hand L 1", "Hand L 2", "Hand L 3"]
var Texture_RHands = ["Hand R 1", "Hand R 2", "Hand R 3"]

var heads = [
	"res://media/Frames/Skull/Frame0.svg",
	"res://media/Frames/Skull/Frame1.svg", 
	"res://media/Frames/Skull/Frame2.svg", 
	"res://media/Frames/Skull/Frame3.svg", 
	"res://media/Frames/Skull/Frame4.svg", 
	"res://media/Frames/Skull/Frame5.svg", 
	"res://media/Frames/Skull/Frame6.svg", 
	"res://media/Frames/Skull/Frame7.svg", 
	"res://media/Frames/Skull/Frame8.svg"
]

var bodies = [
	"res://media/Frames/Clouds/Cloud1.png", 
	"res://media/Frames/Clouds/Cloud2.png", 
	"res://media/Frames/Clouds/Cloud3.png", 
	"res://media/Frames/Clouds/Cloud4.png", 
	"res://media/Frames/Clouds/Cloud.png", 
	"res://media/Frames/Clouds/CloudA.png"
]

var random = RandomNumberGenerator.new()

# Declare member variables here. Examples:
# var a = 2
# var b = "text
# Called when the node enters the scene tree for the first time.
#func _ready():
	
# Called every frame. 'delta' is the elapsed time since the previous frame.
#func _process(delta):
#	pass

func _unhandled_input(event):
	print("click")
	var carl = get_node("Carl")
	
	var head = carl.get_node("Body/Head/Polygon2D")
	head.texture = load(get_next(heads))
	
	var body = carl.get_node("Body/Polygon2D")
	body.texture = load(get_next(bodies))

func get_next(array: Array):
	return array[random.randi_range(0, array.size() - 1)]
	
	
