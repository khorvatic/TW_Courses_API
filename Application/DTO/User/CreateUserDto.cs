using System;
using System.Collections.Generic;
using System.Text;

namespace Application.DTO.User
{
    public class CreateUserDto
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
    }
}
