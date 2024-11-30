using Hangfire;
using System;
using System.Linq;
using System.Threading.Tasks;
using BookNest.Repositories.Interfaces;

public class ExpiredBookJob
{
    private readonly IBookRepository _bookRepository;
    private readonly IUserRepository _userRepository;
    private readonly EmailService _emailService;

    public ExpiredBookJob(IBookRepository bookRepository, IUserRepository userRepository, EmailService emailService)
    {
        _bookRepository = bookRepository;
        _userRepository = userRepository;
        _emailService = emailService;
    }

    [AutomaticRetry(Attempts = 0)]
    public async Task CheckExpiredBooks()
    {
        var expiredBooks = _bookRepository.GetExpiredBookIssues();
        foreach (var issue in expiredBooks)
        {
            var user = _userRepository.GetUserById(issue.UserId);
            if (user != null)
            {
                var message = $"Dear {user.FullName},\n\nYour book '{issue.Book.Title}' is overdue. Please return it as soon as possible.";
                await _emailService.SendEmailAsync(user.Email, "Overdue Book Notice", message);
            }
        }
    }
}
