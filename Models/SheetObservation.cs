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
        public int SheetId { get; set; }

        [Column("observation")]
        [Required]
        public string Observation { get; set; } = null!;

        [Column("observation_date")]
        public DateTime ObservationDate { get; set; }

        public virtual Sheet? Sheet { get; set; }
    }
}