using System.ComponentModel.DataAnnotations;

namespace Workly.Domain.Entities
{
    public class BaseEntity
    {
        [Key]
        public int RecId { get; set; }
        public DateTime? InsertedAt { get; set; }
        public int? InsertedBy { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? UpdatedBy { get; set; }
    }
}
