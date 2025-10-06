using System.ComponentModel.DataAnnotations;

namespace ModelBindingDemo.Models.DTOs
{
    public class UploadProfilePictureDto
    {
        [Required(ErrorMessage = "Please select a file")]
        public IFormFile ProfilePicture { get; set; }

        public string Description { get; set; }
    }
}