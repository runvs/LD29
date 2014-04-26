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
        private TriggerAreaType _type;
        private string _id;

        public string Id { get { return _id; } }

        public TriggerArea(FloatRect rect, TriggerAreaType type, string id)
        {
            _rect = rect;
            _type = type;
            _id = id;
        }

        public bool CheckIsInside(Vector2f pos)
        {
            return _rect.Contains(pos.X, pos.Y);
        }
    }
}
