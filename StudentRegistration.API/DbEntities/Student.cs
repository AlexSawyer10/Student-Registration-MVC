using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace StudentRegistration.API.DbEntities;

[Table("STUDENT")]
[Index("Email", Name = "EMAIL", IsUnique = true)]
public partial class Student
{
    [Key]
    [Column("STUDENT_ID")]
    public int StudentId { get; set; }

    [Column("FIRST_NAME")]
    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [Column("LAST_NAME")]
    [StringLength(50)]
    public string LastName { get; set; } = null!;

    [Column("EMAIL")]
    [StringLength(100)]
    public string Email { get; set; } = null!;

    [Column("PHONE_NUMBER")]
    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    [Column("DATE_OF_BIRTH", TypeName = "date")]
    public DateTime? DateOfBirth { get; set; }

    [Column("MAJOR")]
    [StringLength(100)]
    public string? Major { get; set; }

    [InverseProperty("Student")]
    public virtual ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
}
