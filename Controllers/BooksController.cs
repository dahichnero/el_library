using ElLibrary.Domain.Entities;
using ElLibrary.Domain.Services;
using ElLibrary.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Principal;

namespace ElLibrary.Controllers
{
    public class BooksController :Controller
    {
        private readonly IBookReader reader;
        private readonly IBooksService booksService;
        private readonly IWebHostEnvironment appEnvironment;
        private readonly ILogger<BooksController> logger;

        public BooksController (IBookReader reader, IBooksService booksService, IWebHostEnvironment appEnvironment, ILogger<BooksController> logger)
        {
            this.reader = reader;
            this.booksService = booksService;
            this.appEnvironment = appEnvironment;
            this.logger = logger;
        }

        [Authorize]
        public async Task<IActionResult> Index (string searchString="",int categoryId=0)
        {
            var viewModel = new BooksCatalogViewModel
            {
                Books=await reader.FindBooksAsync(searchString,categoryId),
                Categories=await reader.GetCategoriesAsync()
            };
            return View(viewModel);
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> AddBook ()
        {
            logger.LogInformation("Вы зашли на страницу добавления");
            var viewModel = new BookViewModel();
            var categories=await reader.GetCategoriesAsync();
            var items=categories.Select(c=> new SelectListItem { Text=c.Name, Value=c.Id.ToString() });
            viewModel.Categories.AddRange(items);
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> AddBook (BookViewModel bookVm)
        {
            logger.LogInformation("Происходит добавление");
            if (!ModelState.IsValid)
            {
                logger.LogWarning("Состояние модели не валидное");
                return View(bookVm);
            }
            try
            {
                var book = new Book
                {
                    Author = bookVm.Author,
                    Title=bookVm.Title,
                    CategoryId=bookVm.CategoryId,
                    PagesCount=bookVm.PageCount
                };
                string wwwroot = appEnvironment.WebRootPath;
                book.Filename = await booksService.LoadFile(bookVm.File.OpenReadStream(),
                    Path.Combine(wwwroot, "books"));
                book.ImageUrl = await booksService.LoadPhoto(bookVm.Photo.OpenReadStream(),
                    Path.Combine(wwwroot, "images", "books"));
                await booksService.AddBook(book);
            }
            catch (IOException)
            {
                logger.LogWarning("Не удалось сохранить файл");
                ModelState.AddModelError("ioerror", "Не удалось сохранить файл");
                return View(bookVm);
            } 
            catch
            {
                logger.LogWarning("Не удалось сохранить в БД");
                ModelState.AddModelError("database", "Ошибка при сохранении в базу данных");
                return View(bookVm);
            }
            logger.LogInformation("Добавление завершено");
            return RedirectToAction("Index","Books");
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> UpdateBook(int bookId)
        {
            var book=await reader.FindBookAsync(bookId);
            if (book is null)
            {
                return NotFound();
            }
            var bookVm = new UpdateBookViewModel
            {
                Id=book.Id,
                Author=book.Author,
                Title=book.Title,
                CategoryId=book.CategoryId,
                PageCount=book.PagesCount,
                FileString=book.Filename,
                PhotoString=book.ImageUrl
            };
            var categories = await reader.GetCategoriesAsync();
            var items = categories.Select(c =>
            new SelectListItem
            {
                Text=c.Name, Value=c.Id.ToString(),
            });
            bookVm.Categories.AddRange(items);
            return View(bookVm);
        }

        [HttpPost]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> UpdateBook(UpdateBookViewModel bookVm)
        {
            logger.LogInformation("Происходит изменение");
            if (!ModelState.IsValid)
            {
                return View(bookVm);
            }
            var book = await reader.FindBookAsync(bookVm.Id);
            if (book is null)
            {
                ModelState.AddModelError("not_found", "Книга не найдена");
                return View(book);
            }
            try
            {

                book.Author = bookVm.Author;
                book.CategoryId = bookVm.CategoryId;
                book.Title = bookVm.Title;
                book.PagesCount = bookVm.PageCount;
                string wwwroot = appEnvironment.WebRootPath;
                if (bookVm.File is not null)
                {
                    book.Filename = await booksService.LoadFile(bookVm.File.OpenReadStream(),
                        Path.Combine(wwwroot, "images", "books"));

                }
                await booksService.UpdateBook(book);
            }
            catch (IOException)
            {
                logger.LogWarning("Не удалось сохранить файл");
                ModelState.AddModelError("ioerror", "Не удалось сохранить файл");
                return View(bookVm);
            } 
            catch
            {
                logger.LogWarning("Не удалось сохранить в БД");
                ModelState.AddModelError("database", "Ошибка при сохранении в базу данных");
                return View(bookVm);
            }
            logger.LogInformation("Изменение успешно");
            return RedirectToAction("Index", "Books");
        }

        [HttpGet]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            logger.LogInformation("Вы зашли на страницу удаления");
            var book = await reader.FindBookAsync(bookId);
            if (book is null)
            {
                logger.LogWarning("Книга не найдена");
                NotFound();
            }
            var bookVm = new DeleteBookViewModel
            {
                Id=book.Id,
                Title=book.Title,
                Author=book.Author,
                PageCount=book.PagesCount,
                CategoryId=book.CategoryId,
            };
            var categories = await reader.GetCategoriesAsync();
            bookVm.Category = categories.First(c => c.Id == bookVm.CategoryId);
            return View(bookVm);
        }

        [HttpPost]
        [Authorize(Roles ="admin")]
        public async Task<IActionResult> DeleteBook(DeleteBookViewModel bookVm)
        {
            logger.LogInformation("Происходит удаление");
            if (!ModelState.IsValid)
            {
                return View(bookVm);
            }
            var book=await reader.FindBookAsync(bookVm.Id);
            if (book is null)
            {
                ModelState.AddModelError("not_found", "Книга не найдена");
                return View(book);
            }
            try
            {
                
                await booksService.DeleteBook(book);
            } 
            catch
            {
                logger.LogWarning("Не удалось удалить из БД");
                ModelState.AddModelError("database", "Ошибка при удалении из базы данных");
                return View(bookVm);
            }
            logger.LogInformation("Удаение завершено");
            return RedirectToAction("Index", "Books");
        }
    }
}
