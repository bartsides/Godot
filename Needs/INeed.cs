using NotRimworld.code;
using NotRimworld.Directives;
using Playerr = NotRimworld.code.Player;

namespace NotRimworld.Needs
{
    public interface INeed
    {
        float Value { get; set; }
        float Minimum { get; set; }
        float Increment { get; set; }

        void Handle(Playerr player, float delta);
        IDirective GetDirective();
    }
}
