FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /build

COPY ["src/Api/Api.csproj", "src/Api/"]
COPY ["src/Application/Application.csproj", "src/Application/"]
COPY ["src/Domain/Domain.csproj", "src/Domain/"]
COPY ["src/Infrastructure/Infrastructure.csproj", "src/Infrastructure/"]
COPY ["Directory.Packages.props", "."]
COPY ["Directory.Build.props", "."]

RUN dotnet restore "src/Api/Api.csproj"

COPY . .

RUN dotnet publish "src/Api/Api.csproj" -c Release -o /app

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final

RUN apt-get update && apt-get install -y tzdata && \
    ln -fs /usr/share/zoneinfo/Asia/Baghdad /etc/localtime && \
    dpkg-reconfigure -f noninteractive tzdata && \
    rm -rf /var/lib/apt/lists/*

ENV TZ=Asia/Baghdad

WORKDIR /app
COPY --from=build /app .
EXPOSE 80
ENTRYPOINT ["dotnet", "Api.dll"]