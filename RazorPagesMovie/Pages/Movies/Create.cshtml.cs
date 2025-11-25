using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorPagesMovie.Data;
using RazorPagesMovie.Models;
using System.IO;
using System.Threading.Tasks;

namespace RazorPagesMovie.Pages.Movies
{
    public class CreateModel : PageModel
    {
        private readonly RazorPagesMovieContext _context;

        public CreateModel(RazorPagesMovieContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Movie Movie { get; set; } = default!;

        [BindProperty]
        public IFormFile? ImageFile { get; set; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // 1️⃣ Save movie to get ID
            _context.Movie.Add(Movie);
            await _context.SaveChangesAsync(); // Movie.ID now exists

            // 2️⃣ Save image if uploaded
            if (ImageFile != null)
            {
                var ext = Path.GetExtension(ImageFile.FileName);
                var fileName = Movie.Id + ext;

                // Use absolute path
                var root = Directory.GetCurrentDirectory();
                var filePath = Path.Combine(root, "wwwroot/images/movies", fileName);

                using (var stream = System.IO.File.Create(filePath))
                {
                    await ImageFile.CopyToAsync(stream);
                }
            }

            // 3️⃣ Always redirect after saving
            return RedirectToPage("./Index");
        }
    }
}
