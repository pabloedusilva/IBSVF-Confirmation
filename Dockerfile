# Use a imagem base do ASP.NET Core Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

# Use a imagem do SDK para build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copiar o arquivo de projeto e restaurar dependências
COPY ["IBSVF.Web.csproj", "./"]
RUN dotnet restore "IBSVF.Web.csproj"

# Copiar todo o código fonte
COPY . .

# Build da aplicação
RUN dotnet build "IBSVF.Web.csproj" -c Release -o /app/build

# Publish da aplicação
FROM build AS publish
RUN dotnet publish "IBSVF.Web.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Estágio final
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

# Definir a porta de entrada
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "IBSVF.Web.dll"]
