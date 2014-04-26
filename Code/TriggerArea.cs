using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    public enum TriggerAreaType
    {
        PORTAL,
        QUEST,
        FUNCTION
        //TAT_ITEM  ?
    }

    public class TriggerArea
    {
        private FloatRect _rect;

        public bool HasTriggered { get; private set; }
        public bool IsTriggeredOnce { get; private set; }
        public string Id { get; private set; }
        public TriggerAreaType Type { get; private set; }

        public TriggerArea(FloatRect rect, TriggerAreaType type, string id, bool isTriggeredOnce = false)
        {
            _rect = rect;
            IsTriggeredOnce = isTriggeredOnce;
            HasTriggered = false;
            Type = type;
            Id = id;
        }

        public bool CheckIsInside(Vector2f pos)
        {
            if (IsTriggeredOnce && HasTriggered)
                return false;

            if (_rect.Contains(pos.X, pos.Y))
            {
                HasTriggered = true;
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
