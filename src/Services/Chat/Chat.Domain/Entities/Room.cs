using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using ViaChatServer.BuildingBlocks.Infrastructure.Entities;

namespace Chat.Domain.Entities
{
    public class Room : BaseIdEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Room()
        {
            Posts = new HashSet<Post>();
        }

        public string Name { get; set; }
        public int Limit { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Post> Posts { get; set; }
    }
}
