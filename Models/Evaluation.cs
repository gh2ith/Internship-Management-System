using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models;

public class Evaluation
{
    [Key]
    [Column("eval_id")]
    public int EvalId { get; set; }

    [Column("student_id")]
    public int? StudentId { get; set; }

    [Column("comp_id")]
    public int? CompId { get; set; }

    [Column("super_id")]
    public int? SuperId { get; set; }

    [Column("performance_level")]
    public string? PerformanceLevel { get; set; }

    [Column("responsibility")]
    public string? Responsibility { get; set; }

    [Column("punctuality")]
    public string? Punctuality { get; set; }

    [Column("accuracy")]
    public string? Accuracy { get; set; }

    [Column("teamwork")]
    public string? Teamwork { get; set; }

    [Column("adaptability")]
    public string? Adaptability { get; set; }

    [Column("skill_acquisition")]
    public string? SkillAcquisition { get; set; }

    [Column("overall_completion")]
    public string? OverallCompletion { get; set; }

    [Column("comments")]
    public string? Comments { get; set; }

    [Column("created_at")]
    public DateTime? CreatedAt { get; set; }

    [ForeignKey("StudentId")]
    public virtual Student? Student { get; set; }

    [ForeignKey("CompId")]
    public virtual Company? Comp { get; set; }

    [ForeignKey("SuperId")]
    public virtual Supervisor? Super { get; set; }
}
