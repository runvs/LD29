using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    public enum TriggerAreaType
    {
        TAT_PORTAL,
        TAT_QUEST,
        //TAT_ITEM  ?
    }

    public class TriggerArea
    {
        private FloatRect _rect;

        public string Id { get; private set; }
        public TriggerAreaType Type { get; private set; }

        public TriggerArea(FloatRect rect, TriggerAreaType type, string id)
        {
            _rect = rect;
            Type = type;
            Id = id;
        }

        public bool CheckIsInside(Vector2f pos)
        {
            return _rect.Contains(pos.X, pos.Y);
        }
    }
}
