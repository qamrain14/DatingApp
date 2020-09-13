using System;
using System.ComponentModel.DataAnnotations;

namespace datingApp.api.DTOs
{
    public class UserForRegisterDTO
    {
        [Required]
        public String UserName{get;set;}

        [Required]
        [StringLength(8,MinimumLength = 4,ErrorMessage = "Please specify password between 4 to 8 charachters")]
        public String Password{get;set;}
    }
}