FROM mcr.microsoft.com/dotnet/sdk:8.0

# Installa le dipendenze necessarie
RUN apt-get update && apt-get install -y \
    curl \
    git \
    && rm -rf /var/lib/apt/lists/*

WORKDIR /app
COPY . .
RUN dotnet restore

EXPOSE 5000 5001

ENTRYPOINT ["dotnet", "watch", "run", "--urls", "http://0.0.0.0:5000;https://0.0.0.0:5001"] 