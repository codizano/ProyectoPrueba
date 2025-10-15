using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoPrueba.Models
{
    [Table("students")]
    public class Student
    {
        [Column("_id")]
        [Key]
        public string _id { get; set; } = null!;

        [Column("name")]
        [Required]
        public string Name { get; set; } = null!;

        [Column("created_at")]
        public DateTime? CreatedAt { get; set; }

        [Column("updated_at")]
        public DateTime? UpdatedAt { get; set; }
    }
}