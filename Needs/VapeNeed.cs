using NotRimworld.Directives;
using NotRimworld.Enums;
using System;
using NotRimworld.code;

namespace NotRimworld.Needs
{
    public class VapeNeed : INeed
    {
        public float Value { get; set; }
        public float Minimum { get; set; }
        public float Increment { get; set; }

        public VapeNeed()
        {
            Value = 0;
            Minimum = 30;
            Increment = 7;
        }

        public void Handle(Player player, float delta)
        {
            if (player.State == PlayerState.Interacting)
                return;
            Value = Math.Min(Value + Increment * delta, 100);
        }

        public IDirective GetDirective()
        {
            return new VapeDirective(this);
        }
    }
}
