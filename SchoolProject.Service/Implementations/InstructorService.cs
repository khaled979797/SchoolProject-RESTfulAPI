using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchoolProject.Data.Entities;
using SchoolProject.Infrastructure.Abstracts;
using SchoolProject.Infrastructure.Abstracts.Functions;
using SchoolProject.Infrastructure.Data;
using SchoolProject.Service.Abstracts;

namespace SchoolProject.Service.Implementations
{
    public class InstructorService : IInstructorService
    {
        #region Fileds
        private readonly AppDbContext context;
        private readonly IInstructorFunctionsRepository instructorFunctionsRepository;
        private readonly IFileService fileService;
        private readonly IHttpContextAccessor httpContextAccessor;
        private readonly IInstructorRepository instructorsRepository;

        #endregion
        #region Constructors
        public InstructorService(AppDbContext context, IInstructorRepository instructorsRepository,
            IInstructorFunctionsRepository instructorFunctionsRepository, IFileService fileService,
            IHttpContextAccessor httpContextAccessor)
        {
            this.context = context;
            this.instructorFunctionsRepository = instructorFunctionsRepository;
            this.fileService = fileService;
            this.httpContextAccessor = httpContextAccessor;
            this.instructorsRepository = instructorsRepository;
        }
        #endregion

        #region Functions
        public async Task<decimal> GetSalarySummationOfInstructor()
        {
            return instructorFunctionsRepository.GetSalarySummationOfInstructor("select dbo.GetSalarySummation()");
        }

        public async Task<bool> IsNameArExist(string nameAr)
        {
            return instructorsRepository.GetTableNoTracking().
                Where(x => x.ENameAr.Equals(nameAr)).FirstOrDefault() == null ? false : true;
        }
        public async Task<bool> IsNameEnExist(string nameEn)
        {
            return await instructorsRepository.GetTableNoTracking().
                Where(x => x.ENameEn.Equals(nameEn)).FirstOrDefaultAsync() == null ? false : true;
        }

        public async Task<bool> IsNameArExistExcludeSelf(string nameAr, int id)
        {
            return await instructorsRepository.GetTableNoTracking().Where(x => x.ENameAr.Equals(nameAr)
            & x.InsId != id).FirstOrDefaultAsync() == null ? false : true;
        }

        public async Task<bool> IsNameEnExistExcludeSelf(string nameEn, int id)
        {
            return await instructorsRepository.GetTableNoTracking().Where(x => x.ENameEn.Equals(nameEn)
            & x.InsId != id).FirstOrDefaultAsync() == null ? false : true;
        }
        public async Task<string> AddInstructorAsync(Instructor instructor, IFormFile file)
        {
            var context = httpContextAccessor.HttpContext.Request;
            var baseUrl = context.Scheme + "://" + context.Host;
            var imageUrl = await fileService.UploadImage("Instructors", file);
            switch (imageUrl)
            {
                case "NoImage": return "NoImage";
                case "FailedToUploadImage": return "FailedToUploadImage";
            }
            instructor.Image = baseUrl + imageUrl;
            try
            {
                await instructorsRepository.AddAsync(instructor);
                return "Success";
            }
            catch
            {
                return "FailedToAdd";
            }
        }
        #endregion
    }
}
