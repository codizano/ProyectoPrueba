using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoPrueba.Models
{
    [Table("sheets")]
    public class Sheet
    {
        [Column("id")]
        public int Id { get; set; }

        [Column("student_id")]
        public string? StudentId { get; set; }

        [Column("sheet_name")]
        [Required]
        public string SheetName { get; set; } = null!;

        [Column("objective")]
        public string? Objective { get; set; }

        [Column("opinion")]
        public string? Opinion { get; set; }

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [ForeignKey("StudentId")]
        public Student? Student { get; set; }
    }
}