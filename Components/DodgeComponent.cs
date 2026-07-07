using Engine.ECS;
using Events;
using Microsoft.Xna.Framework;

namespace Components;

public class DodgeComponent : IComponent
{
    public double TimeRemainingSecs { get; set; }
    public double CooldownRemainingSecs { get; set; }
    public DodgeEvent DodgeEvent { get; set; }

    public Vector2 Direction => DodgeEvent.Direction;
    public bool CanDodge => TimeRemainingSecs <= 0 && CooldownRemainingSecs <= 0;

    public DodgeComponent(DodgeEvent dodgeEvent)
    {
        TimeRemainingSecs = dodgeEvent.TimeSeconds;
        CooldownRemainingSecs = dodgeEvent.CooldownSeconds;
        DodgeEvent = dodgeEvent;
    }
}
