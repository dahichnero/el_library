using ElLibrary.Domain.Entities;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace ElLibrary.ViewModels
{
    public class DeleteBookViewModel
    {
        public int Id { get; set; }


        [Display(Name = "Название книги")]
        public string Title { get; set; } = string.Empty;


        [Display(Name = "Автор")]
        public string Author { get; set; } = string.Empty;


        [Display(Name = "Количество страниц")]
        public int PageCount { get; set; }

        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }
    }
}
