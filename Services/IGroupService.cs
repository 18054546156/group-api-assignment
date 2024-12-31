public interface IGroupService
{
    Task<string> GetAccessTokenAsync();
    Task<bool> CreateGroupAsync(string groupName);
    Task<IEnumerable<GroupDto>> GetGroupsAsync();
} 