﻿namespace Workly.Application.DTOs.Users
{
    public class RegisterUserDto
    {
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string ConfirmPassword { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string? PhoneNumber { get; set; }
    }
}
