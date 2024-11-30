using BookNest.Models;
using BookNest.Repositories.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace BookNest.Controllers
{
    [Authorize]
    public class BookController : Controller
    {
        private readonly IBookRepository _bookRepository;
        private readonly IBookIssueRequestRepository _bookIssueRequestRepository;
        private readonly IBookIssueRepository _bookIssueRepository;
        private readonly IUserRepository _userRepository;
        private readonly UserManager<User> _userManager;

        public BookController(IBookRepository bookRepository, IBookIssueRequestRepository bookIssueRequestRepository, IBookIssueRepository bookIssueRepository, IUserRepository userRepository, UserManager<User> userManager)
        {
            _bookRepository = bookRepository;
            _bookIssueRequestRepository = bookIssueRequestRepository;
            _bookIssueRepository = bookIssueRepository;
            _userRepository = userRepository;
            _userManager = userManager;
        }

        public IActionResult Index()
        {
            var user = _userRepository.GetAllUsers().FirstOrDefault(u => u.UserName == User.Identity.Name);
            if (user == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var books = _bookRepository.GetAllBooks();
            ViewBag.IsAdmin = _userManager.IsInRoleAsync(user, "Admin").Result;
            return View(books);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult UpdateBook(Book book)
        {
            if (ModelState.IsValid)
            {
                var existingBook = _bookRepository.GetBookById(book.Id);
                existingBook.Title = book.Title;
                existingBook.Author = book.Author;
                existingBook.Quantity = book.Quantity;
                _bookRepository.UpdateBook(existingBook);
                TempData["SuccessMessage"] = "Book updated successfully!";
                return RedirectToAction("Index");
            }

            TempData["ErrorMessage"] = "Failed to update book.";
            return RedirectToAction("Index");
        }


        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteBook(int bookId)
        {
            var book = _bookRepository.GetBookById(bookId);
            if (book == null)
            {
                return BadRequest("Book does not exist.");
            }

            _bookRepository.DeleteBook(bookId);
            TempData["SuccessMessage"] = "Book deleted successfully!";
            return RedirectToAction("Index");
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult AdminIssueBook(int bookId, DateTime? returnDate)
        {
            if (returnDate == null)
            {
                TempData["ErrorMessage"] = "Return date is required.";
                return RedirectToAction("Index");
            }

            var book = _bookRepository.GetBookById(bookId);
            var user = _userRepository.GetAllUsers().FirstOrDefault(u => u.UserName == User.Identity.Name);

            if (book == null || user == null)
            {
                return NotFound("Book or user not found.");
            }

            var bookIssue = new BookIssue
            {
                BookId = bookId,
                UserId = user.Id,
                IssueDate = DateTime.Now,
                ReturnDate = returnDate.Value
            };

            _bookIssueRepository.AddBookIssue(bookIssue);

            book.Quantity -= 1;
            _bookRepository.UpdateBook(book);

            TempData["SuccessMessage"] = "Book issued successfully!";
            return RedirectToAction("Index");
        }

        [HttpPost]
        public IActionResult RequestIssue(int bookId, DateTime returnDate)
        {
            var book = _bookRepository.GetBookById(bookId);
            if (book == null || book.Quantity <= 0)
            {
                return BadRequest("Book not available or does not exist.");
            }

            var userId = _userRepository.GetAllUsers().FirstOrDefault(u => u.UserName == User.Identity.Name)?.Id;
            if (userId == null)
            {
                return BadRequest("User does not exist.");
            }

            // চেক: ব্যবহারকারী রেস্ট্রিক্টেড কিনা
            var existingIssue = _bookIssueRepository.GetAllBookIssues().FirstOrDefault(bi => bi.UserId == userId && bi.DueDate < DateTime.Now && !bi.IsReturned);
            if (existingIssue != null)
            {
                return RedirectToAction("Manage", "Account");
            }

            var request = new BookIssueRequest
            {
                BookId = bookId,
                UserId = userId,
                RequestDate = DateTime.Now,
                ReturnDate = returnDate,
                IsApproved = false,
                IsPending = true
            };

            _bookIssueRequestRepository.AddRequest(request);

            // কোয়ান্টিটি কমানো
            book.Quantity -= 1;
            _bookRepository.UpdateBook(book);

            return Ok();
        }
    }
}
