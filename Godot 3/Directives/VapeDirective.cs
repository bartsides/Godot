using NotRimworld.Enums;
using NotRimworld.Needs;
using System;
using NotRimworld.code;
using Playerr = NotRimworld.code.Player;

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

        public void Handle(Playerr player, float delta)
        {
            if (Need == null) return;

            switch (player.State)
            {
                case PlayerState.Follow:
                    return;
                case PlayerState.Idle:
                    player.GoToClosest(1);
                    return;
                case PlayerState.Interacting:
                    // TODO: Show cloud
                    Need.Value = Math.Max(Need.Value - 10 * delta, 0);
                    if (Need.Value < 1)
                    {
                        player.ClearDirective();
                    }
                    return;
            }
        }
    }
}
