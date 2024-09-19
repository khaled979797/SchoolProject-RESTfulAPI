using Microsoft.Extensions.Localization;
using SchoolProject.Core.Resources;

namespace SchoolProject.Core.Bases
{
    public class ResponseHandler
    {
        private readonly IStringLocalizer<SharedResources> stringLocalizer;

        public ResponseHandler(IStringLocalizer<SharedResources> stringLocalizer)
        {
            this.stringLocalizer = stringLocalizer;
        }
        public Response<T> Deleted<T>(string message = null)
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true,
                Message = message == null ? stringLocalizer[SharedResourcesKeys.Deleted] : message

            };
        }
        public Response<T> Success<T>(T entity, object Meta = null)
        {
            return new Response<T>()
            {
                Data = entity,
                StatusCode = System.Net.HttpStatusCode.OK,
                Succeeded = true,
                Message = stringLocalizer[SharedResourcesKeys.Success],
                Meta = Meta
            };
        }
        public Response<T> Unauthorized<T>(string message = null)
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.Unauthorized,
                Succeeded = true,
                Message = message == null ? stringLocalizer[SharedResourcesKeys.UnAuthorized] : message
            };
        }
        public Response<T> BadRequest<T>(string message = null)
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.BadRequest,
                Succeeded = false,
                Message = message == null ? stringLocalizer[SharedResourcesKeys.BadRequest] : message
            };
        }

        public Response<T> UnprocessableEntity<T>(string message = null)
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.UnprocessableEntity,
                Succeeded = false,
                Message = message == null ? stringLocalizer[SharedResourcesKeys.UnprocessableEntity] : message
            };
        }

        public Response<T> NotFound<T>(string message = null)
        {
            return new Response<T>()
            {
                StatusCode = System.Net.HttpStatusCode.NotFound,
                Succeeded = false,
                Message = message == null ? stringLocalizer[SharedResourcesKeys.NotFound] : message
            };
        }

        public Response<T> Created<T>(T entity, object Meta = null)
        {
            return new Response<T>()
            {
                Data = entity,
                StatusCode = System.Net.HttpStatusCode.Created,
                Succeeded = true,
                Message = stringLocalizer[SharedResourcesKeys.Created],
                Meta = Meta
            };
        }
    }
}
