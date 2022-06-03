extends Node

const Directive = preload("Directive.gd")
const VapeDirective = preload("VapeDirective.gd")

# Needs
export(float) var vape = 0.0

# Increments
export(float) var vapeIncrement = 1.0

var directive : Directive = Directive.new();

func _ready():
	print("ready");
	directive = VapeDirective.new();

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	increment_needs();
	handle_directives();

func increment_needs():
	vape += vapeIncrement * get_process_delta_time();
	if vape > 100:
		vape = 100;

func handle_directives():
	if directive != null:
		directive.handle();
	else:
		select_next_directive();

func select_next_directive():
	# directive ranking logic here
	pass
