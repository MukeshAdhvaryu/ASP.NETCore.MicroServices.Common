# Stage 1: Build and compile the application
FROM mcr.microsoft.com/dotnet/sdk:7.0 AS build
WORKDIR /app

# Copy the solution file and restore dependencies
COPY ASP.NETCore.MicroService.Common.sln .
COPY . .

#Assuming we want to exclude test projects, the follwing will be required 
#Since we have already put folder names in dockerignore file.
#If we do not want to exclude test projects, then remove the following 2 lines
#then also remove folder names from dockerignore file.

RUN sed -i '/Service.Tests.csproj/d' ASP.NETCore.MicroService.Common.sln
RUN sed -i '/Standard.Tests.csproj/d' ASP.NETCore.MicroService.Common.sln

# Restore dependencies and build the application
RUN dotnet restore 
RUN dotnet publish -c Release -o out


# Stage 2: Create the runtime image
FROM mcr.microsoft.com/dotnet/aspnet:7.0 AS runtime
WORKDIR /app

# Copy only the necessary files from the build stage
COPY --from=build /app/out ./

# Expose the port that your application will run on
EXPOSE 80

# Specify the entry point for your application
ENTRYPOINT ["dotnet", "UserDefined.API.dll"]
