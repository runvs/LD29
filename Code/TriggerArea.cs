using System;
using SFML.Graphics;
using SFML.Window;

namespace JamTemplate
{
    public class TriggerArea
    {
        private FloatRect _rect;

        public enum TriggerAreaType
        {
            TAT_PORTAL,
            TAT_QUEST,
            //TAT_ITEM  ?
        }

        private TriggerAreaType _type;
        private String _id;


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
