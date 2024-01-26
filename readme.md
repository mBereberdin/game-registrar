# Game registrar

<!-- Актуализировать после импорта -->
[![Build](https://github.com/mBereberdin/game-registrar/actions/workflows/Build.yml/badge.svg)](https://github.com/mBereberdin/game-registrar/actions/workflows/Build.yml)

## Описание проекта

Сабмодуль для игр, который позволяет регистрировать игру в узле игр (games-hub).

## Как использовать

1. Определить настройки регистратора в appsettings проекта;

```json
  "RegistrarSettings": {
    "GameName": "game",
    "RegistrationTimeSeconds": "10"
  }
```

2. определить настройки узла игр в appsettings проекта;

```json
  "GamesHubSettings": {
    "Host": "http://localhost:5002",
    "RegisterEndpoint": "/games/"
  }
```

3. в Program вызвать расширение для подключения настроек и сервисов модуля.

```csharp
builder.Services.AddGamesHubRegistration(builder.Configuration);
```

## Используемые технологии

- .net core 7
- serilog
