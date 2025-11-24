namespace StudentRegistration.API.Mappers;

public static class EnrollmentsMapper
{
    public static Shared.Models.API.Enrollment ToApiModel(this DbEntities.Enrollment entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        
        return new Shared.Models.API.Enrollment
        {
            ENROLLMENT_ID = entity.EnrollmentId,
            STUDENT_ID = entity.StudentId,
            COURSE_ID = entity.CourseId,
            GRADE = entity.Grade
            
        };
    }
    
    public static DbEntities.Enrollment ToEntity(this Shared.Models.API.Enrollment model)
    {
        
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        
        return new DbEntities.Enrollment
        {
            EnrollmentId = model.ENROLLMENT_ID,
            CourseId = model.COURSE_ID,
            Grade = model.GRADE
        };
    }
}