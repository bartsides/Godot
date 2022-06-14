using NotRimworld.Needs;

namespace NotRimworld.Directives
{
    public interface IDirective
    {
        INeed Need { get; set; }
        string Name { get; set; }
        void Handle(Character character, float delta);
    }
}
