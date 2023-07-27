using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities.Models
{
    public class ShapedEntity
    {
        public int Id { get; set; }
        public Entity Entity { get; set; } //  Bu ifade referans alması gerektiği için ctor oluşturduk ve alttaki kodu yazdık.

        public ShapedEntity()
        {
            Entity = new Entity();
        }
    }
}
