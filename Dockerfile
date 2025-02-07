FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY ["src/InvoiceApp.API/InvoiceApp.API.csproj", "src/InvoiceApp.API/"]
COPY ["src/InvoiceApp.Application/InvoiceApp.Application.csproj", "src/InvoiceApp.Application/"]
COPY ["src/InvoiceApp.Infrastructure/InvoiceApp.Infrastructure.csproj", "src/InvoiceApp.Infrastructure/"]
COPY ["src/InvoiceApp.Domain/InvoiceApp.Domain.csproj", "src/InvoiceApp.Domain/"]
RUN dotnet restore "src/InvoiceApp.API/InvoiceApp.API.csproj"
COPY . .
RUN dotnet build "src/InvoiceApp.API/InvoiceApp.API.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "src/InvoiceApp.API/InvoiceApp.API.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "InvoiceApp.API.dll"]