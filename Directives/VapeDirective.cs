using NotRimworld.Enums;
using NotRimworld.Needs;
using System;

namespace NotRimworld.Directives
{
    public class VapeDirective : IDirective
    {
        public INeed Need { get; set; }
        public string Name { get; set; } = "Vape";

        public VapeDirective(INeed need)
        {
            Need = need;
        }

        public void Handle(Character character, float delta)
        {
            if (Need == null) return;

            switch (character.State)
            {
                case CharacterState.Follow:
                    return;
                case CharacterState.Idle:
                    character.GoToClosest(1);
                    return;
                case CharacterState.Interacting:
                    // TODO: Show cloud
                    Need.Value = Math.Max(Need.Value - 10 * delta, 0);
                    if (Need.Value < 1)
                    {
                        character.ClearDirective();
                    }
                    return;
            }
        }
    }
}
