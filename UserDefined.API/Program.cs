using UserDefined.Models;

using MicroService.Common.API;
using UserDefined.Models.Models;

//-:cnd:noEmit
#if MODEL_USEDTO
using UserDefined.DTOs;
#endif
//+:cnd:noEmit

namespace UserDefined.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region *** IMPORTANT PART
            //builder.Services.AddMVC(builder.Environment.IsProduction());
            //OR
            //builder.Services.AddMVC(builder.Environment.IsProduction(), builder.Configuration["SwaggerDocTitle"], builder.Configuration["SwaggerDocDescription"]);
            //OR
            builder.Services.AddMVC(builder.Environment.IsProduction(), "Subject", "API for subject model operations");

            /*This single call will bind every thing together.
             * Since we are using default Service class: Service<ISubject, Subject, int>,
             * the follwing call can be made:
             * If you want to add another model.
             * Please note that the previous type must not be repeated.
             * Duplicate models are not allowed.
             * 
            */

            //builder.Services.AddModel<Subject>(builder.Configuration);
            //var list = new List<Subject>();

            //-:cnd:noEmit
#if MODEL_USEDTO
            //If you are using DTO the follwing model can be added:
            //builder.Services.AddModel<ISubjectOutDTO, Subject, ISubjectInDTO>(builder.Configuration);

            //builder.Services.AddModelSingleton<ISubjectOutDTO, Subject, ISubjectInDTO>(builder.Configuration, list);

            builder.Services.AddModel<SubjectOutDTO, Subject, SubjectInDTO>(builder.Configuration);
#else
            /*
             * Single Subject object can be used as in and out type.
             * Not reccomended...
             */
            builder.Services.AddModel<Subject>(builder.Configuration);
#endif

#if !MODEL_NONQUERYABLE
#if MODEL_USEDTO
            builder.Services.AddKeyedQueryModel<SubjectOutDTO, Subject>(builder.Configuration);
#elif MODEL_NONREADABLE
            builder.Services.AddKeyedQueryModel<Subject>(builder.Configuration);
#endif
            builder.Services.AddQueryModel<FacultyInfo, FacultyInfo>(builder.Configuration);
            //builder.Services.AddKeyedQueryModelSingleton<ISubjectOutDTO, Subject>(builder.Configuration, list);
#endif

            //builder.Services.AddTransient<HttpExceptionMiddleWare>();
#endregion
            //+:cnd:noEmit

            var app = builder.Build();
            
            //-:cnd:noEmit
#if MODEL_USESWAGGER
            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
#endif
            //+:cnd:noEmit

            //if (app.Environment.IsDevelopment())
            //app.UseMiddleware<HttpExceptionMiddleWare>();

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}