using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PostQuery.Domain.Entities;

[Table("Comment", Schema = "dbo")]
public class Comment
{
    [Key]
    public Guid Id { get; set; }
    public string UserName { get; set; }
    public DateTime CommentDate { get; set; }
    public string CommentText { get; set; }
    public bool Edited { get; set; }
    public Guid PostId { get; set; }
    public virtual Post Post { get; set; }

}