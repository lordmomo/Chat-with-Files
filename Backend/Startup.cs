//using GenAIApp.Utlis;
//using Microsoft.AspNetCore.Builder;
//using System.Net.Http.Headers;

//namespace GenAIApp
//{
//    public class Startup
//    {
//        const string apiKey = "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";

//        public void ConfigureServices(IServiceCollection services)
//        {
//            services.AddControllers();
//            services.AddEndpointsApiExplorer();
//            services.AddSwaggerGen();
//            services.AddTransient<DocumentProcessor>();
//            services.AddHttpClient<AIClient>(client =>
//            {
//                //client.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");
//                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", $"{apiKey}");

//            });

//            services.AddTransient<DocumentProcessor>();

//        }

//        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
//        {
//            if (app.IsDevelopment())
//            {
//                app.UseSwagger();
//                app.UseSwaggerUI();
//            }

//            app.UseHttpsRedirection();

//            app.UseAuthorization();


//            app.MapControllers();

//            app.Run();

//        }
//    }
//}
