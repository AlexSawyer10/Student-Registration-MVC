namespace StudentRegistration.API.Mappers;

public static class CoursesMapper
{
    public static Shared.Models.API.Course ToApiModel(this DbEntities.Course entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        
        return new Shared.Models.API.Course
        {
            COURSE_ID = entity.CourseId,
            TITLE = entity.Title,
            CREDITS = entity.Credits 
            
        };
    }
    
    public static DbEntities.Course ToEntity(this Shared.Models.API.Course model)
    {
        
        if (model == null)
        {
            throw new ArgumentNullException(nameof(model));
        }
        
        return new DbEntities.Course
        {
             CourseId = model.COURSE_ID,
            Title = model.TITLE,
            Credits = model.CREDITS
        };
    }
}
