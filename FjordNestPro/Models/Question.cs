using System;
using System.ComponentModel.DataAnnotations;

namespace FjordNestPro.Models
{
    public class Question
    {
        [Key]
        public int QuestionID { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(255)]
        public required string Email { get; set; }

        [Required]
        [MaxLength(1000)]  // Adjust for typical question length
        public required string Content { get; set; }

       
        [MaxLength(1000)]
        public string? ImageUrl { get; set; }

        [MaxLength(1000)]  // Adjust for typical answer length
        public string? AnswerContent { get; set; }

        
        [MaxLength(1000)]
        public string? AnswerImageUrl { get; set; }
    }
}

