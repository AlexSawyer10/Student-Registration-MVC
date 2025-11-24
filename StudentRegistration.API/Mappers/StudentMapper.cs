using System.Runtime.InteropServices.JavaScript;

namespace StudentRegistration.API.Mappers;

public static class StudentMapper 
{
    public static Shared.Models.API.Student ToApiModel(this DbEntities.Student entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        return new Shared.Models.API.Student
        {
            STUDENT_ID = entity.StudentId,
            FIRST_NAME = entity.FirstName,
            LAST_NAME = entity.LastName,
            EMAIL = entity.Email,
            PHONE_NUMBER = entity.PhoneNumber,
            DATE_OF_BIRTH = entity.DateOfBirth ?? DateTime.MinValue, // can be null
            MAJOR = entity.Major
        };
        
        
    }

    public static DbEntities.Student ToEntity(this Shared.Models.API.Student model)
    {
        
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        
        return new DbEntities.Student
        {
            StudentId = model.STUDENT_ID,
            FirstName = model.FIRST_NAME,
            LastName = model.LAST_NAME,
            Email = model.EMAIL,
            PhoneNumber = model.PHONE_NUMBER,
            DateOfBirth = model.DATE_OF_BIRTH,
            Major = model.MAJOR,
        };
    }
}