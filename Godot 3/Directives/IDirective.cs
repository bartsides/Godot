using NotRimworld.code;
using NotRimworld.Needs;
using Playerr = NotRimworld.code.Player;

namespace NotRimworld.Directives
{
    public interface IDirective
    {
        INeed Need { get; set; }
        string Name { get; set; }
        void Handle(Playerr player, float delta);
    }
}
