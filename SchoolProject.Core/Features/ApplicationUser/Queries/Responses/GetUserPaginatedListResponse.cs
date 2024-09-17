﻿namespace SchoolProject.Core.Features.ApplicationUser.Queries.Responses
{
    public class GetUserPaginatedListResponse
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string? Address { get; set; }
        public string? Country { get; set; }
    }
}