using ElLibrary.Domain.Entities;
using ElLibrary.Domain.Services;

namespace ElLibrary.Infrastructure
{
    public class BooksService :IBooksService
    {
        private readonly IRepository<Book> books;

        public BooksService (IRepository<Book> books)
        {
            this.books = books;
        }

        public async Task AddBook (Book book)
        {
            await books.AddAsync(book);
        }

        public async Task DeleteBook (Book book)
        {
            await books.DeleteAsync(book);
        }

        private async Task CopyFromStream(Stream stream, string filename)
        {
            using (var writer = new FileStream(filename, FileMode.Create))
            {
                int count = 0;
                byte[] buffer = new byte[1024];
                do
                {
                    count=await stream.ReadAsync(buffer, 0, buffer.Length);
                    await writer.WriteAsync(buffer, 0, count);
                }
                while (count > 0);
            }
        }
        public async Task<string> LoadFile (Stream file, string path)
        {
            var filename = Path.GetRandomFileName() + ".pdf";
            var fullname=Path.Combine(path, filename);
            await CopyFromStream(file, fullname);
            return filename;
        }

        public async Task<string> LoadPhoto (Stream file, string path)
        {
            var filename = Path.GetRandomFileName() + ".png";
            var fullname = Path.Combine(path, filename);
            await CopyFromStream(file, fullname);
            return filename;
        }

        public async Task UpdateBook (Book book)
        {
            await books.UpdateAsync(book);
        }
    }
}
