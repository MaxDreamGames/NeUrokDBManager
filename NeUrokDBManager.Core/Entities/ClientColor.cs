using System.Drawing;

namespace NeUrokDBManager.Core.Entities
{
    public class ClientColor
    {
        public Guid ClientId { get; set; }
        public Color Color { get; set; }
        public Client Client { get; set; }
    }
}
