using System.ComponentModel.DataAnnotations;

namespace Blog.Users.Dto
{
    public class ChangeUserLanguageDto
    {
        [Required]
        public string LanguageName { get; set; }
    }
}