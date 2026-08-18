using Application.DTO.Login;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Interfaces
{
    public interface IAuthService
    {
        Task<string> LoginAsync(LoginDto dto);
    }
}
