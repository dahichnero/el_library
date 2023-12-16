﻿using ElLibrary.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ElLibrary.ViewModels
{
    public class BookViewModel
    {
        [MinLength(2)]
        [MaxLength(150)]
        [Display(Name = "Название книги")]
        public string Title { get; set; } = string.Empty;
        [MinLength(2)]
        [MaxLength(150)]
        [Display(Name = "Автор")]
        public string Author { get; set; } = string.Empty;
        [Range(0, 10000)]
        [Display(Name = "Количество страниц")]
        public int PageCount { get; set; }
        [Required]
        [Display(Name = "Изображение")]
        public IFormFile Photo { get; set; }
        [Required]
        [Display(Name = "Файл книги (pdf)")]
        public IFormFile File { get; set; }
        [Required]
        [Display(Name = "Категории")]
        public int CategoryId { get; set; }
        public List<SelectListItem> Categories { get; set; } = new();
    }
}
