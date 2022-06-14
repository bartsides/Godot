using NotRimworld.Directives;

namespace NotRimworld.Needs
{
    public interface INeed
    {
        float Value { get; set; }
        float Minimum { get; set; }
        float Increment { get; set; }

        void Handle(Character character, float delta);
        IDirective GetDirective();
    }
}
