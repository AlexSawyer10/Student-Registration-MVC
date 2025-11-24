using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StudentRegistration.API.DbEntities;

[Table("ENROLLMENT")]
[Index("CourseId", Name = "FK_ENROLLMENT_COURSE")]
[Index("StudentId", "CourseId", Name = "UQ_ENROLLMENT_UNIQUE", IsUnique = true)]
public partial class Enrollment
{
    [Key]
    [Column("ENROLLMENT_ID")]
    public int EnrollmentId { get; set; }

    [Column("STUDENT_ID")]
    public int StudentId { get; set; }

    [Column("COURSE_ID")]
    public int CourseId { get; set; }

    [Column("GRADE")]
    [Precision(5)]
    public decimal? Grade { get; set; }

    [ForeignKey("CourseId")]
    [InverseProperty("Enrollments")]
    public virtual Course Course { get; set; } = null!;

    [ForeignKey("StudentId")]
    [InverseProperty("Enrollments")]
    public virtual Student Student { get; set; } = null!;
}
