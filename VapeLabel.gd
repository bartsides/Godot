extends Label

const VapeNeed = preload("res://VapeNeed.gd")

var vape = 0.0;

# Called when the node enters the scene tree for the first time.
func _ready():
	get_vape();

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	get_vape();

func get_vape():
	var node = get_parent().get_parent().get_node("Character");
	var need : VapeNeed = node.get_need(typeof(VapeNeed));
	if need == null:
		vape = 0.0;
	else:
		vape = need.value;
	#print(var2str(vape));
	text = "Vape: " + var2str(int(vape));
