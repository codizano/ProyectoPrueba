using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoPrueba.Models
{
    [Table("sheet_observations")]
    public class SheetObservation
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("sheet_id")]
        public int? SheetId { get; set; }

        [Column("observation")]
        [Required]
        public string Observation { get; set; } = null!;

        [Column("observation_date")]
        [Required]
        public DateTime ObservationDate { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("SheetId")]
        public Sheet? Sheet { get; set; }
    }
}