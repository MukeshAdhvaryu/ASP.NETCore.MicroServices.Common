using UserDefined.Models;
using MicroService.Common.Web.API;
using Microsoft.Extensions.Options;
using System.Text.Json.Serialization;

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
            builder.Services.AddMVC();

            /*This single call will bind every thing together.
             * Since we are using default Service class: Service<ISubject, Subject, int>,
             * the follwing call can be made:
             * If you want to add another model.
             * Please note that the previous type must not be repeated.
             * Duplicate models are not allowed.
             * 
            */

            //builder.Services.AddModel<ISubject, Subject>(builder.Configuration);

            //-:cnd:noEmit
#if MODEL_USEDTO
            //If you are using DTO the follwing model can be added:
            builder.Services.AddModel<ISubjectDTO, Subject>(builder.Configuration);
#else
            /*
             * Single Subject object can be used as in and out type.
             * Not reccomended...
             */
            builder.Services.AddModel<ISubject, Subject>(builder.Configuration);
#endif
            //+:cnd:noEmit
            #endregion

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(option=>
            {
                option.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            //-:cnd:noEmit
#if MODEL_USESWAGGER
            builder.Services.AddSwaggerGen(opt => {
                opt.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = builder.Configuration["SwaggerDocTitle"],
                    Description = builder.Configuration["SwaggerDocDescription"]
                });

                
                opt.SchemaFilter<EnumSchemaFilter>();
            });
#endif
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

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}