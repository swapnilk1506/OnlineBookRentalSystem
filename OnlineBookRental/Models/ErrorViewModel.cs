using System;

namespace OnlineBookRental.Web.Models
{
    // ViewModel for displaying error information.
    public class ErrorViewModel
    {
        // Request ID for tracking errors.
        public string? RequestId { get; set; }
        // Indicates if the RequestId should be shown.
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
