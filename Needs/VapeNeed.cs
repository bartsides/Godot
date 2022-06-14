using NotRimworld.Directives;
using NotRimworld.Enums;
using System;

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

        public void Handle(Character character, float delta)
        {
            if (character.State == CharacterState.Interacting)
                return;
            Value = Math.Min(Value + Increment * delta, 100);
        }

        public IDirective GetDirective()
        {
            return new VapeDirective(this);
        }
    }
}
