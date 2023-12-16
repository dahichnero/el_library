﻿using System.ComponentModel.DataAnnotations;

namespace ElLibrary.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Укажите имя пользователя")]
        public string Username { get; set; }
        [Required(ErrorMessage = "Введите пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
