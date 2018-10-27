using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Persistency.Entities
{
    public abstract class BaseEntity
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid? Id { get; set; }
    }
}