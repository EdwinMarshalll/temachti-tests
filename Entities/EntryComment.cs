using Microsoft.AspNetCore.Identity;

namespace Temachti.Api.Entities;

public class EntryComment
{
    public int Id { get; set; }
    public string Content { get; set; }
    public int Likes { get; set; }
    public string EntryId { get; set; }
    public string UserId { get; set; }

    public Entry Entry { get; set; }
    public IdentityUser User { get; set; }
}