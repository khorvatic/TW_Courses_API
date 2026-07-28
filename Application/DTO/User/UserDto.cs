using Application.DTO.Review;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.User
{
    public class UserDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public ICollection<ReviewDto> Reviews { get; set; }
    }
}
