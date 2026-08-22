using Application.DTO.Review;
using Application.DTO.User;
using Application.Interfaces;
using Domain.Exceptions;
using Domain.Interfaces;
using Domain.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher<User> _passwordHasher;

        public UserService(IUnitOfWork unitOfWork, IPasswordHasher<User> passwordHasher)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
        }

        public async Task<UserDto> CreateUserAsync(CreateUserDto dto)
        {
            var existingUser = await _unitOfWork.Users.GetUserByEmailAsync(dto.Email);
            if (existingUser != null) throw new BusinessRuleException("User with this email already exists.");

            var user = new User
            {
                Name = dto.Name,
                Surname = dto.Surname,
                Email = dto.Email,
                DateOfRegistration = DateOnly.FromDateTime(DateTime.UtcNow)
            };

            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            var role = await _unitOfWork.Roles.GetRoleByNameAsync("User");
            if (role == null) throw new NotFoundException("Role 'User' doesn't exist");

            await _unitOfWork.Users.AddAsync(user);
            await _unitOfWork.UserRoles.AddAsync(new UserRole { User = user, Role = role });
            await _unitOfWork.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email
            };
        }

        public async Task DeleteUserAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) throw new NotFoundException("Cannot delete because User with that ID not found.");

            await _unitOfWork.Users.DeleteAsync(id);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _unitOfWork.Users.GetAllAsync();

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                Reviews = u.Reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Text = r.Text,
                    DateOfReview = r.DateOfReview,
                    NumOfStars = r.NumOfStars,
                    CourseId = r.CourseId,
                    UserId = r.UserId
                }).ToList()
            });
        }

        public async Task<UserDto> GetUserByEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetUserByEmailAsync(email);
            if (user == null) throw new NotFoundException("User with that email address not found.");

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Reviews = user.Reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Text = r.Text,
                    DateOfReview = r.DateOfReview,
                    NumOfStars = r.NumOfStars,
                    CourseId = r.CourseId,
                    UserId = r.UserId
                }).ToList()
            };
        }

        public async Task<UserDto> GetUserByIdAsync(int id)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) throw new NotFoundException("User with that ID not found.");

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Reviews = user.Reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Text = r.Text,
                    DateOfReview = r.DateOfReview,
                    NumOfStars = r.NumOfStars,
                    CourseId = r.CourseId,
                    UserId = r.UserId
                }).ToList()
            };
        }

        public async Task<IEnumerable<UserDto>> GetUsersByFullNameAsync(string name, string surname)
        {
            var users = await _unitOfWork.Users.GetUsersByFullNameAsync(name, surname);
            if (users == null) throw new NotFoundException("No Users with that name and surname found");

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                Reviews = u.Reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Text = r.Text,
                    DateOfReview = r.DateOfReview,
                    NumOfStars = r.NumOfStars,
                    CourseId = r.CourseId,
                    UserId = r.UserId
                }).ToList()
            });
        }

        public async Task<IEnumerable<UserDto>> GetUsersByNameAsync(string name)
        {
            var users = await _unitOfWork.Users.GetUsersByNameAsync(name);
            if (users == null) throw new NotFoundException("No Users with that name found");

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                Reviews = u.Reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Text = r.Text,
                    DateOfReview = r.DateOfReview,
                    NumOfStars = r.NumOfStars,
                    CourseId = r.CourseId,
                    UserId = r.UserId
                }).ToList()
            });
        }

        public async Task<IEnumerable<UserDto>> GetUsersBySurnameAsync(string surname)
        {
            var users = await _unitOfWork.Users.GetUsersBySurnameAsync(surname);
            if (users == null) throw new NotFoundException("No Users with that surname found");

            return users.Select(u => new UserDto
            {
                Id = u.Id,
                Name = u.Name,
                Surname = u.Surname,
                Email = u.Email,
                Reviews = u.Reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Text = r.Text,
                    DateOfReview = r.DateOfReview,
                    NumOfStars = r.NumOfStars,
                    CourseId = r.CourseId,
                    UserId = r.UserId
                }).ToList()
            });
        }

        public async Task<UserDto> UpdateUserAsync(int id, CreateUserDto dto)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(id);
            if (user == null) throw new NotFoundException("Cannot update because User with that ID not found.");

            user.Name = dto.Name;
            user.Surname = dto.Surname;
            user.Email = dto.Email;
            user.PasswordHash = _passwordHasher.HashPassword(user, dto.Password);

            _unitOfWork.Users.Update(user);
            await _unitOfWork.SaveChangesAsync();

            return new UserDto
            {
                Id = user.Id,
                Name = user.Name,
                Surname = user.Surname,
                Email = user.Email,
                Reviews = user.Reviews.Select(r => new ReviewDto
                {
                    Id = r.Id,
                    Text = r.Text,
                    DateOfReview = r.DateOfReview,
                    NumOfStars = r.NumOfStars,
                    CourseId = r.CourseId,
                    UserId = r.UserId
                }).ToList()
            };
        }
    }
}
