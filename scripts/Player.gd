extends Node

const VapeDirective = preload("VapeDirective.gd")

# Needs
export(float) var vape = 0.0

# Increments
export(float) var vapeIncrement = 1.0

var directive = null;

func _ready():
	print("ready");
	directive = VapeDirective.new();

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	increment_needs(delta);
	handle_directives();

func increment_needs(delta):
	vape += vapeIncrement * delta;
	if vape > 100:
		vape = 100;

func handle_directives():
	if directive != null:
		directive.handle(self);
	else:
		select_next_directive();

func select_next_directive():
	# directive ranking logic here
	pass
