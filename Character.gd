extends Node

const VapeNeed = preload("res://VapeNeed.gd")
const VapeDirective = preload("res://VapeDirective.gd")

var needs = Array()

var directive = null;

func _ready():
	needs.append(VapeNeed.new());
	directive = VapeDirective.new();

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	increment_needs(delta);
	handle_directives();

func increment_needs(delta):
	for need in needs:
		need.handle(self, delta);

func get_need(type):
	for need in needs:
		if typeof(need) == type:
			return need;

func handle_directives():
	if directive != null:
		directive.handle(self);
	else:
		select_next_directive();

func select_next_directive():
	# directive ranking logic here
	pass
