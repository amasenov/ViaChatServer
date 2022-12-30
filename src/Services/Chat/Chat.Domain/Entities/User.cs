using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using ViaChatServer.BuildingBlocks.Infrastructure.Entities;

namespace Chat.Domain.Entities
{
    public class User : BaseIdEntity
    {
        [SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public User()
        {
            Posts = new HashSet<Post>();
        }

        public string Name { get; set; }

        [SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Post> Posts { get; set; }
    }
}
