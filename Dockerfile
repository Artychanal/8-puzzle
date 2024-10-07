# Указываем базовый образ для .NET 8.0
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем файлы проекта и восстанавливаем зависимости
COPY ConsoleApp1.csproj ./
RUN dotnet restore

# Копируем остальные файлы и собираем проект
COPY . ./
RUN dotnet publish -c Release -o out

# Указываем базовый образ для запуска приложения
FROM mcr.microsoft.com/dotnet/runtime:8.0

# Устанавливаем рабочую директорию
WORKDIR /app

# Копируем собранное приложение из образа сборки
COPY --from=build /app/out .

# Указываем команду для запуска приложения
ENTRYPOINT ["dotnet", "ConsoleApp1.dll"]
