

using GenAIApp.Services.ServiceImplementation;
using GenAIApp.Services.ServiceInterface;
using GenAIApp.Utlis;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.DataProtection.KeyManagement;
using Microsoft.ML;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GenAIApp
{
    public class Program
    {
        public static void Main(string[] args)
        {

            var builder = WebApplication.CreateBuilder(args);


            // Add services to the container.

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAllOrigins",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                               .AllowAnyMethod()
                               .AllowAnyHeader();
                    });
            });

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddHttpClient();


            //builder.Services.AddSingleton(new AIClient(new HttpClient(), apiKey));

            //builder.Services.AddHttpClient<AIClient>( client =>
            //{
            //    //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
            //    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);
            //    client.DefaultRequestHeaders.Append

            //});
            builder.Services.AddSingleton<MLContext>(new MLContext());

            builder.Services.AddSingleton<DocumentRepository>();
            builder.Services.AddTransient<DocumentProcessor>();
            builder.Services.AddTransient<DocHelper>();

            builder.Services.AddTransient<AIClient>();

            builder.Services.AddTransient<QuestionServiceInterface, QuestionServiceImplementation>();

            var app = builder.Build();

            app.UseCors("AllowAllOrigins");

            //Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }


    }
}
