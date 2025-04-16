# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

COPY JourneyMentor.Loyalty.sln ./
COPY JourneyMentor.Loyalty.API/*.csproj JourneyMentor.Loyalty.API/
COPY JourneyMentor.Loyalty.Application/*.csproj JourneyMentor.Loyalty.Application/
COPY JourneyMentor.Loyalty.Domain/*.csproj JourneyMentor.Loyalty.Domain/
COPY JourneyMentor.Loyalty.Infrastructure/*.csproj JourneyMentor.Loyalty.Infrastructure/
COPY JourneyMentor.Loyalty.Persistence/*.csproj JourneyMentor.Loyalty.Persistence/
COPY JourneyMentor.Loyalty.Tests/*.csproj JourneyMentor.Loyalty.Tests/

RUN dotnet restore

COPY . .

# Install EF CLI
RUN dotnet tool install --global dotnet-ef
ENV PATH="${PATH}:/root/.dotnet/tools"

# Run migration
RUN dotnet ef database update --project JourneyMentor.Loyalty.Persistence/JourneyMentor.Loyalty.Persistence.csproj --startup-project JourneyMentor.Loyalty.API/JourneyMentor.Loyalty.API.csproj

RUN dotnet publish JourneyMentor.Loyalty.API/JourneyMentor.Loyalty.API.csproj -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/publish .

ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Development

EXPOSE 80
ENTRYPOINT ["dotnet", "JourneyMentor.Loyalty.API.dll"]
