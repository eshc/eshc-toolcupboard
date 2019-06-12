namespace ToolCupboard.Website

open System
open Microsoft.AspNetCore.Authorization
open Microsoft.AspNetCore.Authentication.Cookies
open Microsoft.AspNetCore.Builder
open Microsoft.AspNetCore.Hosting
open Microsoft.AspNetCore.Http
open Microsoft.AspNetCore.Mvc
open Microsoft.Extensions.Configuration
open Microsoft.Extensions.DependencyInjection
open ToolCupboard.Website.Services

type Startup private () =
    new (configuration: IConfiguration) as this =
        Startup() then
        this.Configuration <- configuration

    // This method gets called by the runtime. Use this method to add services to the container.
    member this.ConfigureServices(services: IServiceCollection) =
        ToolCupboard.Database.Provider.defaultConnectionString := 
            this.Configuration.["Database:ConnectionString"] |> Option.ofObj

        // Add framework services.
        services.AddSingleton<ILdapService, LdapService>() |> ignore

        services.Configure<CookiePolicyOptions>(fun (options : CookiePolicyOptions) -> 
                options.CheckConsentNeeded <- (fun v -> false)
                options.MinimumSameSitePolicy <- SameSiteMode.Strict
            ) |> ignore


        services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(fun opts ->
                opts.LoginPath <- PathString("/Auth/Login")
                opts.LogoutPath <- PathString("/Auth/Logout"))
            |> ignore

        services.AddAuthorization(fun opts ->
            opts.AddPolicy("AdminOnly", AuthorizationPolicyBuilder().RequireClaim("ToolAdmin", "True").Build())
        ) |> ignore

        services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2) |> ignore

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    member this.Configure(app: IApplicationBuilder, env: IHostingEnvironment) =

        if (env.IsDevelopment()) then
            app.UseDeveloperExceptionPage() |> ignore
        else
            app.UseExceptionHandler("/Home/Error") |> ignore
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts() |> ignore

        app.UseHttpsRedirection() |> ignore
        app.UseStaticFiles() |> ignore
        
        app
            .UseAuthentication()
            .UseCookiePolicy()
        |> ignore

        app.UseMvc(fun routes ->
            routes.MapRoute(
                name = "default",
                template = "{controller=Home}/{action=Index}/{id?}") |> ignore
            ) |> ignore

    member val Configuration : IConfiguration = null with get, set
