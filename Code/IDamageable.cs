public interface IDamagable {
	double Health { get; set; }
	void Damage(double amount);
}