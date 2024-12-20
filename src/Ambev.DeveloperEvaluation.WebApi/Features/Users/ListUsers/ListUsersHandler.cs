using Ambev.DeveloperEvaluation.Domain.Repositories;

namespace Ambev.DeveloperEvaluation.WebApi.Features.Users.ListUsers;

public class ListUsersHandler
{
    private readonly IUserRepository _userRepository;

    public ListUsersHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ListUsersResponse> HandleAsync(ListUsersRequest request)
    {
        var users = await _userRepository.GetAllPagedAsync(request.PageNumber, request.PageSize);
        var totalCount = await _userRepository.GetTotalCountAsync();

        return new ListUsersResponse
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = totalCount,
            Users = users.Select(u => new ListUsersResponse.UserDto
            {
                Id = u.Id,
                Name = u.Username,
                Email = u.Email,
                Role = u.Role.ToString()
            }).ToList()
        };
    }
}

public class ListUsersResponse
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalCount { get; set; }
    public required List<UserDto> Users { get; set; }

    public class UserDto
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string Role { get; set; }
    }
}