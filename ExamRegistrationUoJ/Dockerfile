#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Debug
WORKDIR /src
COPY ["ExamRegistrationUoJ/ExamRegistrationUoJ.csproj", "ExamRegistrationUoJ/"]
RUN dotnet restore "./ExamRegistrationUoJ/ExamRegistrationUoJ.csproj"
COPY . .
WORKDIR "/src/ExamRegistrationUoJ"
RUN dotnet build "./ExamRegistrationUoJ.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Debug
RUN dotnet publish "./ExamRegistrationUoJ.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ExamRegistrationUoJ.dll"]
