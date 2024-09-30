using Microsoft.Extensions.DependencyInjection;
using SchoolProject.Data.Entities.Views;
using SchoolProject.Infrastructure.Abstracts;
using SchoolProject.Infrastructure.Abstracts.Functions;
using SchoolProject.Infrastructure.Abstracts.Procedures;
using SchoolProject.Infrastructure.Abstracts.Views;
using SchoolProject.Infrastructure.InfrastructureBases;
using SchoolProject.Infrastructure.Repositories;
using SchoolProject.Infrastructure.Repositories.Functions;
using SchoolProject.Infrastructure.Repositories.Procedures;
using SchoolProject.Infrastructure.Repositories.Views;

namespace SchoolProject.Infrastructure
{
    public static class ModuleInfrastructureDependencies
    {
        public static IServiceCollection AddInfrastructureDependencies(this IServiceCollection services)
        {
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddTransient<IStudentRepository, StudentRepository>();
            services.AddTransient<IDepartmentRepository, DepartmentRepository>();
            services.AddTransient<IInstructorRepository, InstructorRepository>();
            services.AddTransient<ISubjectRepository, SubjectRepository>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<IViewRepository<ViewDepartment>, ViewDepartmentRepository>();
            services.AddTransient<IDepartmentStudentcountProcRepository, DepartmentStudentcountProcRepository>();
            services.AddTransient<IInstructorFunctionsRepository, InstructorFunctionsRepository>();

            return services;
        }
    }
}
