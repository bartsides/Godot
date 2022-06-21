using NotRimworld.code;
using NotRimworld.Directives;

namespace NotRimworld.Needs
{
    public interface INeed
    {
        float Value { get; set; }
        float Minimum { get; set; }
        float Increment { get; set; }

        void Handle(Player player, float delta);
        IDirective GetDirective();
    }
}
