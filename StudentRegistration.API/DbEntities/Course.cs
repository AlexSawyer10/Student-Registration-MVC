using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StudentRegistration.API.DbEntities;

[Table("COURSE")]
public partial class Course
{
    [Key]
    [Column("COURSE_ID")]
    public int CourseId { get; set; }

    [Column("TITLE")]
    [StringLength(100)]
    public string Title { get; set; } = null!;

    [Column("CREDITS")]
    public int Credits { get; set; }

    [InverseProperty("Course")]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
